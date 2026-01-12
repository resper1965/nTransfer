namespace TransferenciaMateriais.Domain;

/// <summary>
/// Adaptador de integração Qive ↔ RM (TBD-01).
/// Modo Stub até definição do mecanismo definitivo (webhook/polling/fila).
/// </summary>
public interface IIntegrationAdapter
{
    /// <summary>
    /// Recebe evento de NFe recebida do Qive/RM.
    /// Implementação deve ser idempotente (chave de acesso NFe como chave idempotente).
    /// </summary>
    Task<IntegrationResult> ReceiveNFeAsync(NFeReceivedEvent nfeEvent, CancellationToken cancellationToken = default);
}

/// <summary>
/// Resultado de integração.
/// </summary>
public record IntegrationResult(bool Success, string? ErrorMessage = null, string? PendingId = null);

/// <summary>
/// Evento de NFe recebida (conforme OpenAPI /integrations/qive/nfe-recebida).
/// </summary>
public record NFeReceivedEvent(
    string IdempotencyKey,
    string ChaveAcesso,
    string XmlRef,
    DateTime ReceivedAt
);
