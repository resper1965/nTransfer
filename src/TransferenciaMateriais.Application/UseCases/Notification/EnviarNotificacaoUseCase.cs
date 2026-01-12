using TransferenciaMateriais.Application.Services;
using TransferenciaMateriais.Domain;
using TransferenciaMateriais.Domain.Entities;
using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TransferenciaMateriais.Application.UseCases.Notification;

/// <summary>
/// Use Case para enviar notificações por e-mail.
/// Registra notificação, resolve destinatários, renderiza template e envia e-mail.
/// </summary>
public class EnviarNotificacaoUseCase
{
    private readonly ApplicationDbContext _context;
    private readonly NotificationService _notificationService;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<EnviarNotificacaoUseCase> _logger;

    public EnviarNotificacaoUseCase(
        ApplicationDbContext context,
        NotificationService notificationService,
        IEmailSender emailSender,
        ILogger<EnviarNotificacaoUseCase> logger)
    {
        _context = context;
        _notificationService = notificationService;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<Notificacao> ExecuteAsync(
        NotificacaoTipo tipo,
        string correlationType,
        string correlationId,
        string? filialDestinoId = null,
        string? ownerRole = null,
        Dictionary<string, string>? additionalVariables = null)
    {
        // 1. Verificar deduplicação (janela de 10 minutos para eventos transacionais, 24h para lembretes)
        var dedupeWindow = tipo is NotificacaoTipo.LEMBRETE_7_DIAS_ENTREGA_ESTIMADA or NotificacaoTipo.LEMBRETE_30_DIAS_DESTINO
            ? TimeSpan.FromHours(24)
            : TimeSpan.FromMinutes(10);

        var dedupeKey = $"{tipo}:{correlationType}:{correlationId}";
        var since = DateTimeOffset.UtcNow - dedupeWindow;

        var duplicate = await _context.Notificacoes
            .AnyAsync(n =>
                n.Tipo == tipo &&
                n.CorrelationType == correlationType &&
                n.CorrelationId == correlationId &&
                n.CreatedAt >= since);

        if (duplicate)
        {
            _logger.LogInformation(
                "Notificação duplicada ignorada: {Tipo}, {CorrelationType}, {CorrelationId}",
                tipo, correlationType, correlationId
            );
            throw new InvalidOperationException("Notificação duplicada na janela de deduplicação.");
        }

        // 2. Resolver destinatários
        var recipients = _notificationService.ResolveRecipients(tipo, filialDestinoId, ownerRole);

        if (recipients.To.Count == 0)
        {
            _logger.LogWarning(
                "Nenhum destinatário encontrado para notificação: {Tipo}, {FilialDestinoId}, {OwnerRole}",
                tipo, filialDestinoId, ownerRole
            );
            throw new InvalidOperationException("Nenhum destinatário encontrado para a notificação.");
        }

        // 3. Preparar variáveis do template
        var variables = new Dictionary<string, string>
        {
            { "correlationId", $"{correlationType}:{correlationId}" },
            { "processoUrl", $"https://app.example.local/paineis/processo?{correlationType.ToLower()}={correlationId}" }
        };

        // Buscar dados adicionais se necessário
        if (correlationType == "OS")
        {
            var os = await _context.OrdensServico.FindAsync(Guid.Parse(correlationId));
            if (os != null)
            {
                variables["osNumero"] = os.Numero;
                variables["osId"] = os.Id.ToString();
                variables["filialDestinoId"] = os.FilialDestinoId;
                variables["statusWorkflow"] = os.StatusWorkflow.ToString();
                if (os.DataEstimadaEntrega.HasValue)
                {
                    variables["dataEstimadaEntrega"] = os.DataEstimadaEntrega.Value.ToString("yyyy-MM-dd");
                }
            }
        }

        if (additionalVariables != null)
        {
            foreach (var (key, value) in additionalVariables)
            {
                variables[key] = value;
            }
        }

        // 4. Renderizar template
        var template = _notificationService.RenderTemplate(tipo, variables);

        // 5. Criar registro de notificação (ENFILEIRADA)
        var notificacao = new Notificacao
        {
            Id = Guid.NewGuid(),
            Tipo = tipo,
            CorrelationType = correlationType,
            CorrelationId = correlationId,
            Assunto = template.Subject,
            Corpo = template.Body,
            DestinatariosTo = string.Join(",", recipients.To),
            DestinatariosCc = recipients.Cc.Count > 0 ? string.Join(",", recipients.Cc) : null,
            Status = NotificacaoStatus.ENFILEIRADA,
            CreatedAt = DateTimeOffset.UtcNow
        };
        _context.Notificacoes.Add(notificacao);

        // 6. Registrar auditoria (NOTIFICACAO_ENFILEIRADA)
        var auditoriaEvento = new AuditoriaEvento
        {
            Id = Guid.NewGuid(),
            EventType = AuditoriaEventType.NOTIFICACAO_ENFILEIRADA,
            CorrelationType = correlationType,
            CorrelationId = correlationId,
            ActorRole = "SISTEMA",
            ActorId = "NOTIFICATION_SERVICE",
            Timestamp = DateTimeOffset.UtcNow,
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                notificacaoId = notificacao.Id,
                tipo = tipo.ToString(),
                to = recipients.To,
                cc = recipients.Cc,
                subject = template.Subject
            })
        };
        _context.AuditoriaEventos.Add(auditoriaEvento);

