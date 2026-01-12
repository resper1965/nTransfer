using TransferenciaMateriais.Domain.Enums;

namespace TransferenciaMateriais.Domain.Entities;

/// <summary>
/// Ordem de Serviço de Fabricação.
/// </summary>
public class OrdemServico
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

    // Navegação
    public ICollection<OrdemCompra> OrdensCompra { get; set; } = new List<OrdemCompra>();
    public ICollection<Vinculo> Vinculos { get; set; } = new List<Vinculo>();
}
