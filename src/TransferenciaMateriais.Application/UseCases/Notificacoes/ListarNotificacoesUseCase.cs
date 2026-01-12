using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Domain.Entities;
using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TransferenciaMateriais.Application.UseCases.Notificacoes;

public class ListarNotificacoesUseCase
{
    private readonly ApplicationDbContext _context;

    public ListarNotificacoesUseCase(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotificacaoDto>> ExecuteAsync(
        string? correlationType = null,
        string? correlationId = null,
        NotificacaoStatus? status = null,
        NotificacaoTipo? tipo = null)
    {
        var query = _context.Notificacoes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(correlationType))
        {
            query = query.Where(n => n.CorrelationType == correlationType);
        }

        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            query = query.Where(n => n.CorrelationId == correlationId);
        }

        if (status.HasValue)
        {
            query = query.Where(n => n.Status == status.Value);
        }

        if (tipo.HasValue)
        {
            query = query.Where(n => n.Tipo == tipo.Value);
        }

        var notificacoes = await query
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return notificacoes.Select(n => new NotificacaoDto
        {
            Id = n.Id,
            Tipo = n.Tipo,
            Status = n.Status,
            CorrelationType = n.CorrelationType,
            CorrelationId = n.CorrelationId,
            DestinatariosTo = n.DestinatariosTo,
            DestinatariosCc = n.DestinatariosCc,
            Assunto = n.Assunto,
            ProviderMessageId = n.ProviderMessageId,
            Erro = n.Erro,
            CreatedAt = n.CreatedAt,
            SentAt = n.SentAt
        }).ToList();
    }
}
