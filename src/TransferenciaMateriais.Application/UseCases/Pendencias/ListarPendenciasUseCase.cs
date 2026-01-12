using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Domain.Entities;
using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TransferenciaMateriais.Application.UseCases.Pendencias;

public class ListarPendenciasUseCase
{
    private readonly ApplicationDbContext _context;

    public ListarPendenciasUseCase(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PendenciaDto>> ExecuteAsync(
        string? correlationType = null,
        string? correlationId = null,
        PendenciaStatus? status = null,
        PendenciaTipo? tipo = null)
    {
        var query = _context.Pendencias.AsQueryable();

        if (!string.IsNullOrWhiteSpace(correlationType))
        {
            query = query.Where(p => p.CorrelationType == correlationType);
        }

        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            query = query.Where(p => p.CorrelationId == correlationId);
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        if (tipo.HasValue)
        {
            query = query.Where(p => p.Tipo == tipo.Value);
        }

        var pendencias = await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return pendencias.Select(p => new PendenciaDto
        {
            Id = p.Id,
            Tipo = p.Tipo,
            Status = p.Status,
            CorrelationType = p.CorrelationType,
            CorrelationId = p.CorrelationId,
            Descricao = p.Descricao,
            OwnerRole = p.OwnerRole,
            DueAt = p.DueAt,
            CreatedAt = p.CreatedAt,
            ResolvedAt = p.ResolvedAt
        }).ToList();
    }
}
