namespace TransferenciaMateriais.Domain.Entities;

/// <summary>
/// Mensagem processada (idempotência de integrações).
/// </summary>
public class ProcessedMessage
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public DateTimeOffset ReceivedAt { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
    public string Result { get; set; } = string.Empty;
    public string? Error { get; set; }
}
