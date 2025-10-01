using Boamesa.Infrastructure;
using Boamesa.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Boamesa.Domain.Enums;
using Boamesa.Domain.Entities;

namespace Boamesa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItensCardapioController : ControllerBase
{
    private readonly BoamesaContext _db;
    public ItensCardapioController(BoamesaContext db) => _db = db;

    // GET api/itens?periodo=Almoco|Jantar (opcional)
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Periodo? periodo = null)
    {
        var query = _db.ItensCardapio.AsNoTracking().Where(i => i.Ativo);
        if (periodo.HasValue)
            query = query.Where(i => i.Periodo == periodo.Value);

        var itens = await query.ToListAsync();
        return Ok(itens);
    }

    // GET api/itens/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _db.ItensCardapio.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        return item is null ? NotFound() : Ok(item);
    }

    // POST api/itens
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ItemCardapio dto)
    {
        // Defaults seguros (MVP)
        dto.Ativo = true;
        _db.ItensCardapio.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    // PUT api/itens/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ItemCardapio dto)
    {
        var item = await _db.ItensCardapio.FirstOrDefaultAsync(i => i.Id == id);
        if (item is null) return NotFound();

        item.Nome = dto.Nome;
        item.Descricao = dto.Descricao;
        item.PrecoBase = dto.PrecoBase;
        item.Periodo = dto.Periodo;
        item.Ativo = dto.Ativo;
        item.ImagemUrl = dto.ImagemUrl;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/itens/5  (soft delete simples: Ativo = false)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.ItensCardapio.FirstOrDefaultAsync(i => i.Id == id);
        if (item is null) return NotFound();

        item.Ativo = false;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // PUT api/itens/5/imagem  (vincula URL retornada pelo UploadsController)
    [HttpPut("{id:int}/imagem")]
    public async Task<IActionResult> SetImage(int id, [FromBody] string imagemUrl)
    {
        var item = await _db.ItensCardapio.FirstOrDefaultAsync(i => i.Id == id);
        if (item is null) return NotFound();

        item.ImagemUrl = imagemUrl;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}