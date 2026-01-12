using Microsoft.AspNetCore.Mvc;
using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Application.UseCases.Pendencias;
using TransferenciaMateriais.Domain.Enums;

namespace TransferenciaMateriais.Api.Controllers;

[ApiController]
[Route("pendencias")]
[Tags("Pendencias")]
public class PendenciasController : ControllerBase
{
    private readonly ListarPendenciasUseCase _listarPendenciasUseCase;

    public PendenciasController(ListarPendenciasUseCase listarPendenciasUseCase)
    {
        _listarPendenciasUseCase = listarPendenciasUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<List<PendenciaDto>>> Listar(
        [FromQuery] string? correlationType,
        [FromQuery] string? correlationId,
        [FromQuery] PendenciaStatus? status,
        [FromQuery] PendenciaTipo? tipo)
    {
        try
        {
            var pendencias = await _listarPendenciasUseCase.ExecuteAsync(
                correlationType, correlationId, status, tipo);
            return Ok(pendencias);
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Code = "BAD_REQUEST",
                Message = ex.Message,
                CorrelationId = $"PENDENCIA:{Guid.NewGuid()}"
            });
        }
    }
}
