using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Application.UseCases.Vinculo;
using TransferenciaMateriais.Application.Services;
using TransferenciaMateriais.Domain.Entities;
using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace TransferenciaMateriais.Application.Tests.UseCases.Vinculo;

public class CriarVinculoUseCaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CriarVinculoUseCase _useCase;
    private readonly AuditoriaService _auditoriaService;

    public CriarVinculoUseCaseTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        var loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole());
        _auditoriaService = new AuditoriaService(_context, loggerFactory.CreateLogger<AuditoriaService>());
        _useCase = new CriarVinculoUseCase(_context, _auditoriaService);
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_ShouldCreateVinculo()
    {
        // Arrange
        var os = new OrdemServico
        {
            Id = Guid.NewGuid(),
            Numero = "OS-2025-000123",
            FluxType = FluxType.COMPRA_DIRETA,
            FilialDestinoId = "FIL-DEST-01",
            QuantidadePlanejada = 10,
            StatusWorkflow = WorkflowStatus.OS_CRIADA,
            CreatedAt = DateTimeOffset.UtcNow
        };
        _context.OrdensServico.Add(os);

        var nfe = new NotaFiscal
        {
            ChaveAcesso = "35250123456789000123550010000012341000012345",
            Tipo = NfeTipo.VENDA,
            ValidacaoStatus = NfeValidacaoStatus.CORRETA,
            CreatedAt = DateTimeOffset.UtcNow
        };
        _context.NotasFiscais.Add(nfe);
        await _context.SaveChangesAsync();

        var request = new VinculoCreateRequest
        {
            OsId = os.Id,
            NfeChaveAcesso = nfe.ChaveAcesso,
            DivergenciaQuantidade = -1
        };

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(os.Id, result.OsId);
        Assert.Equal(nfe.ChaveAcesso, result.NfeChaveAcesso);
        Assert.Equal(VinculoStatus.CRIADO, result.Status);
        Assert.Equal(-1, result.DivergenciaQuantidade);

        // Verificar auditoria
        var eventos = await _context.AuditoriaEventos
            .Where(a => a.CorrelationId == result.Id.ToString())
            .ToListAsync();
        Assert.Contains(eventos, e => e.EventType == AuditoriaEventType.VINCULO_CRIADO);
    }

    [Fact]
    public async Task ExecuteAsync_DuplicateVinculo_ShouldThrow()
    {
        // Arrange
        var os = new OrdemServico
        {
            Id = Guid.NewGuid(),
            Numero = "OS-2025-000123",
            FluxType = FluxType.COMPRA_DIRETA,
            FilialDestinoId = "FIL-DEST-01",
            QuantidadePlanejada = 10,
            StatusWorkflow = WorkflowStatus.OS_CRIADA,
            CreatedAt = DateTimeOffset.UtcNow
        };
        _context.OrdensServico.Add(os);

        var nfe = new NotaFiscal
        {
            ChaveAcesso = "35250123456789000123550010000012341000012345",
            Tipo = NfeTipo.VENDA,
            ValidacaoStatus = NfeValidacaoStatus.CORRETA,
            CreatedAt = DateTimeOffset.UtcNow
        };
        _context.NotasFiscais.Add(nfe);
        await _context.SaveChangesAsync();

        var request = new VinculoCreateRequest
        {
            OsId = os.Id,
            NfeChaveAcesso = nfe.ChaveAcesso
        };

        await _useCase.ExecuteAsync(request);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _useCase.ExecuteAsync(request));
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
