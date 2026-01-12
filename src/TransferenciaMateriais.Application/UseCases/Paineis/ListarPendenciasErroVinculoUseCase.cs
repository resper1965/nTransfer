using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TransferenciaMateriais.Application.UseCases.Paineis;

public class ListarPendenciasErroVinculoUseCase
{
    private readonly ApplicationDbContext _context;

    public ListarPendenciasErroVinculoUseCase(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PendenciaErroVinculoDto>> ExecuteAsync()
    {
        var pendencias = await _context.Pendencias
            .Where(p => 
                p.Tipo == PendenciaTipo.ERRO_VINCULO &&
                p.Status == PendenciaStatus.ABERTA)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var result = new List<PendenciaErroVinculoDto>();

        foreach (var pendencia in pendencias)
        {
            if (pendencia.CorrelationType == "OS")
            {
                var os = await _context.OrdensServico.FindAsync(Guid.Parse(pendencia.CorrelationId));
                if (os != null)
                {
                    // Buscar vínculo relacionado se houver
                    var vinculo = await _context.Vinculos
                        .FirstOrDefaultAsync(v => v.OsId == os.Id && v.Status == VinculoStatus.ERRO_VINCULO);

                    result.Add(new PendenciaErroVinculoDto
                    {
                        PendenciaId = pendencia.Id,
                        OsId = os.Id,
                        OsNumero = os.Numero,
                        NfeChaveAcesso = vinculo?.NfeChaveAcesso,
                        MotivoErro = pendencia.Tipo.ToString(),
                        Descricao = pendencia.Descricao,
                        DataErro = pendencia.CreatedAt,
                        Status = pendencia.Status
                    });
                }
            }
        }

        return result;
    }
}
