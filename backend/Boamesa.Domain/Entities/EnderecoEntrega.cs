// Boamesa.Domain/Entities/EnderecoEntrega.cs
namespace Boamesa.Domain.Entities;

public class EnderecoEntrega
{
    public int Id { get; set; }
    public string Logradouro { get; set; } = default!;
    public string Numero { get; set; } = default!;
    public string Bairro { get; set; } = default!;
    public string Cidade { get; set; } = default!;
    public string Uf { get; set; } = default!;
    public string Cep { get; set; } = default!;
    public string? Complemento { get; set; }

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = default!;
}
