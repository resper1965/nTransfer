using TransferenciaMateriais.Application.Services;
using TransferenciaMateriais.Domain.Entities;
using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TransferenciaMateriais.Application.UseCases.Aprovacoes;

public class ProcessarDecisaoAprovacaoUseCase
{
    private readonly ApplicationDbContext _context;
    private readonly AuditoriaService _auditoriaService;
    private readonly ILogger<ProcessarDecisaoAprovacaoUseCase> _logger;

    public ProcessarDecisaoAprovacaoUseCase(
        ApplicationDbContext context,
        AuditoriaService auditoriaService,
        ILogger<ProcessarDecisaoAprovacaoUseCase> logger)
    {
        _context = context;
        _auditoriaService = auditoriaService;
        _logger = logger;
    }

    public async Task ProcessarAsync(
        Guid aprovacaoId,
        string decisao, // "APROVADA" ou "REPROVADA"
        string? motivo = null)
    {
        var pendencia = await _context.Pendencias.FindAsync(aprovacaoId);
        if (pendencia == null)
        {
            throw new InvalidOperationException($"Aprovação com ID '{aprovacaoId}' não encontrada.");
        }

        if (pendencia.Tipo != PendenciaTipo.APROVACAO_PENDENTE && 
            pendencia.Tipo != PendenciaTipo.MEDICAO_PENDENTE)
        {
            throw new InvalidOperationException($"Pendência não é uma aprovação: {pendencia.Tipo}");
        }

        if (pendencia.Status != PendenciaStatus.ABERTA)
        {
            throw new InvalidOperationException($"Aprovação já foi processada: {pendencia.Status}");
        }

        // Validar motivo se reprovada
        if (decisao == "REPROVADA" && string.IsNullOrWhiteSpace(motivo))
        {
            throw new ArgumentException("Motivo é obrigatório para reprovação.");
        }

        // Atualizar status da pendência
        pendencia.Status = decisao == "APROVADA" 
            ? PendenciaStatus.RESOLVIDA 
            : PendenciaStatus.ABERTA; // Mantém aberta se reprovada
        pendencia.ResolvedAt = decisao == "APROVADA" ? DateTimeOffset.UtcNow : null;
        pendencia.Descricao = decisao == "REPROVADA" 
            ? $"{pendencia.Descricao} | Reprovação: {motivo}"
            : pendencia.Descricao;

        // Se for aprovação de entrega e aprovada, atualizar workflow
        if (pendencia.CorrelationType == "OS" && decisao == "APROVADA")
        {
            var os = await _context.OrdensServico.FindAsync(Guid.Parse(pendencia.CorrelationId));
            if (os != null)
            {
                if (pendencia.Tipo == PendenciaTipo.APROVACAO_PENDENTE)
                {
                    // Transição para ENTREGA_APROVADA (F3)
                    if (os.StatusWorkflow == WorkflowStatus.APROVACAO_ENTREGA_PENDENTE)
                    {
                        os.StatusWorkflow = WorkflowStatus.ENTREGA_APROVADA;
                        os.UpdatedAt = DateTimeOffset.UtcNow;

                        await _auditoriaService.RegistrarTransicaoWorkflowAsync(
                            "OS",
                            os.Id.ToString(),
                            WorkflowStatus.APROVACAO_ENTREGA_PENDENTE,
                            WorkflowStatus.ENTREGA_APROVADA,
                            "GESTOR_CONTRATO",
                            null,
                            "Aprovação de entrega"
                        );
                    }
                }
            }
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Decisão de aprovação processada: {AprovacaoId}, {Decisao}",
            aprovacaoId,
            decisao
        );
    }
}
