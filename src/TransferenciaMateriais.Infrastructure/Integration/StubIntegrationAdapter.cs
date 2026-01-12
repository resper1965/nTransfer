using TransferenciaMateriais.Domain;

namespace TransferenciaMateriais.Infrastructure.Integration;

/// <summary>
/// Stub do adaptador de integração (TBD-01).
/// Registra eventos sem processar integração real.
/// </summary>
public class StubIntegrationAdapter : IIntegrationAdapter
{
    private readonly ILogger<StubIntegrationAdapter> _logger;

    public StubIntegrationAdapter(ILogger<StubIntegrationAdapter> logger)
    {
        _logger = logger;
    }

    public Task<IntegrationResult> ReceiveNFeAsync(NFeReceivedEvent nfeEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "STUB: NFe recebida - Chave: {ChaveAcesso}, IdempotencyKey: {IdempotencyKey}",
            nfeEvent.ChaveAcesso,
            nfeEvent.IdempotencyKey
        );

        // Stub sempre retorna sucesso
        // Em produção, aqui haveria validação de idempotência e processamento real
        return Task.FromResult(new IntegrationResult(Success: true));
    }
}
