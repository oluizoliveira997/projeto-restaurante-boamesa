// Boamesa.Application/Services/PedidoService.cs
using Boamesa.Application.DTOs;
using Boamesa.Domain.Entities;
using Boamesa.Domain.Enums;
using Boamesa.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Boamesa.Application.Services;

public class PedidoService
{
    private readonly BoamesaContext _db;
    public PedidoService(BoamesaContext db) => _db = db;

    public async Task<PedidoVm> CriarAsync(CriarPedidoDto dto, CancellationToken ct = default)
    {
        if (dto.Itens is null || dto.Itens.Count == 0)
            throw new BusinessRuleException("Pedido deve conter ao menos um item.");

        // Validar itens e período
        var ids = dto.Itens.Select(i => i.ItemCardapioId).ToList();
        var itensDb = await _db.ItensCardapio
            .Where(i => ids.Contains(i.Id))
            .Select(i => new { i.Id, i.Periodo })
            .ToListAsync(ct);

        if (itensDb.Count != ids.Count)
            throw new BusinessRuleException("Há item(s) de cardápio inexistente(s) no pedido.");

        if (itensDb.Any(i => i.Periodo != dto.Periodo))
            throw new BusinessRuleException("Todos os itens devem pertencer ao mesmo período do pedido.");

        // Montar atendimento
        Atendimento atendimento = dto.TipoAtendimento switch
        {
            "Presencial" => new AtendimentoPresencial(),
            "DeliveryProprio" => new AtendimentoDeliveryProprio { TaxaFixa = dto.TaxaFixa ?? 0m },
            "DeliveryApp" => new AtendimentoDeliveryAplicativo {
                ComissaoPercentual = dto.ComissaoPercentual ?? 0m,
                TaxaFixaParceiro = dto.TaxaFixaParceiro,
                ParceiroAppId = dto.ParceiroAppId ?? throw new BusinessRuleException("ParceiroAppId é obrigatório para DeliveryApp."),
            },
            _ => throw new BusinessRuleException("TipoAtendimento inválido. Use Presencial, DeliveryProprio ou DeliveryApp.")
        };

        var pedido = new Pedido {
            UsuarioId = dto.UsuarioId,
            Periodo = dto.Periodo,
            Atendimento = atendimento,
            Status = "Criado",
            Itens = dto.Itens.Select(i => new PedidoItem {
                ItemCardapioId = i.ItemCardapioId,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario,
                DescontoAplicado = i.DescontoAplicado
            }).ToList()
        };

        _db.Pedidos.Add(pedido);
        await _db.SaveChangesAsync(ct);

        return new PedidoVm(
            pedido.Id,
            pedido.Periodo,
            pedido.Status,
            pedido.TotalItens(),
            pedido.TotalDescontos(),
            pedido.TotalTaxas(),
            pedido.TotalGeral()
        );
    }

    public async Task<PedidoVm?> ObterAsync(int id, CancellationToken ct = default)
    {
        var p = await _db.Pedidos
            .Include(p => p.Itens)
            .Include(p => p.Atendimento)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        return p is null ? null : new PedidoVm(
            p.Id, p.Periodo, p.Status, p.TotalItens(), p.TotalDescontos(), p.TotalTaxas(), p.TotalGeral()
        );
    }
}
