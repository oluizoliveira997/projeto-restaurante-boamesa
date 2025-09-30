// Boamesa.Api/Controllers/ReservaController.cs
using Boamesa.Application.DTOs;
using Boamesa.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Boamesa.Api.Controllers;

[ApiController]
[Route("api/reservas")]
public class ReservaController : ControllerBase
{
    private readonly ReservaService _svc;
    public ReservaController(ReservaService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateOnly data)
        => Ok(await _svc.ListarPorDataAsync(data));

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CriarReservaDto dto)
    {
        try
        {
            var vm = await _svc.CriarAsync(dto);
            return CreatedAtAction(nameof(Get), new { data = DateOnly.FromDateTime(vm.DataHora) }, vm);
        }
        catch (BusinessRuleException ex)
        {
            return UnprocessableEntity(new { error = ex.Message });
        }
    }
}
