// Boamesa.Domain/Entities/Atendimento.cs
namespace Boamesa.Domain.Entities;

public abstract class Atendimento
{
    public int Id { get; set; }
    public abstract decimal CalcularTaxa(Pedido pedido);
}
