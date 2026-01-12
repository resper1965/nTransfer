using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TransferenciaMateriais.Application.UseCases.OS;

public class ListarOSUseCase
{
    private readonly ApplicationDbContext _context;

    public ListarOSUseCase(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<OSDto>> ExecuteAsync(
        string? filialDestinoId = null,
        WorkflowStatus? status = null,
        FluxType? fluxType = null,
        string? numero = null)
    {
        var query = _context.OrdensServico.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filialDestinoId))
        {
            query = query.Where(o => o.FilialDestinoId == filialDestinoId);
        }

        if (status.HasValue)
        {
            query = query.Where(o => o.StatusWorkflow == status.Value);
        }

        if (fluxType.HasValue)
        {
            query = query.Where(o => o.FluxType == fluxType.Value);
        }

        if (!string.IsNullOrWhiteSpace(numero))
        {
            query = query.Where(o => o.Numero.Contains(numero));
        }

        var osList = await query.ToListAsync();

        return osList.Select(MapToDto).ToList();
    }

    private static OSDto MapToDto(Domain.Entities.OrdemServico os)
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
