// Boamesa.Api/Controllers/SugestaoController.cs
using Boamesa.Application.DTOs;
using Boamesa.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Boamesa.Api.Controllers;

[ApiController]
[Route("api/sugestao")]
public class SugestaoController : ControllerBase
{
    private readonly SugestaoDoChefeService _svc;
    public SugestaoController(SugestaoDoChefeService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateOnly data)
        => Ok(await _svc.ObterPorDataAsync(data));

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CriarSugestaoDto dto)
    {
        try
        {
            var vm = await _svc.CriarAsync(dto);
            return CreatedAtAction(nameof(Get), new { data = vm.Data }, vm);
        }
        catch (BusinessRuleException ex)
        {
            return UnprocessableEntity(new { error = ex.Message }); // 422
        }
    }
}
