using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Application.UseCases.OS;
using TransferenciaMateriais.Application.Services;
using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace TransferenciaMateriais.Application.Tests.UseCases.OS;

public class CriarOSUseCaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CriarOSUseCase _useCase;
    private readonly AuditoriaService _auditoriaService;

    public CriarOSUseCaseTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _auditoriaService = new AuditoriaService(_context, new Microsoft.Extensions.Logging.LoggerFactory().CreateLogger<AuditoriaService>());
        _useCase = new CriarOSUseCase(_context, _auditoriaService);
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_ShouldCreateOS()
    {
        // Arrange
        var request = new OSCreateRequest
        {
            Numero = "OS-2025-000123",
            FluxType = FluxType.COMPRA_DIRETA,
            FilialDestinoId = "FIL-DEST-01",
            QuantidadePlanejada = 10
        };

        // Act
        var result = await _useCase.ExecuteAsync(request, "ADM_FILIAL_ORIGEM");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Numero, result.Numero);
        Assert.Equal(request.FluxType, result.FluxType);
        Assert.Equal(WorkflowStatus.OS_CRIADA, result.StatusWorkflow);

        // Verificar que foi persistido
        var os = await _context.OrdensServico.FindAsync(result.Id);
        Assert.NotNull(os);
        Assert.Equal(request.Numero, os.Numero);

        // Verificar auditoria
        var eventos = await _context.AuditoriaEventos
            .Where(a => a.CorrelationId == result.Id.ToString())
            .ToListAsync();
        Assert.Contains(eventos, e => e.EventType == AuditoriaEventType.OS_CRIADA);
    }

    [Fact]
    public async Task ExecuteAsync_DuplicateNumero_ShouldThrow()
    {
        // Arrange
        var request = new OSCreateRequest
        {
            Numero = "OS-2025-000123",
            FluxType = FluxType.COMPRA_DIRETA,
            FilialDestinoId = "FIL-DEST-01",
            QuantidadePlanejada = 10
        };

        await _useCase.ExecuteAsync(request, "ADM_FILIAL_ORIGEM");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecuteAsync(request, "ADM_FILIAL_ORIGEM"));
    }

    [Fact]
    public async Task ExecuteAsync_InvalidQuantidade_ShouldThrow()
    {
        // Arrange
        var request = new OSCreateRequest
        {
            Numero = "OS-2025-000124",
            FluxType = FluxType.COMPRA_DIRETA,
            FilialDestinoId = "FIL-DEST-01",
            QuantidadePlanejada = 0
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _useCase.ExecuteAsync(request, "ADM_FILIAL_ORIGEM"));
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
