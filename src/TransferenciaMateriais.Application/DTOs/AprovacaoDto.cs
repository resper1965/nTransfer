namespace TransferenciaMateriais.Application.DTOs;

public class AprovacaoDto
{
    public Guid Id { get; set; }
    public string Tipo { get; set; } = string.Empty; // "ENTREGA" ou "MEDICAO"
    public string Status { get; set; } = string.Empty; // "PENDENTE", "APROVADA", "REPROVADA"
    public Guid? OsId { get; set; }
    public string? OsNumero { get; set; }
    public Guid? OcId { get; set; }
    public string? OcNumero { get; set; }
    public string? NfeChaveAcesso { get; set; }
    public string? Responsavel { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class AprovacaoDecisaoRequest
{
    public string Decisao { get; set; } = string.Empty; // "APROVADA" ou "REPROVADA"
    public string? Motivo { get; set; }
}
