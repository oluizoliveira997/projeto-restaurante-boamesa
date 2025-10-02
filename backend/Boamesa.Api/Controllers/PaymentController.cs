using Microsoft.AspNetCore.Mvc;
using Boamesa.Application.DTOs;
using Boamesa.Application.Services;

namespace Boamesa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly PaymentService _payments;

    public PaymentsController(PaymentService payments)
    {
        _payments = payments;
    }

    /// <summary>
    /// Processa pagamento simulado e marca o pedido como "Pago" quando aprovado.
    /// </summary>
    [HttpPost("process")]
    public async Task<IActionResult> Process([FromBody] PaymentRequestDto dto, CancellationToken ct)
    {
        var res = await _payments.ProcessAsync(dto, ct);
        return Ok(res);
    }
}
