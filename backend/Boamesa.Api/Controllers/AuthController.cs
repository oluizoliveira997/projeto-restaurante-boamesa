using Boamesa.Application.DTOs;
using Boamesa.Application.Services; // PasswordHasher (se usar hash)
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
    /// Login simples de demonstração (não use em produção).
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
            return BadRequest("Informe e-mail e senha.");

        var user = await _db.Usuarios.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null)
            return Unauthorized(); // 401

        // ====== Versão usando HASH (recomendada para demo) ======
        var hash = PasswordHasher.Sha256(dto.Senha);
        if (!string.Equals(user.SenhaHash, hash, StringComparison.OrdinalIgnoreCase))
            return Unauthorized();

        // ====== Se quiser senha em texto puro, use ISSO e remova o bloco acima ======
        // if (user.SenhaHash != dto.Senha)
        //     return Unauthorized();

        var response = new LoginResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Token = Guid.NewGuid().ToString("N") // token "fake" para o front
        };

        return Ok(response);
    }
}
