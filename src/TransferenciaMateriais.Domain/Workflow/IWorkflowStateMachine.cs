using TransferenciaMateriais.Domain.Enums;

namespace TransferenciaMateriais.Domain.Workflow;

/// <summary>
/// Interface para a máquina de estados do workflow.
/// </summary>
public interface IWorkflowStateMachine
{
    /// <summary>
    /// Valida se uma transição é permitida.
    /// </summary>
    WorkflowTransitionResult CanTransition(WorkflowTransitionRequest request);

    /// <summary>
    /// Obtém as transições permitidas a partir de um estado.
    /// </summary>
    List<WorkflowStatus> GetAllowedTransitions(WorkflowStatus currentStatus, FluxType fluxType, string actorRole);

    /// <summary>
    /// Valida regras de negócio específicas para uma transição.
    /// </summary>
    WorkflowTransitionResult ValidateBusinessRules(WorkflowTransitionRequest request);
}
