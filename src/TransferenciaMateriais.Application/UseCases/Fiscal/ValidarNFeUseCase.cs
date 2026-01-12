using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Application.Services;
using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TransferenciaMateriais.Application.UseCases.Fiscal;

public class ValidarNFeUseCase
{
    private readonly ApplicationDbContext _context;
    private readonly AuditoriaService _auditoriaService;

    public ValidarNFeUseCase(
        ApplicationDbContext context,
        AuditoriaService auditoriaService)
    {
        _context = context;
        _auditoriaService = auditoriaService;
    }

    public async Task<NFeValidacaoResponse> ExecuteAsync(string chaveAcesso, NFeValidacaoRequest request)
    {
        var nfe = await _context.NotasFiscais.FindAsync(chaveAcesso);
        if (nfe == null)
        {
            throw new InvalidOperationException($"NFe com chave '{chaveAcesso}' não encontrada.");
        }

        if (request.Decisao == "CORRETA")
        {
            nfe.ValidacaoStatus = NfeValidacaoStatus.CORRETA;
            nfe.MotivoCategoria = null;
            nfe.MotivoDetalhe = null;
        }
        else if (request.Decisao == "INCORRETA")
        {
            if (request.Motivo == null)
            {
                throw new ArgumentException("Motivo é obrigatório quando NFe é marcada como incorreta.");
            }

            nfe.ValidacaoStatus = NfeValidacaoStatus.INCORRETA;
            nfe.MotivoCategoria = request.Motivo.Categoria;
            nfe.MotivoDetalhe = request.Motivo.Detalhe;
        }
        else
        {
            throw new ArgumentException("Decisão deve ser 'CORRETA' ou 'INCORRETA'.");
        }

        nfe.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();

        // Registrar auditoria
        await _auditoriaService.RegistrarValidacaoFiscalAsync(
            nfe,
            nfe.ValidacaoStatus,
            "FISCAL", // TODO: obter do contexto de autenticação
            null,
            request.Motivo?.Categoria.ToString(),
            request.Motivo?.Detalhe
        );

        return new NFeValidacaoResponse
        {
            ChaveAcesso = nfe.ChaveAcesso,
            ValidacaoStatus = nfe.ValidacaoStatus,
            MotivoCategoria = nfe.MotivoCategoria?.ToString(),
            MotivoDetalhe = nfe.MotivoDetalhe
        };
    }
}
