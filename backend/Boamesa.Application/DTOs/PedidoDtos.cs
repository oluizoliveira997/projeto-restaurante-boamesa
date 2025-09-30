// Boamesa.Application/DTOs/PedidoDtos.cs
using Boamesa.Domain.Enums;

namespace Boamesa.Application.DTOs;

public record PedidoItemCreateDto(int ItemCardapioId, int Quantidade, decimal PrecoUnitario, decimal DescontoAplicado);
public record CriarPedidoDto(
    int UsuarioId,
    Periodo Periodo,
    string TipoAtendimento,         // "Presencial" | "DeliveryProprio" | "DeliveryApp"
    int? ParceiroAppId,
    decimal? TaxaFixa,              // usado no DeliveryProprio
    decimal? ComissaoPercentual,    // usado no DeliveryApp (ex.: 0.20 = 20%)
    decimal? TaxaFixaParceiro,      // usado no DeliveryApp
    List<PedidoItemCreateDto> Itens
);
public record PedidoVm(int Id, Periodo Periodo, string Status, decimal TotalItens, decimal TotalDescontos, decimal TotalTaxas, decimal TotalGeral);