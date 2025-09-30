// Boamesa.Application/Services/SugestaoDoChefeService.cs
using Boamesa.Application.DTOs;
using Boamesa.Domain.Entities;
using Boamesa.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Boamesa.Application.Services;

public class SugestaoDoChefeService
{
    private readonly BoamesaContext _db;
    public SugestaoDoChefeService(BoamesaContext db) => _db = db;

    public async Task<SugestaoVm> CriarAsync(CriarSugestaoDto dto, CancellationToken ct = default)
    {
        // 1 por data+periodo
        bool existe = await _db.Sugestoes.AnyAsync(s => s.Data == dto.Data && s.Periodo == dto.Periodo, ct);
        if (existe) throw new BusinessRuleException("Já existe Sugestão do Chefe para essa data e período.");

        var item = await _db.ItensCardapio.AsNoTracking().FirstOrDefaultAsync(i => i.Id == dto.ItemCardapioId, ct);
        if (item is null) throw new BusinessRuleException("Item de cardápio não encontrado.");
        if (item.Periodo != dto.Periodo) throw new BusinessRuleException("Item não pertence ao mesmo período da sugestão.");
        if (dto.DescontoPercentual <= 0 || dto.DescontoPercentual > 1)
            throw new BusinessRuleException("DescontoPercentual deve estar entre 0 e 1 (ex.: 0.20 para 20%).");

        var entidade = new SugestaoDoChefe {
            Data = dto.Data,
            Periodo = dto.Periodo,
            ItemCardapioId = dto.ItemCardapioId,
            DescontoPercentual = dto.DescontoPercentual
        };

        _db.Sugestoes.Add(entidade);
        await _db.SaveChangesAsync(ct);

        return new SugestaoVm(entidade.Id, entidade.Data, entidade.Periodo, entidade.ItemCardapioId, entidade.DescontoPercentual);
    }

    public async Task<List<SugestaoVm>> ObterPorDataAsync(DateOnly data, CancellationToken ct = default)
    {
        return await _db.Sugestoes
            .Where(s => s.Data == data)
            .Select(s => new SugestaoVm(s.Id, s.Data, s.Periodo, s.ItemCardapioId, s.DescontoPercentual))
            .ToListAsync(ct);
    }
}
