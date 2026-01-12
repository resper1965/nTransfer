namespace TransferenciaMateriais.Domain;

/// <summary>
/// Serviço de envio de e-mail (DEC-02: notificações via e-mail).
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Envia e-mail de notificação.
    /// Deve registrar auditoria do envio (destinatário, status, correlação OS/OC/NFe).
    /// </summary>
    Task<EmailResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}

/// <summary>
/// Resultado do envio de e-mail.
/// </summary>
public record EmailResult(bool Success, string? MessageId = null, string? ErrorMessage = null);

/// <summary>
/// Mensagem de e-mail.
/// </summary>
public record EmailMessage(
    string To,
    string Subject,
    string Body,
    string? CorrelationId = null // OS/OC/NFe para auditoria
);
