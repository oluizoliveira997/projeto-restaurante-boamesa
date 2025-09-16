// Boamesa.Domain/Entities/ItemCardapio.cs
namespace Boamesa.Domain.Entities;

using Boamesa.Domain.Enums;

public class ItemCardapio
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public string Descricao { get; set; } = default!;
    public decimal PrecoBase { get; set; }
    public Periodo Periodo { get; set; }
    public bool Ativo { get; set; } = true;

    public List<Ingrediente> Ingredientes { get; set; } = new();
    public List<PedidoItem> PedidoItens { get; set; } = new();
}
