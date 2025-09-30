// Boamesa.Api/Controllers/PedidosController.cs
using Boamesa.Application.DTOs;
using Boamesa.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Boamesa.Api.Controllers;

[ApiController]
[Route("api/pedidos")]
public class PedidosController : ControllerBase
{
    private readonly PedidoService _svc;
    public PedidosController(PedidoService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CriarPedidoDto dto)
    {
        try
        {
            var vm = await _svc.CriarAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = vm.Id }, vm);
        }
        catch (BusinessRuleException ex)
        {
            return UnprocessableEntity(new { error = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var vm = await _svc.ObterAsync(id);
        return vm is null ? NotFound() : Ok(vm);
    }
}
