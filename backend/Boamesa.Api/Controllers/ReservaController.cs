using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Boamesa.Infrastructure;                 // BoamesaContext
using Boamesa.Application.Services;          // ReservaService
using Boamesa.Application.DTOs;              // CriarReservaDto, ReservaVm

namespace Boamesa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservasController : ControllerBase
{
    private readonly ReservaService _svc;
    private readonly BoamesaContext _db;

    public ReservasController(ReservaService svc, BoamesaContext db)
    {
        _svc = svc;
        _db  = db;
    }

    // POST: api/reservas
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CriarReservaDto dto, CancellationToken ct)
    {
        var r = await _svc.CriarAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = r.Id }, r);
    }

    // GET: api/reservas/5
    // (Como o service não tem ObterPorIdAsync, projetamos direto para ReservaVm)
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var r = await _db.Reservas
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new ReservaVm(
                x.Id, x.UsuarioId, x.MesaId, x.DataHora,
                x.CodigoConfirmacao!, x.Status))
            .FirstOrDefaultAsync(ct);

        return r is null ? NotFound() : Ok(r);
    }

    // GET: api/reservas?data=2025-10-01
    // Lista por data usando o método do service
    [HttpGet]
    public async Task<IActionResult> GetByDate([FromQuery] string? data, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            // sem data => lista todas (projeção rápida)
            var todas = await _db.Reservas.AsNoTracking()
                .OrderByDescending(r => r.DataHora)
                .Select(x => new ReservaVm(
                    x.Id, x.UsuarioId, x.MesaId, x.DataHora,
                    x.CodigoConfirmacao!, x.Status))
                .ToListAsync(ct);

            return Ok(todas);
        }

        if (!DateOnly.TryParse(data, out var d))
            return BadRequest("Data inválida. Use yyyy-MM-dd.");

        var lista = await _svc.ListarPorDataAsync(d, ct);
        return Ok(lista);
    }

    // DELETE: api/reservas/5  (cancela)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        var r = await _db.Reservas.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (r is null) return NotFound();

        r.Status = "Cancelada";
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}