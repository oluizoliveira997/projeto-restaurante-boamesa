namespace Boamesa.Domain.Entities;

public class Reserva
{
    public int Id { get; set; }
    public DateTime DataHora { get; set; }
    public string? CodigoConfirmacao { get; set; }
    public string Status { get; set; } = "Ativa";

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = default!;

    public int MesaId { get; set; }
    public Mesa Mesa { get; set; } = default!;
}
