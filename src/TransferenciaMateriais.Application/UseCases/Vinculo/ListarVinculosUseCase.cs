using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TransferenciaMateriais.Application.UseCases.Vinculo;

public class ListarVinculosUseCase
{
    private readonly ApplicationDbContext _context;

    public ListarVinculosUseCase(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<VinculoDto>> ExecuteAsync(
        Guid? osId = null,
        Guid? ocId = null,
        string? nfeChaveAcesso = null)
    {
        var query = _context.Vinculos.AsQueryable();

        if (osId.HasValue)
        {
            query = query.Where(v => v.OsId == osId.Value);
        }

        if (ocId.HasValue)
        {
            query = query.Where(v => v.OcId == ocId.Value);
        }

        if (!string.IsNullOrWhiteSpace(nfeChaveAcesso))
        {
            query = query.Where(v => v.NfeChaveAcesso == nfeChaveAcesso);
        }

        var vinculos = await query.ToListAsync();

        return vinculos.Select(v => new VinculoDto
        {
            Id = v.Id,
            OsId = v.OsId,
            OcId = v.OcId,
            NfeChaveAcesso = v.NfeChaveAcesso,
            Status = v.Status,
            DivergenciaQuantidade = v.DivergenciaQuantidade,
            Observacao = v.Observacao,
            CreatedAt = v.CreatedAt
        }).ToList();
    }
}
