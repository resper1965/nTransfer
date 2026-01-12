using TransferenciaMateriais.Domain;
using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace TransferenciaMateriais.Application.Services;

/// <summary>
/// Serviço de notificações por e-mail.
/// Resolve destinatários, renderiza templates e envia notificações.
/// </summary>
public class NotificationService
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<NotificationService> _logger;
    private readonly Dictionary<string, Dictionary<string, List<string>>> _recipientsConfig;

    public NotificationService(
        IEmailSender emailSender,
        ILogger<NotificationService> logger)
    {
        _emailSender = emailSender;
        _logger = logger;
        
        // Configuração de destinatários (MVP - pode ser movido para banco/config)
        _recipientsConfig = new Dictionary<string, Dictionary<string, List<string>>>
        {
            {
                "FIL-DEST-01",
                new Dictionary<string, List<string>>
                {
                    { "ADM_FILIAL_DESTINO", new List<string> { "adm.destino@example.local" } },
                    { "GESTOR_CONTRATO", new List<string> { "gestor@example.local" } }
                }
            },
            {
                "GLOBAL",
                new Dictionary<string, List<string>>
                {
                    { "GESTOR_CONTRATO", new List<string> { "gestor@example.local" } },
                    { "ADM_FILIAL_ORIGEM", new List<string> { "adm.origem@example.local" } }
                }
            }
        };
    }

    /// <summary>
    /// Resolve destinatários por papel e filial.
    /// </summary>
    public RecipientsResult ResolveRecipients(
        NotificacaoTipo tipo,
        string? filialDestinoId = null,
        string? ownerRole = null)
    {
        var to = new List<string>();
        var cc = new List<string>();

        switch (tipo)
        {
            case NotificacaoTipo.CHEGADA_MATERIAL:
            case NotificacaoTipo.NFE_ENTRADA_DISPONIVEL:
                if (!string.IsNullOrWhiteSpace(filialDestinoId))
                {
                    to.AddRange(GetEmailsForRole(filialDestinoId, "ADM_FILIAL_DESTINO"));
                }
                break;

            case NotificacaoTipo.NFE_SAIDA_PRONTA_IMPRESSAO:
                to.AddRange(GetEmailsForRole("GLOBAL", "ADM_FILIAL_ORIGEM"));
                break;

            case NotificacaoTipo.CANCELAMENTO_PROCESSO:
                to.AddRange(GetEmailsForRole("GLOBAL", "GESTOR_CONTRATO"));
                if (!string.IsNullOrWhiteSpace(filialDestinoId))
                {
                    cc.AddRange(GetEmailsForRole(filialDestinoId, "ADM_FILIAL_DESTINO"));
                }
                break;

            case NotificacaoTipo.PENDENCIA_ABERTA:
                if (!string.IsNullOrWhiteSpace(ownerRole))
                {
                    to.AddRange(GetEmailsForRole(filialDestinoId ?? "GLOBAL", ownerRole));
                }
                cc.AddRange(GetEmailsForRole("GLOBAL", "GESTOR_CONTRATO"));
                break;

            case NotificacaoTipo.LEMBRETE_7_DIAS_ENTREGA_ESTIMADA:
            case NotificacaoTipo.LEMBRETE_30_DIAS_DESTINO:
            case NotificacaoTipo.MEDICAO_CONCLUIDA:
                to.AddRange(GetEmailsForRole("GLOBAL", "GESTOR_CONTRATO"));
                if (!string.IsNullOrWhiteSpace(filialDestinoId))
                {
                    cc.AddRange(GetEmailsForRole(filialDestinoId, "ADM_FILIAL_DESTINO"));
                }
                break;
        }

        // Deduplicar e-mails
        to = to.Distinct().ToList();
        cc = cc.Where(e => !to.Contains(e)).Distinct().ToList();

        return new RecipientsResult(to, cc);
    }

    private List<string> GetEmailsForRole(string? filialId, string role)
    {
        var emails = new List<string>();
        var configKey = filialId ?? "GLOBAL";

        if (_recipientsConfig.TryGetValue(configKey, out var roles) &&
            roles.TryGetValue(role, out var roleEmails))
        {
            emails.AddRange(roleEmails);
        }
        else if (_recipientsConfig.TryGetValue("GLOBAL", out var globalRoles) &&
                 globalRoles.TryGetValue(role, out var globalEmails))
        {
            emails.AddRange(globalEmails);
        }

        return emails;
    }

    /// <summary>
    /// Renderiza template de e-mail substituindo variáveis.
    /// </summary>
    public EmailTemplateResult RenderTemplate(
        NotificacaoTipo tipo,
        Dictionary<string, string> variables)
    {
        var (subject, body) = GetTemplate(tipo);

        // Substituir variáveis no formato {{variavel}}
        foreach (var (key, value) in variables)
        {
            var placeholder = $"{{{{{key}}}}}";
            subject = subject.Replace(placeholder, value);
            body = body.Replace(placeholder, value);
        }

        return new EmailTemplateResult(subject, body);
    }

    private (string subject, string body) GetTemplate(NotificacaoTipo tipo)
    {
        return tipo switch
        {
            NotificacaoTipo.CHEGADA_MATERIAL => (
                "Chegada de material — OS {{osNumero}}",
                @"Olá,
Registramos a chegada do material referente à OS {{osNumero}}.

- Filial destino: {{filialDestinoId}}
- Status atual: {{statusWorkflow}}
- Correlation ID: {{correlationId}}

Acesse o processo: {{processoUrl}}"
            ),
            NotificacaoTipo.NFE_ENTRADA_DISPONIVEL => (
                "NFe de entrada disponível — OS {{osNumero}}",
                @"Olá,
A NFe de entrada está disponível para o processo OS {{osNumero}}.

- Chave NFe: {{nfeChaveAcesso}}
- Status atual: {{statusWorkflow}}
- Correlation ID: {{correlationId}}

Acesse o processo: {{processoUrl}}"
            ),
            NotificacaoTipo.NFE_SAIDA_PRONTA_IMPRESSAO => (
                "NFe de saída pronta para impressão — OS {{osNumero}}",
                @"Olá,
A NFe de saída está pronta para impressão no processo OS {{osNumero}}.

- Chave NFe: {{nfeChaveAcesso}}
- Correlation ID: {{correlationId}}

Acesse o processo: {{processoUrl}}"
            ),
            NotificacaoTipo.PENDENCIA_ABERTA => (
                "Pendência aberta ({{pendenciaTipo}}) — OS {{osNumero}}",
                @"Olá,
Foi aberta uma pendência no processo OS {{osNumero}}.

- Tipo: {{pendenciaTipo}}
- Descrição: {{pendenciaDescricao}}
- Status atual: {{statusWorkflow}}
- Correlation ID: {{correlationId}}

Acesse o processo: {{processoUrl}}"
            ),
            NotificacaoTipo.CANCELAMENTO_PROCESSO => (
                "Processo cancelado — OS {{osNumero}}",
                @"Olá,
O processo OS {{osNumero}} foi cancelado.

- Motivo: {{pendenciaDescricao}}
- Correlation ID: {{correlationId}}

Acesse o processo: {{processoUrl}}"
            ),
            NotificacaoTipo.LEMBRETE_7_DIAS_ENTREGA_ESTIMADA => (
                "Lembrete: 7 dias para entrega estimada — OS {{osNumero}}",
                @"Olá,
Faltam 7 dias para a data estimada de entrega do processo OS {{osNumero}}.

- Data estimada: {{dataEstimadaEntrega}}
- Status atual: {{statusWorkflow}}
- Correlation ID: {{correlationId}}

Acesse o processo: {{processoUrl}}"
            ),
            NotificacaoTipo.LEMBRETE_30_DIAS_DESTINO => (
                "Alerta: 30 dias com ação pendente no destino — OS {{osNumero}}",
                @"Olá,
Há 30 dias existe ação pendente para a filial destino no processo OS {{osNumero}}.

- Filial destino: {{filialDestinoId}}
- Status atual: {{statusWorkflow}}
- Correlation ID: {{correlationId}}

Acesse o processo: {{processoUrl}}"
            ),
            NotificacaoTipo.MEDICAO_CONCLUIDA => (
                "Medição concluída — OS {{osNumero}}",
                @"Olá,
A medição do processo OS {{osNumero}} foi concluída.

- Status atual: {{statusWorkflow}}
- Correlation ID: {{correlationId}}

Acesse o processo: {{processoUrl}}"
            ),
            _ => throw new ArgumentException($"Tipo de notificação não suportado: {tipo}")
        };
    }
}

public record RecipientsResult(List<string> To, List<string> Cc);

public record EmailTemplateResult(string Subject, string Body);
