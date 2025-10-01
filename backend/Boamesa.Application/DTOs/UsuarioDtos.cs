namespace Boamesa.Application.DTOs;

public class UsuarioCreateDto
{
    public string Email { get; set; } = default!;
    public string Senha { get; set; } = default!;
}

public class UsuarioUpdateDto
{
    public int Id { get; set; }
    public string Email { get; set; } = default!;
    public string? NovaSenha { get; set; }
}