// Ingrediente.cs
using Boamesa.Domain.Entities;

namespace Boamesa.Domain;
public class Ingrediente {
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public List<ItemCardapio> Itens { get; set; } = new();
}