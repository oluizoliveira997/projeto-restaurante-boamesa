using Boamesa.Application.DTOs;
using Boamesa.Application.Services; // PasswordHasher
using Boamesa.Domain.Entities;
using Boamesa.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Boamesa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly BoamesaContext _db;
    public AuthController(BoamesaContext db) => _db = db;

    /// <summary>
    /// Login fake (retorna um token aleatório). 
    /// Usa hash SHA-256 no campo SenhaHash.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
            return BadRequest("Informe e-mail e senha.");

        var user = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Email == dto.Email, ct);
        if (user is null) return Unauthorized();

        var hash = PasswordHasher.Sha256(dto.Senha);
        if (!string.Equals(user.SenhaHash, hash, StringComparison.OrdinalIgnoreCase))
            return Unauthorized();

        var response = new LoginResponseDto
        {
            UserId = user.Id,
            Email  = user.Email,
            Token  = Guid.NewGuid().ToString("N") // token fake
        };

        return Ok(response);
    }

    /// <summary>
    /// Registro rápido (opcional) só para acelerar testes no MVP.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
            return BadRequest("Informe e-mail e senha.");

        var exists = await _db.Usuarios.AnyAsync(u => u.Email == dto.Email, ct);
        if (exists) return UnprocessableEntity("E-mail já cadastrado.");

        var u = new Usuario
        {
            Email     = dto.Email,
            SenhaHash = PasswordHasher.Sha256(dto.Senha),
            CriadoEm  = DateTime.UtcNow
        };
        _db.Usuarios.Add(u);
        await _db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(Me), new { id = u.Id }, new { u.Id, u.Email });
    }

    /// <summary>
    /// “Me” fake — não valida token; só retorna um usuário por id para demo.
    /// </summary>
    [HttpGet("me/{id:int}")]
    public async Task<IActionResult> Me(int id, CancellationToken ct)
    {
        var user = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);
        return user is null ? NotFound() : Ok(new { user.Id, user.Email, user.CriadoEm });
    }
}

