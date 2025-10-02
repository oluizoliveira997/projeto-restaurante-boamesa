// Boamesa.Domain/Entities/PedidoItem.cs
namespace Boamesa.Domain.Entities;

public class PedidoItem
{
    public int Id { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal DescontoAplicado { get; set; }

    public int PedidoId { get; set; }
    public Pedido Pedido { get; set; } = default!;

    public int ItemCardapioId { get; set; }
    public ItemCardapio ItemCardapio { get; set; } = default!;

    // Bruto e Desconto explicitados (opcional, mas útil p/ clareza)
    public decimal SubtotalBruto() => Quantidade * PrecoUnitario;
    public decimal DescontoTotal() => Quantidade * DescontoAplicado;

    // Líquido = (Preço - Desconto) * Quantidade
    public decimal Subtotal() => (PrecoUnitario - DescontoAplicado) * Quantidade;
}
