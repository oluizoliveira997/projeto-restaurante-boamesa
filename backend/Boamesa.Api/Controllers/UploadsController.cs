using Microsoft.AspNetCore.Mvc;

namespace Boamesa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadsController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    public UploadsController(IWebHostEnvironment env) => _env = env;

    // POST /api/uploads/item/{itemId}
    [HttpPost("item/{itemId:int}")]
    [RequestSizeLimit(10_000_000)] // 10 MB
    public async Task<IActionResult> UploadItemImage(int itemId, IFormFile file)
    {
        if (file is null || file.Length == 0) return BadRequest("Arquivo vazio.");
        // valida MIME simples
        var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
        if (!allowed.Contains(file.ContentType)) return UnprocessableEntity("Formato inválido.");

        // cria diretório
        var relDir = Path.Combine("uploads", "items");
        var absDir = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), relDir);
        Directory.CreateDirectory(absDir);

        // gera nome seguro
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var safeName = $"{Guid.NewGuid():N}{ext}";
        var absPath = Path.Combine(absDir, safeName);
        await using (var stream = System.IO.File.Create(absPath))
        {
            await file.CopyToAsync(stream);
        }

        var publicUrl = $"/{relDir.Replace("\\", "/")}/{safeName}";
        // você pode aqui atualizar o ItemCardapio.ImagemUrl, se quiser (via DbContext)

        return Ok(new { url = publicUrl });
    }
}
