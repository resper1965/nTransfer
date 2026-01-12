using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TransferenciaMateriais.Application.UseCases.OS;

public class AtualizarOSUseCase
{
    private readonly ApplicationDbContext _context;

    public AtualizarOSUseCase(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OSDto?> ExecuteAsync(Guid osId, OSUpdateRequest request)
    {
        var os = await _context.OrdensServico.FindAsync(osId);
        if (os == null)
        {
            return null;
        }

        if (request.DataEstimadaEntrega.HasValue)
        {
            os.DataEstimadaEntrega = request.DataEstimadaEntrega;
        }

        os.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();

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
