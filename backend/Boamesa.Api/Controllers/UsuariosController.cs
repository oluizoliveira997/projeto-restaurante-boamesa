using Boamesa.Application.DTOs;
using Boamesa.Domain.Entities;
using Boamesa.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Boamesa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly BoamesaContext _db;

        public UsuariosController(BoamesaContext db)
        {
            _db = db;
        }

        // GET api/usuarios
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _db.Usuarios.AsNoTracking().ToListAsync();
            return Ok(usuarios);
        }

        // GET api/usuarios/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _db.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();
            return Ok(usuario);
        }

        // POST api/usuarios
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Usuario dto)
        {
            dto.CriadoEm = DateTime.UtcNow; // garante data
            _db.Usuarios.Add(dto);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        // PUT api/usuarios/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Usuario dto)
        {
            var usuario = await _db.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            // atualiza os campos desejados
            usuario.Email = dto.Email;
            usuario.SenhaHash = dto.SenhaHash;

            await _db.SaveChangesAsync();
            return NoContent(); // ou Ok(usuario) se quiser retornar
        }

        // DELETE api/usuarios/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _db.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            _db.Usuarios.Remove(usuario);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}