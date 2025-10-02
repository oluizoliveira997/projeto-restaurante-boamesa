using Boamesa.Domain.Enums;

namespace Boamesa.Application.DTOs;

public class PedidoItemCreateDto
{
    public int ItemCardapioId { get; set; }
    public int Quantidade { get; set; } = 1;
    public decimal PrecoUnitario { get; set; } = 0;   // se 0, usa PrecoBase do cardápio
    public decimal DescontoAplicado { get; set; } = 0;
}

public class PedidoCreateDto
{
    public int UsuarioId { get; set; }
    public Periodo Periodo { get; set; }                // Almoco | Jantar
    public string TipoAtendimento { get; set; } = "Presencial"; // Presencial | DeliveryProprio | DeliveryAplicativo
      // ✅ necessário para DeliveryAplicativo
    public int? ParceiroAppId { get; set; }
    public List<PedidoItemCreateDto> Itens { get; set; } = new();
}

// ViewModel que o service retorna ao controller (tem Id!)
public class PedidoVm
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Periodo Periodo { get; set; }
    public string Status { get; set; } = "";
    public string AtendimentoTipo { get; set; } = "";
    public decimal TotalItens { get; set; }
    public decimal TotalGeral { get; set; }
    public DateTime DataHora { get; set; }
}
