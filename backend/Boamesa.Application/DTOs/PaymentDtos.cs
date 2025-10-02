namespace Boamesa.Application.DTOs;

public class PaymentRequestDto
{
    public int PedidoId { get; set; }
    public string Metodo { get; set; } = "pix";   // "pix" | "cartao"
    public decimal Valor { get; set; }            // valor a pagar
}

public class PaymentResponseDto
{
    public string Status { get; set; } = default!;     // "Aprovado" | "Recusado"
    public string TransacaoId { get; set; } = default!;
    public DateTime AutorizadoEm { get; set; }
}