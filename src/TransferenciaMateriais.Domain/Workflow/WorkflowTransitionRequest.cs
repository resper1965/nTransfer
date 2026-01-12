using TransferenciaMateriais.Domain.Enums;

namespace TransferenciaMateriais.Domain.Workflow;

/// <summary>
/// Requisição para transição de workflow.
/// </summary>
public class WorkflowTransitionRequest
{
    public Guid OsId { get; set; }
    public WorkflowStatus FromStatus { get; set; }
    public WorkflowStatus ToStatus { get; set; }
    public string ActorRole { get; set; } = string.Empty;
    public string? ActorId { get; set; }
    public string? Reason { get; set; }
    public Dictionary<string, object>? Context { get; set; }
}
