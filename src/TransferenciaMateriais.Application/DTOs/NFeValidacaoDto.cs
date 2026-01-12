using TransferenciaMateriais.Domain.Enums;

namespace TransferenciaMateriais.Application.DTOs;

public class NFeValidacaoRequest
{
    public string Decisao { get; set; } = string.Empty; // "CORRETA" ou "INCORRETA"
    public NFeIncorretaMotivo? Motivo { get; set; }
}

public class NFeIncorretaMotivo
{
    public NfeIncorretaMotivoCategoria Categoria { get; set; }
    public string Detalhe { get; set; } = string.Empty;
}

public class NFeValidacaoResponse
{
    public string ChaveAcesso { get; set; } = string.Empty;
    public NfeValidacaoStatus ValidacaoStatus { get; set; }
    public string? MotivoCategoria { get; set; }
    public string? MotivoDetalhe { get; set; }
}
