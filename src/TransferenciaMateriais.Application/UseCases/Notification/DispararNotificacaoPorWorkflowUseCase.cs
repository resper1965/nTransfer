using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace TransferenciaMateriais.Application.UseCases.Notification;

/// <summary>
/// Use Case para disparar notificações baseadas em transições de workflow.
/// </summary>
public class DispararNotificacaoPorWorkflowUseCase
{
    private readonly EnviarNotificacaoUseCase _enviarNotificacaoUseCase;
    private readonly ILogger<DispararNotificacaoPorWorkflowUseCase> _logger;

    public DispararNotificacaoPorWorkflowUseCase(
        EnviarNotificacaoUseCase enviarNotificacaoUseCase,
        ILogger<DispararNotificacaoPorWorkflowUseCase> logger)
    {
        _enviarNotificacaoUseCase = enviarNotificacaoUseCase;
        _logger = logger;
    }

    public async Task DispararPorTransicaoAsync(
        OrdemServico os,
        WorkflowStatus fromStatus,
        WorkflowStatus toStatus)
    {
        try
        {
            switch (toStatus)
            {
                case WorkflowStatus.CHEGADA_MATERIAL_DESTINO:
                    await _enviarNotificacaoUseCase.ExecuteAsync(
                        NotificacaoTipo.CHEGADA_MATERIAL,
                        "OS",
                        os.Id.ToString(),
                        filialDestinoId: os.FilialDestinoId
                    );
                    break;

                case WorkflowStatus.NFE_SAIDA_ORIGEM_EMITIDA:
                    await _enviarNotificacaoUseCase.ExecuteAsync(
                        NotificacaoTipo.NFE_SAIDA_PRONTA_IMPRESSAO,
                        "OS",
                        os.Id.ToString(),
                        filialDestinoId: os.FilialDestinoId
                    );
                    break;

                case WorkflowStatus.PROCESSO_CANCELADO:
                    await _enviarNotificacaoUseCase.ExecuteAsync(
                        NotificacaoTipo.CANCELAMENTO_PROCESSO,
                        "OS",
                        os.Id.ToString(),
                        filialDestinoId: os.FilialDestinoId
                    );
                    break;
            }

            // Notificações de NFe entrada disponível são disparadas quando XML é obtido
            // (já implementado em ProcessarNFeQiveUseCase)
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao disparar notificação por transição de workflow: OS {OsId}, {FromStatus} -> {ToStatus}",
                os.Id, fromStatus, toStatus
            );
            // Não propagar exceção - notificação não deve quebrar o workflow
        }
    }
}
