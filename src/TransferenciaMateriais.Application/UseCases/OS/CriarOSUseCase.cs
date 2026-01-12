using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Application.Services;
using TransferenciaMateriais.Domain.Entities;
using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TransferenciaMateriais.Application.UseCases.OS;

public class CriarOSUseCase
{
    private readonly ApplicationDbContext _context;
    private readonly AuditoriaService _auditoriaService;

    public CriarOSUseCase(
        ApplicationDbContext context,
        AuditoriaService auditoriaService)
    {
        _context = context;
        _auditoriaService = auditoriaService;
    }

    public async Task<OSDto> ExecuteAsync(OSCreateRequest request, string actorRole, string? actorId = null)
    {
        // Validação: número único
        var existe = await _context.OrdensServico.AnyAsync(o => o.Numero == request.Numero);
        if (existe)
        {
            throw new InvalidOperationException($"OS com número '{request.Numero}' já existe.");
        }

        // Validação: quantidade > 0
        if (request.QuantidadePlanejada <= 0)
        {
            throw new ArgumentException("Quantidade planejada deve ser maior que zero.");
        }

        var os = new OrdemServico
        {
            Id = Guid.NewGuid(),
            Numero = request.Numero,
            FluxType = request.FluxType,
            FilialDestinoId = request.FilialDestinoId,
            QuantidadePlanejada = request.QuantidadePlanejada,
            DataEstimadaEntrega = request.DataEstimadaEntrega,
            StatusWorkflow = WorkflowStatus.OS_CRIADA,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _context.OrdensServico.Add(os);
        await _context.SaveChangesAsync();

        // Registrar auditoria
        await _auditoriaService.RegistrarOSCriadaAsync(os, actorRole, actorId);

        return MapToDto(os);
    }

    private static OSDto MapToDto(OrdemServico os)
    {
        return new OSDto
        {
            Id = os.Id,
            Numero = os.Numero,
            FluxType = os.FluxType,
            FilialDestinoId = os.FilialDestinoId,
            QuantidadePlanejada = os.QuantidadePlanejada,
            DataEstimadaEntrega = os.DataEstimadaEntrega,
            StatusWorkflow = os.StatusWorkflow,
            CreatedAt = os.CreatedAt,
            UpdatedAt = os.UpdatedAt
        };
    }
}
