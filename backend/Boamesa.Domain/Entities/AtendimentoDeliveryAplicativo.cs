namespace Boamesa.Domain.Entities;

public class AtendimentoDeliveryAplicativo : Atendimento
{
    public decimal ComissaoPercentual { get; set; }
    public decimal? TaxaFixaParceiro { get; set; }

    // ✅ agora opcionais no TPH
    public int? ParceiroAppId { get; set; }
    public ParceiroApp? ParceiroApp { get; set; }

    public override decimal CalcularTaxa(Pedido p)
    {
        var baseCalculo = p.Itens.Sum(i => i.Subtotal());
        return (baseCalculo * ComissaoPercentual) + (TaxaFixaParceiro ?? 0m);
    }
}
