using TransferenciaMateriais.Domain.Enums;

namespace TransferenciaMateriais.Domain.Entities;

/// <summary>
/// Evento de auditoria (append-only).
/// </summary>
public class AuditoriaEvento
{
    public Guid Id { get; set; }
    public AuditoriaEventType EventType { get; set; }
    public string CorrelationType { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string ActorRole { get; set; } = string.Empty;
    public string? ActorId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string PayloadJson { get; set; } = string.Empty;
}
