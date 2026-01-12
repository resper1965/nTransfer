using TransferenciaMateriais.Domain.Enums;

namespace TransferenciaMateriais.Application.DTOs;

public class NotificacaoDto
{
    public Guid Id { get; set; }
    public NotificacaoTipo Tipo { get; set; }
    public NotificacaoStatus Status { get; set; }
    public string CorrelationType { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string DestinatariosTo { get; set; } = string.Empty;
    public string? DestinatariosCc { get; set; }
    public string Assunto { get; set; } = string.Empty;
    public string? ProviderMessageId { get; set; }
    public string? Erro { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? SentAt { get; set; }
}
