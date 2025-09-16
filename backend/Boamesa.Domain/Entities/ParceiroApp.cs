// ParceiroApp.cs
using Boamesa.Domain.Entities;

namespace Boamesa.Domain;
public class ParceiroApp {
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public List<AtendimentoDeliveryAplicativo> Atendimentos { get; set; } = new();
}