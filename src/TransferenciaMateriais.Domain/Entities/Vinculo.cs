using TransferenciaMateriais.Domain.Enums;

namespace TransferenciaMateriais.Domain.Entities;

/// <summary>
/// Vínculo entre OS, OC e NFe.
/// </summary>
public class Vinculo
{
    public Guid Id { get; set; }
    public Guid OsId { get; set; }
    public Guid? OcId { get; set; }
    public string NfeChaveAcesso { get; set; } = string.Empty;
    public VinculoStatus Status { get; set; } = VinculoStatus.PENDENTE;
    public decimal? DivergenciaQuantidade { get; set; }
    public string? Observacao { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    // Navegação
    public OrdemServico OrdemServico { get; set; } = null!;
    public OrdemCompra? OrdemCompra { get; set; }
    public NotaFiscal NotaFiscal { get; set; } = null!;
}
