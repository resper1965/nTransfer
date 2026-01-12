using TransferenciaMateriais.Domain.Entities;
using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace TransferenciaMateriais.Application.Services;

/// <summary>
/// Serviço de auditoria para registrar eventos de transições e decisões.
/// </summary>
public class AuditoriaService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuditoriaService> _logger;

    public AuditoriaService(
        ApplicationDbContext context,
        ILogger<AuditoriaService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Registra evento de transição de workflow.
    /// </summary>
    public async Task RegistrarTransicaoWorkflowAsync(
        string correlationType,
        string correlationId,
        WorkflowStatus fromStatus,
        WorkflowStatus toStatus,
        string actorRole,
        string? actorId = null,
        string? reason = null,
        Dictionary<string, object>? additionalPayload = null)
    {
        var payload = new Dictionary<string, object>
        {
            { "fromStatus", fromStatus.ToString() },
            { "toStatus", toStatus.ToString() }
        };

        if (!string.IsNullOrWhiteSpace(reason))
        {
            payload["reason"] = reason;
        }

        if (additionalPayload != null)
        {
            foreach (var (key, value) in additionalPayload)
            {
                payload[key] = value;
            }
        }

        var evento = new AuditoriaEvento
        {
            Id = Guid.NewGuid(),
            EventType = AuditoriaEventType.WORKFLOW_TRANSICAO,
            CorrelationType = correlationType,
            CorrelationId = correlationId,
            ActorRole = actorRole,
            ActorId = actorId,
            Timestamp = DateTimeOffset.UtcNow,
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(payload)
        };

        _context.AuditoriaEventos.Add(evento);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Transição de workflow registrada: {CorrelationType}:{CorrelationId}, {FromStatus} -> {ToStatus}",
            correlationType, correlationId, fromStatus, toStatus
        );
    }

    /// <summary>
    /// Registra evento de criação de OS.
    /// </summary>
    public async Task RegistrarOSCriadaAsync(OrdemServico os, string actorRole, string? actorId = null)
    {
        var payload = new Dictionary<string, object>
        {
            { "osId", os.Id.ToString() },
            { "osNumero", os.Numero },
            { "fluxType", os.FluxType.ToString() },
            { "filialDestinoId", os.FilialDestinoId },
            { "quantidadePlanejada", os.QuantidadePlanejada }
        };

        var evento = new AuditoriaEvento
        {
            Id = Guid.NewGuid(),
            EventType = AuditoriaEventType.OS_CRIADA,
            CorrelationType = "OS",
            CorrelationId = os.Id.ToString(),
            ActorRole = actorRole,
            ActorId = actorId,
            Timestamp = DateTimeOffset.UtcNow,
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(payload)
        };

        _context.AuditoriaEventos.Add(evento);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Registra evento de validação fiscal de NFe.
    /// </summary>
    public async Task RegistrarValidacaoFiscalAsync(
        NotaFiscal nfe,
        NfeValidacaoStatus validacaoStatus,
        string actorRole,
        string? actorId = null,
        string? motivoCategoria = null,
        string? motivoDetalhe = null)
    {
        var payload = new Dictionary<string, object>
        {
            { "nfeChaveAcesso", nfe.ChaveAcesso },
            { "validacaoStatus", validacaoStatus.ToString() }
        };

        if (validacaoStatus == NfeValidacaoStatus.INCORRETA)
        {
            payload["motivoCategoria"] = motivoCategoria ?? string.Empty;
            payload["motivoDetalhe"] = motivoDetalhe ?? string.Empty;
        }

        var evento = new AuditoriaEvento
        {
            Id = Guid.NewGuid(),
            EventType = AuditoriaEventType.FISCAL_NFE_VALIDADA,
            CorrelationType = "NFE",
            CorrelationId = nfe.ChaveAcesso,
            ActorRole = actorRole,
            ActorId = actorId,
            Timestamp = DateTimeOffset.UtcNow,
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(payload)
        };

        _context.AuditoriaEventos.Add(evento);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Registra evento de criação de vínculo.
    /// </summary>
    public async Task RegistrarVinculoCriadoAsync(
        Domain.Entities.Vinculo vinculo,
        string actorRole,
        string? actorId = null)
    {
        var payload = new Dictionary<string, object>
        {
            { "vinculoId", vinculo.Id.ToString() },
            { "osId", vinculo.OsId.ToString() },
            { "nfeChaveAcesso", vinculo.NfeChaveAcesso }
        };

        if (vinculo.OcId.HasValue)
        {
            payload["ocId"] = vinculo.OcId.Value.ToString();
        }

        if (vinculo.DivergenciaQuantidade.HasValue)
        {
            payload["divergenciaQuantidade"] = vinculo.DivergenciaQuantidade.Value;
        }

        if (!string.IsNullOrWhiteSpace(vinculo.Observacao))
        {
            payload["observacao"] = vinculo.Observacao;
        }

        var evento = new AuditoriaEvento
        {
            Id = Guid.NewGuid(),
            EventType = AuditoriaEventType.VINCULO_CRIADO,
            CorrelationType = "VINCULO",
            CorrelationId = vinculo.Id.ToString(),
            ActorRole = actorRole,
            ActorId = actorId,
            Timestamp = DateTimeOffset.UtcNow,
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(payload)
        };

        _context.AuditoriaEventos.Add(evento);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Registra evento de criação de pendência.
    /// </summary>
    public async Task RegistrarPendenciaAbertaAsync(
        Pendencia pendencia,
        string actorRole,
        string? actorId = null)
    {
        var payload = new Dictionary<string, object>
        {
            { "pendenciaId", pendencia.Id.ToString() },
            { "tipo", pendencia.Tipo.ToString() },
            { "correlationType", pendencia.CorrelationType },
            { "correlationId", pendencia.CorrelationId },
            { "descricao", pendencia.Descricao }
        };

        if (!string.IsNullOrWhiteSpace(pendencia.OwnerRole))
        {
            payload["ownerRole"] = pendencia.OwnerRole;
        }

        var evento = new AuditoriaEvento
        {
            Id = Guid.NewGuid(),
            EventType = AuditoriaEventType.PENDENCIA_ABERTA,
            CorrelationType = pendencia.CorrelationType,
            CorrelationId = pendencia.CorrelationId,
            ActorRole = actorRole,
            ActorId = actorId,
            Timestamp = DateTimeOffset.UtcNow,
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(payload)
        };

        _context.AuditoriaEventos.Add(evento);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Registra evento de cancelamento de processo.
    /// </summary>
    public async Task RegistrarProcessoCanceladoAsync(
        OrdemServico os,
        string motivo,
        string actorRole,
        string? actorId = null)
    {
        var payload = new Dictionary<string, object>
        {
            { "osId", os.Id.ToString() },
            { "osNumero", os.Numero },
            { "motivo", motivo },
            { "statusAnterior", os.StatusWorkflow.ToString() }
        };

        var evento = new AuditoriaEvento
        {
            Id = Guid.NewGuid(),
            EventType = AuditoriaEventType.PROCESSO_CANCELADO,
            CorrelationType = "OS",
            CorrelationId = os.Id.ToString(),
            ActorRole = actorRole,
            ActorId = actorId,
            Timestamp = DateTimeOffset.UtcNow,
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(payload)
        };

        _context.AuditoriaEventos.Add(evento);
        await _context.SaveChangesAsync();
    }
}
