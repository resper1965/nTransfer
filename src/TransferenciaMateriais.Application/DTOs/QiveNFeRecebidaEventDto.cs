namespace TransferenciaMateriais.Application.DTOs;

public class QiveNFeRecebidaEventDto
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public QiveNFeDto Nfe { get; set; } = new();
    public QiveContextDto? Context { get; set; }
}

public class QiveNFeDto
{
    public string ChaveAcesso { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string? XmlBase64 { get; set; }
}

public class QiveContextDto
{
    public string? OsNumero { get; set; }
    public string? OcNumero { get; set; }
}
