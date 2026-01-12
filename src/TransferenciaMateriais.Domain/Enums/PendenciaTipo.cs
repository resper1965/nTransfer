namespace TransferenciaMateriais.Domain.Enums;

/// <summary>
/// Tipo de pendência operacional.
/// </summary>
public enum PendenciaTipo
{
    ERRO_VINCULO,
    NFE_INCORRETA,
    FALTA_ANEXO_OBRIGATORIO,
    INTEGRACAO_FALHOU,
    SEM_VINCULO_REMESSA,
    ATRASO_ENTREGA_7_DIAS,
    ATRASO_DESTINO_30_DIAS,
    APROVACAO_PENDENTE,
    MEDICAO_PENDENTE
}
