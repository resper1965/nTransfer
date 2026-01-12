namespace TransferenciaMateriais.Domain.Entities;

/// <summary>
/// Anexo de documento.
/// </summary>
public class Anexo
{
    public Guid Id { get; set; }
    public string CorrelationType { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string StorageRef { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public long? SizeBytes { get; set; }
    public string? UploadedBy { get; set; }
    public DateTimeOffset UploadedAt { get; set; }
}
