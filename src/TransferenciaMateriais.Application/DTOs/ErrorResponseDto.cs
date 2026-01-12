namespace TransferenciaMateriais.Application.DTOs;

public class ErrorResponseDto
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public Dictionary<string, object>? Details { get; set; }
}

public class ValidationErrorResponseDto : ErrorResponseDto
{
    public List<ValidationViolationDto> Violations { get; set; } = new();
}

public class ValidationViolationDto
{
    public string Field { get; set; } = string.Empty;
    public string Rule { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
