namespace TransferenciaMateriais.Domain.Workflow;

/// <summary>
/// Resultado de uma transição de workflow.
/// </summary>
public class WorkflowTransitionResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> ValidationErrors { get; set; } = new();

    public static WorkflowTransitionResult Success() => new() { IsValid = true };
    
    public static WorkflowTransitionResult Failure(string errorMessage, List<string>? validationErrors = null) => new()
    {
        IsValid = false,
        ErrorMessage = errorMessage,
        ValidationErrors = validationErrors ?? new List<string>()
    };
}
