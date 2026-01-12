using TransferenciaMateriais.Domain.Entities;
using TransferenciaMateriais.Domain.Enums;

namespace TransferenciaMateriais.Domain.Workflow;

/// <summary>
/// Serviço de workflow que integra state machine com entidades.
/// </summary>
public class WorkflowService
{
    private readonly IWorkflowStateMachine _stateMachine;

    public WorkflowService(IWorkflowStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    /// <summary>
    /// Executa uma transição de workflow em uma OS.
    /// </summary>
    public WorkflowTransitionResult ExecuteTransition(
        OrdemServico os,
        WorkflowStatus toStatus,
        string actorRole,
        string? actorId = null,
        string? reason = null,
        Dictionary<string, object>? context = null)
    {
        var request = new WorkflowTransitionRequest
        {
            OsId = os.Id,
            FromStatus = os.StatusWorkflow,
            ToStatus = toStatus,
            ActorRole = actorRole,
            ActorId = actorId,
            Reason = reason,
            Context = context
        };

        // Valida transição
        var canTransition = _stateMachine.CanTransition(request);
        if (!canTransition.IsValid)
        {
            return canTransition;
        }

        // Valida regras de negócio
        var businessRules = _stateMachine.ValidateBusinessRules(request);
        if (!businessRules.IsValid)
        {
            return businessRules;
        }

        // Atualiza status
        os.StatusWorkflow = toStatus;
        os.UpdatedAt = DateTimeOffset.UtcNow;

        return WorkflowTransitionResult.Success();
    }

    /// <summary>
    /// Obtém transições permitidas para uma OS.
    /// </summary>
    public List<WorkflowStatus> GetAllowedTransitions(OrdemServico os, string actorRole)
    {
        return _stateMachine.GetAllowedTransitions(os.StatusWorkflow, os.FluxType, actorRole);
    }
}
