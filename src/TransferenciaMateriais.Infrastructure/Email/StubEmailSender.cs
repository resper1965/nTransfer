using TransferenciaMateriais.Domain;

namespace TransferenciaMateriais.Infrastructure.Email;

/// <summary>
/// Stub do serviço de e-mail para desenvolvimento local.
/// Em produção, usar SMTP ou Mailpit (via docker-compose).
/// </summary>
public class StubEmailSender : IEmailSender
{
    private readonly ILogger<StubEmailSender> _logger;

    public StubEmailSender(ILogger<StubEmailSender> logger)
    {
        _logger = logger;
    }

    public Task<EmailResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "STUB EMAIL: To={To}, Subject={Subject}, CorrelationId={CorrelationId}",
            message.To,
            message.Subject,
            message.CorrelationId
        );

        // Stub sempre retorna sucesso
        // Em produção, aqui haveria envio real e registro de auditoria
        return Task.FromResult(new EmailResult(Success: true, MessageId: Guid.NewGuid().ToString()));
    }
}
