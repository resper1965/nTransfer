using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TransferenciaMateriais.Application.UseCases.Auditoria;

public class ListarAuditoriaUseCase
{
    private readonly ApplicationDbContext _context;

    public ListarAuditoriaUseCase(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AuditoriaEventoDto>> ExecuteAsync(
        string? correlationType = null,
        string? correlationId = null,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null)
    {
        var query = _context.AuditoriaEventos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(correlationType))
        {
            query = query.Where(a => a.CorrelationType == correlationType);
        }

        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            query = query.Where(a => a.CorrelationId == correlationId);
        }

        if (from.HasValue)
        {
            query = query.Where(a => a.Timestamp >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(a => a.Timestamp <= to.Value);
        }

        query = query.OrderByDescending(a => a.Timestamp);

        var eventos = await query.ToListAsync();

        return eventos.Select(e => new AuditoriaEventoDto
        {
            Id = e.Id,
            EventType = e.EventType,
            CorrelationType = e.CorrelationType,
            CorrelationId = e.CorrelationId,
            ActorRole = e.ActorRole,
            ActorId = e.ActorId,
            Timestamp = e.Timestamp,
            PayloadJson = e.PayloadJson
        }).ToList();
    }
}
