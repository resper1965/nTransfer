using TransferenciaMateriais.Domain.Enums;

namespace TransferenciaMateriais.Application.DTOs;

public class PendenciaDto
{
    public Guid Id { get; set; }
    public PendenciaTipo Tipo { get; set; }
    public PendenciaStatus Status { get; set; }
    public string CorrelationType { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? OwnerRole { get; set; }
    public DateTimeOffset? DueAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
}
