// Mesa, Reserva
using Boamesa.Domain.Entities;

namespace Boamesa.Domain;
public class Mesa {
    public int Id { get; set; }
    public int Numero { get; set; }
    public int Capacidade { get; set; }
    public List<Reserva> Reservas { get; set; } = new();
}