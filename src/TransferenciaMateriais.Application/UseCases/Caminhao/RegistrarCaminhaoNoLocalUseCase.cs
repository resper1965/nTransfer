using TransferenciaMateriais.Application.Services;
using TransferenciaMateriais.Domain.Entities;
using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TransferenciaMateriais.Application.UseCases.Caminhao;

/// <summary>
/// Use Case para registrar "caminhão no local" (TBD-07).
/// Estado/evento que marca quando o caminhão chega no local de origem ou destino.
/// </summary>
public class RegistrarCaminhaoNoLocalUseCase
{
    private readonly ApplicationDbContext _context;
    private readonly AuditoriaService _auditoriaService;
    private readonly ILogger<RegistrarCaminhaoNoLocalUseCase> _logger;

    public RegistrarCaminhaoNoLocalUseCase(
        ApplicationDbContext context,
        AuditoriaService auditoriaService,
        ILogger<RegistrarCaminhaoNoLocalUseCase> logger)
    {
        _context = context;
        _auditoriaService = auditoriaService;
        _logger = logger;
    }

    public async Task RegistrarAsync(
        Guid osId,
        string local, // "ORIGEM" ou "DESTINO"
        string actorRole,
        string? actorId = null)
    {
        var os = await _context.OrdensServico.FindAsync(osId);
        if (os == null)
        {
            throw new InvalidOperationException($"OS com ID '{osId}' não encontrada.");
        }

        // Registrar evento de auditoria
        var evento = new AuditoriaEvento
        {
            Id = Guid.NewGuid(),
            EventType = AuditoriaEventType.CAMINHAO_NO_LOCAL,
            CorrelationType = "OS",
            CorrelationId = os.Id.ToString(),
            ActorRole = actorRole,
            ActorId = actorId,
            Timestamp = DateTimeOffset.UtcNow,
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                osId = os.Id.ToString(),
                osNumero = os.Numero,
                local = local,
                timestamp = DateTimeOffset.UtcNow
            })
        };
        _context.AuditoriaEventos.Add(evento);

        // Se for no destino, pode disparar transição para CHEGADA_MATERIAL_DESTINO
        if (local == "DESTINO" && os.StatusWorkflow == WorkflowStatus.EM_TRANSITO)
        {
            var statusAnterior = os.StatusWorkflow;
            os.StatusWorkflow = WorkflowStatus.CHEGADA_MATERIAL_DESTINO;
            os.UpdatedAt = DateTimeOffset.UtcNow;

            await _auditoriaService.RegistrarTransicaoWorkflowAsync(
                "OS",
                os.Id.ToString(),
                statusAnterior,
                WorkflowStatus.CHEGADA_MATERIAL_DESTINO,
                actorRole,
                actorId,
                "Caminhão chegou no local de destino"
            );
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Caminhão no local registrado: OS {OsNumero}, Local {Local}",
            os.Numero,
            local
        );
    }
}
