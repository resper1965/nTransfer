namespace TransferenciaMateriais.Domain.Entities;

/// <summary>
/// Ordem de Compra.
/// </summary>
public class OrdemCompra
{
    public Guid Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public Guid OsId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    // Navegação
    public OrdemServico OrdemServico { get; set; } = null!;
    public ICollection<Vinculo> Vinculos { get; set; } = new List<Vinculo>();
}
