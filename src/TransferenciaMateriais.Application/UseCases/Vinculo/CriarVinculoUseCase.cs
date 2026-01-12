using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Application.Services;
using TransferenciaMateriais.Domain.Entities;
using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TransferenciaMateriais.Application.UseCases.Vinculo;

public class CriarVinculoUseCase
{
    private readonly ApplicationDbContext _context;
    private readonly AuditoriaService _auditoriaService;

    public CriarVinculoUseCase(
        ApplicationDbContext context,
        AuditoriaService auditoriaService)
    {
        _context = context;
        _auditoriaService = auditoriaService;
    }

    public async Task<VinculoDto> ExecuteAsync(VinculoCreateRequest request)
    {
        // Validação: OS existe
        var os = await _context.OrdensServico.FindAsync(request.OsId);
        if (os == null)
        {
            throw new InvalidOperationException($"OS com ID '{request.OsId}' não encontrada.");
        }

        // Validação: NFe existe
        var nfe = await _context.NotasFiscais.FindAsync(request.NfeChaveAcesso);
        if (nfe == null)
        {
            throw new InvalidOperationException($"NFe com chave '{request.NfeChaveAcesso}' não encontrada.");
        }

        // Validação: OC existe (se fornecida)
        if (request.OcId.HasValue)
        {
            var oc = await _context.OrdensCompra.FindAsync(request.OcId.Value);
            if (oc == null)
            {
                throw new InvalidOperationException($"OC com ID '{request.OcId}' não encontrada.");
            }
        }

        // Validação: idempotência (unique constraint)
        var existe = await _context.Vinculos.AnyAsync(v =>
            v.OsId == request.OsId &&
            v.OcId == request.OcId &&
            v.NfeChaveAcesso == request.NfeChaveAcesso);

        if (existe)
        {
            throw new InvalidOperationException("Vínculo já existe para esta combinação OS/OC/NFe.");
        }

        var vinculo = new Domain.Entities.Vinculo
        {
            Id = Guid.NewGuid(),
            OsId = request.OsId,
            OcId = request.OcId,
            NfeChaveAcesso = request.NfeChaveAcesso,
            Status = VinculoStatus.CRIADO,
            DivergenciaQuantidade = request.DivergenciaQuantidade,
            Observacao = request.Observacao,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _context.Vinculos.Add(vinculo);
        await _context.SaveChangesAsync();

        // Registrar auditoria
        await _auditoriaService.RegistrarVinculoCriadoAsync(vinculo, "ADM_FILIAL_ORIGEM", null);

        return new VinculoDto
        {
            Id = vinculo.Id,
            OsId = vinculo.OsId,
            OcId = vinculo.OcId,
            NfeChaveAcesso = vinculo.NfeChaveAcesso,
            Status = vinculo.Status,
            DivergenciaQuantidade = vinculo.DivergenciaQuantidade,
            Observacao = vinculo.Observacao,
            CreatedAt = vinculo.CreatedAt
        };
    }
}
