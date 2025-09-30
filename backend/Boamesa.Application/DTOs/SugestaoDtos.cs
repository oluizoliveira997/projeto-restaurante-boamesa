// Boamesa.Application/DTOs/SugestaoDtos.cs
using Boamesa.Domain.Enums;

namespace Boamesa.Application.DTOs;

public record CriarSugestaoDto(DateOnly Data, Periodo Periodo, int ItemCardapioId, decimal DescontoPercentual = 0.20m);
public record SugestaoVm(int Id, DateOnly Data, Periodo Periodo, int ItemCardapioId, decimal DescontoPercentual);