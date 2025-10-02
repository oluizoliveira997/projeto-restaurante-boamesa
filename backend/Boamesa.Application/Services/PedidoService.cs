using Boamesa.Application.DTOs;
using Boamesa.Domain.Entities;
using Boamesa.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Boamesa.Application.Services;

public class PedidoService
{
    private readonly BoamesaContext _db;
    public PedidoService(BoamesaContext db) => _db = db;

    public async Task<PedidoVm> CriarPedidoAsync(PedidoCreateDto dto, CancellationToken ct = default)
    {
        // 1) valida usuário
        var usuario = await _db.Usuarios.FindAsync(new object?[] { dto.UsuarioId }, ct);
        if (usuario is null) throw new BusinessRuleException("Usuário não encontrado.");

        if (dto.Itens == null || dto.Itens.Count == 0)
            throw new BusinessRuleException("Informe pelo menos 1 item.");

        // 2) carrega itens de cardápio
        var ids = dto.Itens.Select(i => i.ItemCardapioId).Distinct().ToList();
        var cardapios = await _db.ItensCardapio.Where(i => ids.Contains(i.Id)).ToListAsync(ct);
        if (cardapios.Count != ids.Count)
            throw new BusinessRuleException("Um ou mais itens do cardápio não foram encontrados.");

        // 3) valida ativo e período
        foreach (var ic in cardapios)
        {
            if (!ic.Ativo) throw new BusinessRuleException($"Item '{ic.Nome}' está inativo.");
            if (ic.Periodo != dto.Periodo)
                throw new BusinessRuleException($"Item '{ic.Nome}' não pertence ao período do pedido ({dto.Periodo}).");
        }

        // 4) cria a instância de Atendimento (com validação do ParceiroAppId p/ DeliveryAplicativo)
        Atendimento atendimento = dto.TipoAtendimento?.ToLowerInvariant() switch
        {
            "presencial" => new AtendimentoPresencial(),

            "deliveryproprio" => new AtendimentoDeliveryProprio
            {
                TaxaFixa = 5m
            },

            "deliveryaplicativo" => CreateDeliveryApp(dto),

            _ => new AtendimentoPresencial()
        };
        _db.Atendimentos.Add(atendimento);

        // 5) monta pedido + itens
        var pedido = new Pedido
        {
            UsuarioId   = dto.UsuarioId,
            DataHora    = DateTime.UtcNow,
            Periodo     = dto.Periodo,
            Status      = "Criado",
            Atendimento = atendimento,
            Itens       = new List<PedidoItem>()
        };

        foreach (var it in dto.Itens)
        {
            var ic = cardapios.First(c => c.Id == it.ItemCardapioId);
            var preco = it.PrecoUnitario > 0 ? it.PrecoUnitario : ic.PrecoBase;

            pedido.Itens.Add(new PedidoItem
            {
                ItemCardapioId   = ic.Id,
                Quantidade       = it.Quantidade,
                PrecoUnitario    = preco,
                DescontoAplicado = it.DescontoAplicado
            });
        }

        _db.Pedidos.Add(pedido);
        await _db.SaveChangesAsync(ct);

        // 6) totais (líquido + taxas)
        var totalItens = pedido.Itens.Sum(i => i.Subtotal());
        var taxas      = pedido.Atendimento?.CalcularTaxa(pedido) ?? 0m;
        var totalGeral = totalItens + taxas;

        return new PedidoVm
        {
            Id              = pedido.Id,
            UsuarioId       = pedido.UsuarioId,
            Periodo         = pedido.Periodo,
            Status          = pedido.Status,
            AtendimentoTipo = dto.TipoAtendimento,
            TotalItens      = totalItens,
            TotalGeral      = totalGeral,
            DataHora        = pedido.DataHora
        };
    }

    private static AtendimentoDeliveryAplicativo CreateDeliveryApp(PedidoCreateDto dto)
    {
        if (dto.ParceiroAppId is null)
            throw new BusinessRuleException("Parceiro do app é obrigatório para DeliveryAplicativo.");

        return new AtendimentoDeliveryAplicativo
        {
            ComissaoPercentual = 0.12m,
            TaxaFixaParceiro   = 2m,
            ParceiroAppId      = dto.ParceiroAppId
        };
    }

    public async Task<PedidoVm?> ObterPedidoAsync(int id, CancellationToken ct = default)
    {
        var p = await _db.Pedidos
            .AsNoTracking()
            .Include(p => p.Itens)
            .Include(p => p.Atendimento)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (p is null) return null;

        var tipoAtend = p.Atendimento switch
        {
            AtendimentoDeliveryAplicativo => "DeliveryAplicativo",
            AtendimentoDeliveryProprio    => "DeliveryProprio",
            AtendimentoPresencial         => "Presencial",
            _                             => "Presencial"
        };

        var totalItens = p.Itens.Sum(i => i.Subtotal());
        var taxas      = p.Atendimento?.CalcularTaxa(p) ?? 0m;
        var totalGeral = totalItens + taxas;

        return new PedidoVm
        {
            Id              = p.Id,
            UsuarioId       = p.UsuarioId,
            Periodo         = p.Periodo,
            Status          = p.Status,
            AtendimentoTipo = tipoAtend,
            TotalItens      = totalItens,
            TotalGeral      = totalGeral,
            DataHora        = p.DataHora
        };
    }
}
