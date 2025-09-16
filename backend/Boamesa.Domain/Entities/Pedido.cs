namespace Boamesa.Domain.Entities;

using Boamesa.Domain.Enums;

public class Pedido
{
    public int Id { get; set; }
    public DateTime DataHora { get; set; } = DateTime.UtcNow;
    public Periodo Periodo { get; set; }
    public string Status { get; set; } = "Criado";

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = default!;

    public int AtendimentoId { get; set; }
    public Atendimento Atendimento { get; set; } = default!;

    public List<PedidoItem> Itens { get; set; } = new();

    public decimal TotalItens() => Itens.Sum(i => i.Subtotal());
    public decimal TotalDescontos() => Itens.Sum(i => i.DescontoAplicado);
    public decimal TotalTaxas() => Atendimento?.CalcularTaxa(this) ?? 0m;
    public decimal TotalGeral() => TotalItens() - TotalDescontos() + TotalTaxas();
}
