using Microsoft.AspNetCore.Mvc;
using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Application.UseCases.Auditoria;

namespace TransferenciaMateriais.Api.Controllers;

[ApiController]
[Route("auditoria")]
[Tags("Auditoria")]
public class AuditoriaController : ControllerBase
{
    private readonly ListarAuditoriaUseCase _listarAuditoriaUseCase;

    public AuditoriaController(ListarAuditoriaUseCase listarAuditoriaUseCase)
    {
        _listarAuditoriaUseCase = listarAuditoriaUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuditoriaEventoDto>>> Listar(
        [FromQuery] string? correlationType,
        [FromQuery] string? correlationId,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to)
    {
        try
        {
            var eventos = await _listarAuditoriaUseCase.ExecuteAsync(correlationType, correlationId, from, to);
            return Ok(eventos);
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Code = "BAD_REQUEST",
                Message = ex.Message,
                CorrelationId = $"AUDITORIA:{Guid.NewGuid()}"
            });
        }
    }
}
