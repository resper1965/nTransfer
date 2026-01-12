using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TransferenciaMateriais.Application.UseCases.Paineis;

public class ListarOCPendenteEntregaFuturaUseCase
{
    private readonly ApplicationDbContext _context;

    public ListarOCPendenteEntregaFuturaUseCase(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<OCPendenteDto>> ExecuteAsync(
        string? filialDestinoId = null,
        string? tipo = null, // "MAE" ou "FILHA"
        string? status = null,
        DateOnly? dataEstimadaFrom = null,
        DateOnly? dataEstimadaTo = null)
    {
        var query = _context.OrdensCompra
            .Include(oc => oc.OrdemServico)
            .AsQueryable();

        // Filtrar por fluxo de entrega futura
        query = query.Where(oc => 
            oc.OrdemServico.FluxType == Domain.Enums.FluxType.ENTREGA_FUTURA_MAE ||
            oc.OrdemServico.FluxType == Domain.Enums.FluxType.ENTREGA_FUTURA_FILHA);

        if (!string.IsNullOrWhiteSpace(filialDestinoId))
        {
            query = query.Where(oc => oc.OrdemServico.FilialDestinoId == filialDestinoId);
        }

        if (!string.IsNullOrWhiteSpace(tipo))
        {
            var fluxType = tipo == "MAE" 
                ? Domain.Enums.FluxType.ENTREGA_FUTURA_MAE 
                : Domain.Enums.FluxType.ENTREGA_FUTURA_FILHA;
            query = query.Where(oc => oc.OrdemServico.FluxType == fluxType);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(oc => oc.Status == status);
        }

        if (dataEstimadaFrom.HasValue)
        {
            query = query.Where(oc => 
                oc.OrdemServico.DataEstimadaEntrega.HasValue &&
                oc.OrdemServico.DataEstimadaEntrega >= dataEstimadaFrom.Value);
        }

        if (dataEstimadaTo.HasValue)
        {
            query = query.Where(oc => 
                oc.OrdemServico.DataEstimadaEntrega.HasValue &&
                oc.OrdemServico.DataEstimadaEntrega <= dataEstimadaTo.Value);
        }

        var ocs = await query.ToListAsync();

        return ocs.Select(oc =>
        {
            var tipoStr = oc.OrdemServico.FluxType == Domain.Enums.FluxType.ENTREGA_FUTURA_MAE ? "MAE" : "FILHA";
            var diasPendente = oc.OrdemServico.DataEstimadaEntrega.HasValue
                ? (int?)(DateOnly.FromDateTime(DateTime.UtcNow) - oc.OrdemServico.DataEstimadaEntrega.Value).Days
                : null;

            return new OCPendenteDto
            {
                OcId = oc.Id,
                OcNumero = oc.Numero,
                OsId = oc.OsId,
                OsNumero = oc.OrdemServico.Numero,
                Tipo = tipoStr,
                Status = oc.Status,
                FilialDestinoId = oc.OrdemServico.FilialDestinoId,
                DataEstimadaEntrega = oc.OrdemServico.DataEstimadaEntrega,
                DiasPendente = diasPendente,
                CreatedAt = oc.CreatedAt
            };
        }).ToList();
    }
}
