// Boamesa.Application/DTOs/ReservaDtos.cs
namespace Boamesa.Application.DTOs;

public record CriarReservaDto(int UsuarioId, int MesaId, DateTime DataHora);
public record ReservaVm(int Id, int UsuarioId, int MesaId, DateTime DataHora, string CodigoConfirmacao, string Status);