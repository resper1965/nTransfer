using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TransferenciaMateriais.Application.UseCases.Paineis;

public class ListarAprovacoesPendentesUseCase
{
    private readonly ApplicationDbContext _context;

    public ListarAprovacoesPendentesUseCase(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AprovacaoPendenteDto>> ExecuteAsync(
        string? tipo = null, // "ENTREGA", "MEDICAO", "VINCULO"
        string? status = null, // "PENDENTE", "APROVADA", "REPROVADA"
        string? filialDestinoId = null)
    {
        // Buscar pendências de aprovação
        var query = _context.Pendencias
            .Where(p => p.Tipo == PendenciaTipo.APROVACAO_PENDENTE ||
                       p.Tipo == PendenciaTipo.MEDICAO_PENDENTE)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            var statusEnum = Enum.Parse<PendenciaStatus>(status);
            query = query.Where(p => p.Status == statusEnum);
        }
        else
        {
            // Por padrão, mostrar apenas pendentes
            query = query.Where(p => p.Status == PendenciaStatus.ABERTA);
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

        var result = new List<AprovacaoPendenteDto>();

        foreach (var pendencia in pendencias)
        {
            if (pendencia.CorrelationType == "OS")
            {
                var os = await _context.OrdensServico.FindAsync(Guid.Parse(pendencia.CorrelationId));
                if (os != null)
                {
                    // Filtrar por filial se especificado
                    if (!string.IsNullOrWhiteSpace(filialDestinoId) && 
                        os.FilialDestinoId != filialDestinoId)
                    {
                        continue;
                    }

                    var aprovacaoTipo = pendencia.Tipo == PendenciaTipo.MEDICAO_PENDENTE 
                        ? "MEDICAO" 
                        : "ENTREGA";

                    result.Add(new AprovacaoPendenteDto
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
