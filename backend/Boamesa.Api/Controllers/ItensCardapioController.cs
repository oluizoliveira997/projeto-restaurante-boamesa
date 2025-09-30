using Boamesa.Infrastructure;
using Boamesa.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/itens")]
public class ItensCardapioController : ControllerBase
{
    private readonly BoamesaContext _db;
    public ItensCardapioController(BoamesaContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Periodo? periodo)
    {
        var q = _db.ItensCardapio.AsQueryable();
        if (periodo.HasValue) q = q.Where(i => i.Periodo == periodo);
        var data = await q.AsNoTracking().ToListAsync();
        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ItemCardapio item)
    {
        _db.ItensCardapio.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _db.ItensCardapio.FindAsync(id);
        return item is null ? NotFound() : Ok(item);
    }
}