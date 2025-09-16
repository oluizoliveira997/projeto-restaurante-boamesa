namespace Boamesa.Domain.Entities;

public class AtendimentoDeliveryProprio : Atendimento
{
    public decimal TaxaFixa { get; set; }
    public override decimal CalcularTaxa(Pedido p) => TaxaFixa;
}