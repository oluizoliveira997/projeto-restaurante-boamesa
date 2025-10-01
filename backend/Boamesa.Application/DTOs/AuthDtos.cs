namespace Boamesa.Application.DTOs;

public class LoginRequestDto
{
    public string Email { get; set; } = default!;
    public string Senha { get; set; } = default!;
}

public class LoginResponseDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = default!;
    public string Token { get; set; } = default!;
}
