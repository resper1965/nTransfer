using TransferenciaMateriais.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TransferenciaMateriais.Infrastructure.Integration;

/// <summary>
/// Stub do adaptador de integração (TBD-01).
/// Suporta modos: ALWAYS_OK, FAIL_RATE, SCENARIO.
/// </summary>
public class StubIntegrationAdapter : IIntegrationAdapter
{
    private readonly ILogger<StubIntegrationAdapter> _logger;
    private readonly IConfiguration _configuration;
    private readonly Random _random = new();

    public StubIntegrationAdapter(
        ILogger<StubIntegrationAdapter> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public Task<IntegrationResult> ReceiveNFeAsync(NFeReceivedEvent nfeEvent, CancellationToken cancellationToken = default)
    {
        var mode = _configuration["INTEGRATION_MODE"] ?? "stub";
        var stubMode = _configuration["STUB_MODE"] ?? "ALWAYS_OK";

        _logger.LogInformation(
            "STUB: NFe recebida - Chave: {ChaveAcesso}, IdempotencyKey: {IdempotencyKey}, Mode: {Mode}",
            nfeEvent.ChaveAcesso,
            nfeEvent.IdempotencyKey,
            stubMode
        );

        // Modo ALWAYS_OK (padrão)
        if (stubMode == "ALWAYS_OK")
        {
            return Task.FromResult(new IntegrationResult(Success: true));
        }

        // Modo FAIL_RATE
        if (stubMode == "FAIL_RATE")
        {
            var failRate = double.Parse(_configuration["STUB_FAIL_RATE"] ?? "0.1");
            if (_random.NextDouble() < failRate)
            {
                _logger.LogWarning("STUB: Simulando falha (FAIL_RATE)");
                return Task.FromResult(new IntegrationResult(
                    Success: false,
                    ErrorMessage: "Falha simulada pelo stub (FAIL_RATE)"
                ));
            }
            return Task.FromResult(new IntegrationResult(Success: true));
        }

        // Modo SCENARIO (pode ser expandido)
        if (stubMode == "SCENARIO")
        {
            var forceFail = _configuration["STUB_FORCE_FAIL"] == "true";
            if (forceFail)
            {
                _logger.LogWarning("STUB: Simulando falha (FORCE_FAIL)");
                return Task.FromResult(new IntegrationResult(
                    Success: false,
                    ErrorMessage: "Falha simulada pelo stub (FORCE_FAIL)"
                ));
            }
            return Task.FromResult(new IntegrationResult(Success: true));
        }

        // Default: sempre OK
        return Task.FromResult(new IntegrationResult(Success: true));
    }
}
