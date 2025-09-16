// PerfilCliente.cs
using Boamesa.Domain.Entities;

namespace Boamesa.Domain;
public class PerfilCliente {
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public string Telefone { get; set; } = default!;
    public string? Documento { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = default!;
}