        await _context.SaveChangesAsync();

        // 7. Enviar e-mail (sync no MVP)
        try
        {
            var emailMessage = new EmailMessage(
                To: string.Join(",", recipients.To),
                Subject: template.Subject,
                Body: template.Body,
                CorrelationId: $"{correlationType}:{correlationId}"
            );

            var result = await _emailSender.SendAsync(emailMessage);

            if (result.Success)
            {
                // Atualizar status para ENVIADA
                notificacao.Status = NotificacaoStatus.ENVIADA;
                notificacao.ProviderMessageId = result.MessageId;
                notificacao.SentAt = DateTimeOffset.UtcNow;

                // Registrar auditoria (NOTIFICACAO_ENVIADA)
                var enviadaEvento = new AuditoriaEvento
                {
                    Id = Guid.NewGuid(),
                    EventType = AuditoriaEventType.NOTIFICACAO_ENVIADA,
                    CorrelationType = correlationType,
                    CorrelationId = correlationId,
                    ActorRole = "SISTEMA",
                    ActorId = "NOTIFICATION_SERVICE",
                    Timestamp = DateTimeOffset.UtcNow,
                    PayloadJson = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        notificacaoId = notificacao.Id,
                        providerMessageId = result.MessageId,
                        sentAt = notificacao.SentAt
                    })
                };
                _context.AuditoriaEventos.Add(enviadaEvento);
            }
            else
            {
                // Atualizar status para FALHOU
                notificacao.Status = NotificacaoStatus.FALHOU;
                notificacao.Erro = result.ErrorMessage;
                notificacao.SentAt = DateTimeOffset.UtcNow;

                // Registrar auditoria (NOTIFICACAO_FALHOU)
                var falhouEvento = new AuditoriaEvento
                {
                    Id = Guid.NewGuid(),
                    EventType = AuditoriaEventType.NOTIFICACAO_FALHOU,
                    CorrelationType = correlationType,
                    CorrelationId = correlationId,
                    ActorRole = "SISTEMA",
                    ActorId = "NOTIFICATION_SERVICE",
                    Timestamp = DateTimeOffset.UtcNow,
                    PayloadJson = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        notificacaoId = notificacao.Id,
                        erro = result.ErrorMessage,
                        sentAt = notificacao.SentAt
                    })
                };
                _context.AuditoriaEventos.Add(falhouEvento);
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao enviar notificação: {NotificacaoId}, {Tipo}",
                notificacao.Id, tipo
            );

            notificacao.Status = NotificacaoStatus.FALHOU;
            notificacao.Erro = ex.Message;
            await _context.SaveChangesAsync();

            throw;
        }

        return notificacao;
    }
}
