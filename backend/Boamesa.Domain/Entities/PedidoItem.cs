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

    public decimal Subtotal() => Quantidade * PrecoUnitario;
}
