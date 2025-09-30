// Boamesa.Application/Services/ReservaService.cs
using Boamesa.Application.DTOs;
using Boamesa.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Boamesa.Application.Services;

public class ReservaService
{
    private readonly BoamesaContext _db;
    public ReservaService(BoamesaContext db) => _db = db;

    public async Task<ReservaVm> CriarAsync(CriarReservaDto dto, CancellationToken ct = default)
    {
        // Janela jantar 19:00 ≤ h < 22:00
        var h = dto.DataHora.TimeOfDay;
        if (h < TimeSpan.FromHours(19) || h >= TimeSpan.FromHours(22))
            throw new BusinessRuleException("Reservas permitidas apenas no jantar (entre 19:00 e 22:00).");

        bool usuarioOk = await _db.Usuarios.AnyAsync(u => u.Id == dto.UsuarioId, ct);
        if (!usuarioOk) throw new BusinessRuleException("Usuário não encontrado.");

        bool mesaOk = await _db.Mesas.AnyAsync(m => m.Id == dto.MesaId, ct);
        if (!mesaOk) throw new BusinessRuleException("Mesa não encontrada.");

        // Conflito por mesa (overlap 2h)
        var ini = dto.DataHora;
        var fim = dto.DataHora.AddHours(2);
        bool conflito = await _db.Reservas.AnyAsync(r =>
            r.MesaId == dto.MesaId &&
            r.Status != "Cancelada" &&
            r.DataHora < fim && r.DataHora.AddHours(2) > ini, ct);

        if (conflito) throw new BusinessRuleException("Já existe reserva para essa mesa nesse horário.");

        var rsv = new Boamesa.Domain.Entities.Reserva {
            UsuarioId = dto.UsuarioId,
            MesaId = dto.MesaId,
            DataHora = dto.DataHora,
            Status = "Ativa",
            CodigoConfirmacao = GerarCodigo()
        };

        _db.Reservas.Add(rsv);
        await _db.SaveChangesAsync(ct);

        return new ReservaVm(rsv.Id, rsv.UsuarioId, rsv.MesaId, rsv.DataHora, rsv.CodigoConfirmacao!, rsv.Status);
    }

    public async Task<List<ReservaVm>> ListarPorDataAsync(DateOnly data, CancellationToken ct = default)
    {
        var start = data.ToDateTime(TimeOnly.MinValue);
        var end = data.ToDateTime(TimeOnly.MaxValue);
        return await _db.Reservas
            .Where(r => r.DataHora >= start && r.DataHora <= end)
            .Select(r => new ReservaVm(r.Id, r.UsuarioId, r.MesaId, r.DataHora, r.CodigoConfirmacao!, r.Status))
            .ToListAsync(ct);
    }

    private static string GerarCodigo()
    {
        var s = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("+", "").Replace("/", "").Replace("=", "");
        return s[..8].ToUpperInvariant();
    }
}
