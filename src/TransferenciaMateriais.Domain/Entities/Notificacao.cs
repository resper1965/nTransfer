using TransferenciaMateriais.Domain.Enums;

namespace TransferenciaMateriais.Domain.Entities;

/// <summary>
/// Notificação por e-mail.
/// </summary>
public class Notificacao
{
    public Guid Id { get; set; }
    public NotificacaoTipo Tipo { get; set; }
    public NotificacaoStatus Status { get; set; } = NotificacaoStatus.ENFILEIRADA;
    public string CorrelationType { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string DestinatariosTo { get; set; } = string.Empty;
    public string? DestinatariosCc { get; set; }
    public string Assunto { get; set; } = string.Empty;
    public string Corpo { get; set; } = string.Empty;
    public string? ProviderMessageId { get; set; }
    public string? Erro { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? SentAt { get; set; }
}
