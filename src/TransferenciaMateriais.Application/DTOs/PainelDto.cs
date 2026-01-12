using TransferenciaMateriais.Domain.Enums;

namespace TransferenciaMateriais.Application.DTOs;

public class OCPendenteDto
{
    public Guid OcId { get; set; }
    public string OcNumero { get; set; } = string.Empty;
    public Guid OsId { get; set; }
    public string OsNumero { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty; // "MAE" ou "FILHA"
    public string Status { get; set; } = string.Empty;
    public string FilialDestinoId { get; set; } = string.Empty;
    public DateOnly? DataEstimadaEntrega { get; set; }
    public int? DiasPendente { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class PendenciaErroVinculoDto
{
    public Guid PendenciaId { get; set; }
    public Guid OsId { get; set; }
    public string OsNumero { get; set; } = string.Empty;
    public string? NfeChaveAcesso { get; set; }
    public string MotivoErro { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTimeOffset DataErro { get; set; }
    public PendenciaStatus Status { get; set; }
}

public class AprovacaoPendenteDto
{
    public Guid Id { get; set; }
    public string Tipo { get; set; } = string.Empty; // "ENTREGA", "MEDICAO", "VINCULO"
    public string Status { get; set; } = string.Empty; // "PENDENTE", "APROVADA", "REPROVADA"
    public Guid? OsId { get; set; }
    public string? OsNumero { get; set; }
    public Guid? OcId { get; set; }
    public string? OcNumero { get; set; }
    public string? NfeChaveAcesso { get; set; }
    public string? Responsavel { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
