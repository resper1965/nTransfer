using TransferenciaMateriais.Domain.Enums;

namespace TransferenciaMateriais.Domain.Entities;

/// <summary>
/// Nota Fiscal Eletrônica.
/// </summary>
public class NotaFiscal
{
    public string ChaveAcesso { get; set; } = string.Empty;
    public NfeTipo Tipo { get; set; }
    public string? Numero { get; set; }
    public string? Serie { get; set; }
    public string? CnpjEmitente { get; set; }
    public string? CnpjDestinatario { get; set; }
    public DateTimeOffset? DataEmissao { get; set; }
    public string? XmlRef { get; set; }
    public NfeValidacaoStatus ValidacaoStatus { get; set; } = NfeValidacaoStatus.PENDENTE;
    public NfeIncorretaMotivoCategoria? MotivoCategoria { get; set; }
    public string? MotivoDetalhe { get; set; }
    public DateTimeOffset? ReceivedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    // Navegação
    public ICollection<Vinculo> Vinculos { get; set; } = new List<Vinculo>();
}
