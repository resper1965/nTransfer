using TransferenciaMateriais.Application.Services;
using TransferenciaMateriais.Application.UseCases.Notification;
using TransferenciaMateriais.Domain.Entities;
using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TransferenciaMateriais.Application.UseCases.Medicao;

/// <summary>
/// Use Case para processar conclusão de medição (chamado via integração RM).
/// TBD-04 fechado: medição realizada no RM/Contratos com base na OS aberta no nFlow.
/// </summary>
public class ProcessarMedicaoConcluidaUseCase
{
    private readonly ApplicationDbContext _context;
    private readonly AuditoriaService _auditoriaService;
    private readonly EnviarNotificacaoUseCase _enviarNotificacaoUseCase;
    private readonly ILogger<ProcessarMedicaoConcluidaUseCase> _logger;

    public ProcessarMedicaoConcluidaUseCase(
        ApplicationDbContext context,
        AuditoriaService auditoriaService,
        EnviarNotificacaoUseCase enviarNotificacaoUseCase,
        ILogger<ProcessarMedicaoConcluidaUseCase> logger)
    {
        _context = context;
        _auditoriaService = auditoriaService;
        _enviarNotificacaoUseCase = enviarNotificacaoUseCase;
        _logger = logger;
    }

    public async Task ProcessarAsync(
        Guid osId,
        bool aprovada,
        string? motivoReprovacao = null)
    {
        var os = await _context.OrdensServico.FindAsync(osId);
        if (os == null)
        {
            throw new InvalidOperationException($"OS com ID '{osId}' não encontrada.");
        }

        // Atualizar estado conforme resultado da medição
        var novoStatus = aprovada
            ? WorkflowStatus.MEDICAO_APROVADA
            : WorkflowStatus.MEDICAO_REPROVADA;

        var statusAnterior = os.StatusWorkflow;
        os.StatusWorkflow = novoStatus;
        os.UpdatedAt = DateTimeOffset.UtcNow;

        // Registrar auditoria
        await _auditoriaService.RegistrarTransicaoWorkflowAsync(
            "OS",
            os.Id.ToString(),
            statusAnterior,
            novoStatus,
            "SISTEMA",
            "RM_INTEGRATION",
            motivoReprovacao,
            new Dictionary<string, object>
            {
                { "medicaoAprovada", aprovada },
                { "motivoReprovacao", motivoReprovacao ?? string.Empty }
            }
        );

        // Registrar evento específico de medição
        var eventoMedicao = new AuditoriaEvento
        {
            Id = Guid.NewGuid(),
            EventType = aprovada ? AuditoriaEventType.MEDICAO_APROVADA : AuditoriaEventType.MEDICAO_REPROVADA,
            CorrelationType = "OS",
            CorrelationId = os.Id.ToString(),
            ActorRole = "SISTEMA",
            ActorId = "RM_INTEGRATION",
            Timestamp = DateTimeOffset.UtcNow,
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                osId = os.Id.ToString(),
                osNumero = os.Numero,
                aprovada = aprovada,
                motivoReprovacao = motivoReprovacao
            })
        };
        _context.AuditoriaEventos.Add(eventoMedicao);

        await _context.SaveChangesAsync();

        // Enviar notificação
        try
        {
            await _enviarNotificacaoUseCase.ExecuteAsync(
                NotificacaoTipo.MEDICAO_CONCLUIDA,
                "OS",
                os.Id.ToString(),
                filialDestinoId: os.FilialDestinoId
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificação de medição concluída para OS {OsId}", os.Id);
        }

        _logger.LogInformation(
            "Medição processada para OS {OsNumero}: {Status}",
            os.Numero,
            novoStatus
        );
    }
}
