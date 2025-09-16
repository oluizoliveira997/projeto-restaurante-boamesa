namespace Boamesa.Domain.Entities;

public class AtendimentoDeliveryAplicativo : Atendimento
{
    public decimal ComissaoPercentual { get; set; }
    public decimal? TaxaFixaParceiro { get; set; }
    public int ParceiroAppId { get; set; }
    public ParceiroApp ParceiroApp { get; set; } = default!;
    public override decimal CalcularTaxa(Pedido p) =>
        (p.TotalItens() * ComissaoPercentual) + (TaxaFixaParceiro ?? 0m);
}