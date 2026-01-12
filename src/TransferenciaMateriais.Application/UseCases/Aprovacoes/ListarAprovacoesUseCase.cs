using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Domain.Entities;
using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TransferenciaMateriais.Application.UseCases.Aprovacoes;

public class ListarAprovacoesUseCase
{
    private readonly ApplicationDbContext _context;

    public ListarAprovacoesUseCase(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AprovacaoDto>> ExecuteAsync(
        string? tipo = null,
        string? status = null)
    {
        // Buscar pendências de aprovação
        var query = _context.Pendencias
            .Where(p => 
                p.Tipo == PendenciaTipo.APROVACAO_PENDENTE ||
                p.Tipo == PendenciaTipo.MEDICAO_PENDENTE)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            var statusEnum = Enum.Parse<PendenciaStatus>(status);
            query = query.Where(p => p.Status == statusEnum);
        }

        if (!string.IsNullOrWhiteSpace(tipo))
        {
            var tipoEnum = tipo == "MEDICAO" 
                ? PendenciaTipo.MEDICAO_PENDENTE 
                : PendenciaTipo.APROVACAO_PENDENTE;
            query = query.Where(p => p.Tipo == tipoEnum);
        }

        var pendencias = await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var result = new List<AprovacaoDto>();

        foreach (var pendencia in pendencias)
        {
            if (pendencia.CorrelationType == "OS")
            {
                var os = await _context.OrdensServico.FindAsync(Guid.Parse(pendencia.CorrelationId));
                if (os != null)
                {
                    var aprovacaoTipo = pendencia.Tipo == PendenciaTipo.MEDICAO_PENDENTE 
                        ? "MEDICAO" 
                        : "ENTREGA";

                    result.Add(new AprovacaoDto
                    {
                        Id = pendencia.Id,
                        Tipo = aprovacaoTipo,
                        Status = pendencia.Status.ToString(),
                        OsId = os.Id,
                        OsNumero = os.Numero,
                        Responsavel = pendencia.OwnerRole,
                        CreatedAt = pendencia.CreatedAt
                    });
                }
            }
        }

        return result;
    }
}
