// SugestaoDoChefe.cs
using Boamesa.Domain.Enums;

namespace Boamesa.Domain.Entities;
public class SugestaoDoChefe {
    public int Id { get; set; }
    public DateOnly Data { get; set; }
    public Periodo Periodo { get; set; }
    public decimal DescontoPercentual { get; set; } = 0.20m;
    public int ItemCardapioId { get; set; }
    public ItemCardapio ItemCardapio { get; set; } = default!;
}