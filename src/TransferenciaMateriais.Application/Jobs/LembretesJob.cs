using TransferenciaMateriais.Application.UseCases.Notification;
using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TransferenciaMateriais.Application.Jobs;

/// <summary>
/// Job agendado para disparar lembretes de 7 e 30 dias.
/// </summary>
public class LembretesJob
{
    private readonly ApplicationDbContext _context;
    private readonly EnviarNotificacaoUseCase _enviarNotificacaoUseCase;
    private readonly ILogger<LembretesJob> _logger;

    public LembretesJob(
        ApplicationDbContext context,
        EnviarNotificacaoUseCase enviarNotificacaoUseCase,
        ILogger<LembretesJob> logger)
    {
        _context = context;
        _enviarNotificacaoUseCase = enviarNotificacaoUseCase;
        _logger = logger;
    }

    public async Task ProcessarLembretesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando processamento de lembretes...");

        // Lembrete de 7 dias para entrega estimada (F2 - Entrega Futura mãe)
        await ProcessarLembrete7DiasAsync(cancellationToken);

        // Lembrete de 30 dias no destino
        await ProcessarLembrete30DiasAsync(cancellationToken);

        _logger.LogInformation("Processamento de lembretes concluído.");
    }

    private async Task ProcessarLembrete7DiasAsync(CancellationToken cancellationToken)
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var dataLimite = hoje.AddDays(7);

        var osParaLembrete = await _context.OrdensServico
            .Where(os =>
                os.FluxType == FluxType.ENTREGA_FUTURA_MAE &&
                os.DataEstimadaEntrega.HasValue &&
                os.DataEstimadaEntrega.Value == dataLimite &&
                os.StatusWorkflow != WorkflowStatus.PROCESSO_CONCLUIDO &&
                os.StatusWorkflow != WorkflowStatus.PROCESSO_CANCELADO)
            .ToListAsync(cancellationToken);

        foreach (var os in osParaLembrete)
        {
            try
            {
                await _enviarNotificacaoUseCase.ExecuteAsync(
                    NotificacaoTipo.LEMBRETE_7_DIAS_ENTREGA_ESTIMADA,
                    "OS",
                    os.Id.ToString(),
                    filialDestinoId: os.FilialDestinoId,
                    additionalVariables: new Dictionary<string, string>
                    {
                        { "dataEstimadaEntrega", os.DataEstimadaEntrega!.Value.ToString("yyyy-MM-dd") }
                    }
                );

                _logger.LogInformation("Lembrete 7 dias enviado para OS {OsNumero}", os.Numero);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar lembrete 7 dias para OS {OsId}", os.Id);
            }
        }
    }

    private async Task ProcessarLembrete30DiasAsync(CancellationToken cancellationToken)
    {
        var hoje = DateTimeOffset.UtcNow;
        var dataLimite = hoje.AddDays(-30);

        // Estados que indicam processo no destino
        var estadosDestino = new[]
        {
            WorkflowStatus.CHEGADA_MATERIAL_DESTINO,
            WorkflowStatus.ENTRADA_DESTINO_PENDENTE_ANEXO,
            WorkflowStatus.ENTRADA_DESTINO_CONCLUIDA
        };

        var osParaLembrete = await _context.OrdensServico
            .Where(os =>
                estadosDestino.Contains(os.StatusWorkflow) &&
                os.UpdatedAt <= dataLimite &&
                os.StatusWorkflow != WorkflowStatus.PROCESSO_CONCLUIDO &&
                os.StatusWorkflow != WorkflowStatus.PROCESSO_CANCELADO)
            .ToListAsync(cancellationToken);

        foreach (var os in osParaLembrete)
        {
            try
            {
                await _enviarNotificacaoUseCase.ExecuteAsync(
                    NotificacaoTipo.LEMBRETE_30_DIAS_DESTINO,
                    "OS",
                    os.Id.ToString(),
                    filialDestinoId: os.FilialDestinoId
                );

                _logger.LogInformation("Lembrete 30 dias enviado para OS {OsNumero}", os.Numero);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar lembrete 30 dias para OS {OsId}", os.Id);
            }
        }
    }
}
