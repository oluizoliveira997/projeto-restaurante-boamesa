using Boamesa.Application.DTOs;
using Boamesa.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Boamesa.Application.Services;

public class PaymentService
{
    private readonly BoamesaContext _db;
    public PaymentService(BoamesaContext db) => _db = db;

    public async Task<PaymentResponseDto> ProcessAsync(PaymentRequestDto dto, CancellationToken ct = default)
    {
        var pedido = await _db.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == dto.PedidoId, ct);

        if (pedido is null)
            throw new BusinessRuleException("Pedido não encontrado.");

        if (pedido.Status == "Pago")
            throw new BusinessRuleException("Pedido já está pago.");

        // MVP: regra simples de aprovação
        if (dto.Valor <= 0)
            return new PaymentResponseDto
            {
                Status = "Recusado",
                TransacaoId = Guid.NewGuid().ToString("N"),
                AutorizadoEm = DateTime.UtcNow
            };

        // (opcional) validar total calculado do pedido aqui
        pedido.Status = "Pago";
        await _db.SaveChangesAsync(ct);

        return new PaymentResponseDto
        {
            Status = "Aprovado",
            TransacaoId = Guid.NewGuid().ToString("N"),
            AutorizadoEm = DateTime.UtcNow
        };
    }
}
