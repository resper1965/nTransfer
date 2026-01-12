using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Domain;
using TransferenciaMateriais.Domain.Entities;
using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TransferenciaMateriais.Application.UseCases.Integration;

public class ProcessarNFeQiveUseCase
{
    private readonly ApplicationDbContext _context;
    private readonly IIntegrationAdapter _integrationAdapter;
    private readonly ILogger<ProcessarNFeQiveUseCase> _logger;

    public ProcessarNFeQiveUseCase(
        ApplicationDbContext context,
        IIntegrationAdapter integrationAdapter,
        ILogger<ProcessarNFeQiveUseCase> logger)
    {
        _context = context;
        _integrationAdapter = integrationAdapter;
        _logger = logger;
    }

    public async Task<QiveNFeRecebidaEventDto> ExecuteAsync(QiveNFeRecebidaEventDto request)
    {
        // 1. Validar idempotencyKey
        if (string.IsNullOrWhiteSpace(request.IdempotencyKey))
        {
            throw new ArgumentException("IdempotencyKey é obrigatório.");
        }

        // 2. Verificar idempotência
        var processedMessage = await _context.ProcessedMessages
            .FirstOrDefaultAsync(pm => 
                pm.IdempotencyKey == request.IdempotencyKey && 
                pm.Source == "QIVE");

        if (processedMessage != null)
        {
            _logger.LogInformation(
                "Evento já processado (idempotência): {IdempotencyKey}",
                request.IdempotencyKey
            );

            // Retornar sucesso idempotente
            return request;
        }

        // 3. Validar chave de acesso NFe (44 dígitos)
        if (string.IsNullOrWhiteSpace(request.Nfe.ChaveAcesso) || 
            request.Nfe.ChaveAcesso.Length != 44 ||
            !request.Nfe.ChaveAcesso.All(char.IsDigit))
        {
            throw new ArgumentException("Chave de acesso NFe deve ter 44 dígitos numéricos.");
        }

        // 4. Persistir/atualizar NFe
        var nfe = await _context.NotasFiscais.FindAsync(request.Nfe.ChaveAcesso);
        if (nfe == null)
        {
            nfe = new NotaFiscal
            {
                ChaveAcesso = request.Nfe.ChaveAcesso,
                Tipo = Enum.Parse<NfeTipo>(request.Nfe.Tipo),
                XmlRef = request.Nfe.XmlBase64 != null 
                    ? $"storage://xml/{request.Nfe.ChaveAcesso}.xml" 
                    : null,
                ReceivedAt = DateTimeOffset.UtcNow,
                CreatedAt = DateTimeOffset.UtcNow
            };
            _context.NotasFiscais.Add(nfe);
        }
        else
        {
            // Atualizar se necessário
            if (request.Nfe.XmlBase64 != null && nfe.XmlRef == null)
            {
                nfe.XmlRef = $"storage://xml/{request.Nfe.ChaveAcesso}.xml";
            }
            nfe.UpdatedAt = DateTimeOffset.UtcNow;
        }

        // 5. Registrar ProcessedMessage
        var processed = new ProcessedMessage
        {
            IdempotencyKey = request.IdempotencyKey,
            Source = "QIVE",
            ReceivedAt = DateTimeOffset.UtcNow,
            Result = "ACCEPTED"
        };
        _context.ProcessedMessages.Add(processed);

        // 6. Registrar auditoria
        var auditoriaEvento = new AuditoriaEvento
        {
            Id = Guid.NewGuid(),
            EventType = AuditoriaEventType.NFE_XML_OBTIDO,
            CorrelationType = "NFE",
            CorrelationId = request.Nfe.ChaveAcesso,
            ActorRole = "SISTEMA",
            ActorId = "QIVE",
            Timestamp = DateTimeOffset.UtcNow,
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                idempotencyKey = request.IdempotencyKey,
                chaveAcesso = request.Nfe.ChaveAcesso,
                tipo = request.Nfe.Tipo,
                xmlRef = nfe.XmlRef
            })
        };
        _context.AuditoriaEventos.Add(auditoriaEvento);

        // 7. Se houver contexto (OS/OC), atualizar workflow
        if (request.Context != null && !string.IsNullOrWhiteSpace(request.Context.OsNumero))
        {
            var os = await _context.OrdensServico
                .FirstOrDefaultAsync(o => o.Numero == request.Context.OsNumero);

            if (os != null && os.StatusWorkflow == WorkflowStatus.NFE_EMITIDA)
            {
                os.StatusWorkflow = WorkflowStatus.XML_OBTIDO;
                os.UpdatedAt = DateTimeOffset.UtcNow;

                // Registrar transição de workflow
                var workflowEvento = new AuditoriaEvento
                {
                    Id = Guid.NewGuid(),
                    EventType = AuditoriaEventType.WORKFLOW_TRANSICAO,
                    CorrelationType = "OS",
                    CorrelationId = os.Id.ToString(),
                    ActorRole = "SISTEMA",
                    ActorId = "QIVE",
                    Timestamp = DateTimeOffset.UtcNow,
                    PayloadJson = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        fromStatus = WorkflowStatus.NFE_EMITIDA.ToString(),
                        toStatus = WorkflowStatus.XML_OBTIDO.ToString(),
                        reason = "XML obtido via Qive"
                    })
                };
                _context.AuditoriaEventos.Add(workflowEvento);
            }
        }

        // 8. Salvar todas as alterações
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "NFe processada com sucesso: {ChaveAcesso}, IdempotencyKey: {IdempotencyKey}",
            request.Nfe.ChaveAcesso,
            request.IdempotencyKey
        );

        return request;
    }
}
