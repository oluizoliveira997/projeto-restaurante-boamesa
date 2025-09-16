// Boamesa.Domain/Entities/Usuario.cs
namespace Boamesa.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Email { get; set; } = default!;
    public string SenhaHash { get; set; } = default!;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public PerfilCliente PerfilCliente { get; set; } = default!;
    public List<EnderecoEntrega> Enderecos { get; set; } = new();
    public List<Pedido> Pedidos { get; set; } = new();
    public List<Reserva> Reservas { get; set; } = new();
}
