using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Boamesa.Application.DTOs;
using Boamesa.Application.Services;     // PedidoService, BusinessRuleException
using Boamesa.Infrastructure;

namespace Boamesa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly PedidoService _service;
    private readonly BoamesaContext _db;

    public PedidosController(PedidoService service, BoamesaContext db)
    {
        _service = service;
        _db = db;
    }

    /// <summary>
    /// Cria um pedido (MVP: itens devem ter mesmo período do pedido; tipos: Presencial/DeliveryProprio/DeliveryAplicativo).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PedidoCreateDto dto, CancellationToken ct)
    {
        var pedido = await _service.CriarPedidoAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = pedido.Id }, pedido);
    }

    /// <summary>
    /// Detalha um pedido por Id (com itens e atendimento).
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var pedido = await _service.ObterPedidoAsync(id);
        return pedido is null ? NotFound() : Ok(pedido);
    }

    /// <summary>
    /// Lista pedidos. Opcional: filtrar por usuário (?usuarioId=).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? usuarioId = null, CancellationToken ct = default)
    {
        var query = _db.Pedidos
            .AsNoTracking()
            .Include(p => p.Itens)
            .Include(p => p.Atendimento)
            .OrderByDescending(p => p.DataHora)
            .AsQueryable();

        if (usuarioId.HasValue) query = query.Where(p => p.UsuarioId == usuarioId.Value);

        var lista = await query.ToListAsync(ct);
        return Ok(lista);
    }

    /// <summary>
    /// Cancela um pedido se ainda não estiver pago (MVP).
    /// </summary>
    [HttpPost("{id:int}/cancelar")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        var pedido = await _db.Pedidos.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (pedido is null) return NotFound();

        if (string.Equals(pedido.Status, "Pago", StringComparison.OrdinalIgnoreCase))
            return UnprocessableEntity(new { error = "Pedido já pago não pode ser cancelado." });

        pedido.Status = "Cancelado";
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
