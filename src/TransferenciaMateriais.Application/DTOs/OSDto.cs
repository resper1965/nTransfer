using TransferenciaMateriais.Domain.Enums;

namespace TransferenciaMateriais.Application.DTOs;

public class OSDto
{
    public Guid Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public FluxType FluxType { get; set; }
    public string FilialDestinoId { get; set; } = string.Empty;
    public decimal QuantidadePlanejada { get; set; }
    public DateOnly? DataEstimadaEntrega { get; set; }
    public WorkflowStatus StatusWorkflow { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public class OSCreateRequest
{
    public string Numero { get; set; } = string.Empty;
    public FluxType FluxType { get; set; }
    public string FilialDestinoId { get; set; } = string.Empty;
    public decimal QuantidadePlanejada { get; set; }
    public DateOnly? DataEstimadaEntrega { get; set; }
}

public class OSUpdateRequest
{
    public DateOnly? DataEstimadaEntrega { get; set; }
}
