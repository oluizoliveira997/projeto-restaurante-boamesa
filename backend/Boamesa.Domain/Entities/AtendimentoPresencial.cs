// Boamesa.Domain/Entities/AtendimentoPresencial.cs
namespace Boamesa.Domain.Entities;

public class AtendimentoPresencial : Atendimento
{
    public override decimal CalcularTaxa(Pedido p) => 0m;
}