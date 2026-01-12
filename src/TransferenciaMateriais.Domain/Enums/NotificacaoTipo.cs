namespace TransferenciaMateriais.Domain.Enums;

/// <summary>
/// Tipo de notificação por e-mail.
/// </summary>
public enum NotificacaoTipo
{
    CHEGADA_MATERIAL,
    NFE_ENTRADA_DISPONIVEL,
    NFE_SAIDA_PRONTA_IMPRESSAO,
    CANCELAMENTO_PROCESSO,
    PENDENCIA_ABERTA,
    LEMBRETE_7_DIAS_ENTREGA_ESTIMADA,
    LEMBRETE_30_DIAS_DESTINO,
    MEDICAO_CONCLUIDA
}
