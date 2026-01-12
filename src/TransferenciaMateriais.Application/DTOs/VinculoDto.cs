using TransferenciaMateriais.Domain.Enums;

namespace TransferenciaMateriais.Application.DTOs;

public class VinculoDto
{
    public Guid Id { get; set; }
    public Guid OsId { get; set; }
    public Guid? OcId { get; set; }
    public string NfeChaveAcesso { get; set; } = string.Empty;
    public VinculoStatus Status { get; set; }
    public decimal? DivergenciaQuantidade { get; set; }
    public string? Observacao { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class VinculoCreateRequest
{
    public Guid OsId { get; set; }
    public Guid? OcId { get; set; }
    public string NfeChaveAcesso { get; set; } = string.Empty;
    public decimal? DivergenciaQuantidade { get; set; }
    public string? Observacao { get; set; }
}
