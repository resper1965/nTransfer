using Microsoft.AspNetCore.Mvc;
using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Application.UseCases.Paineis;

namespace TransferenciaMateriais.Api.Controllers;

[ApiController]
[Route("paineis")]
[Tags("Paineis")]
public class PaineisController : ControllerBase
{
    private readonly ListarOCPendenteEntregaFuturaUseCase _listarOCPendenteUseCase;
    private readonly ListarPendenciasErroVinculoUseCase _listarPendenciasErroVinculoUseCase;
    private readonly ListarAprovacoesPendentesUseCase _listarAprovacoesPendentesUseCase;

    public PaineisController(
        ListarOCPendenteEntregaFuturaUseCase listarOCPendenteUseCase,
        ListarPendenciasErroVinculoUseCase listarPendenciasErroVinculoUseCase,
        ListarAprovacoesPendentesUseCase listarAprovacoesPendentesUseCase)
    {
        _listarOCPendenteUseCase = listarOCPendenteUseCase;
        _listarPendenciasErroVinculoUseCase = listarPendenciasErroVinculoUseCase;
        _listarAprovacoesPendentesUseCase = listarAprovacoesPendentesUseCase;
    }

    [HttpGet("oc-pendente-entrega-futura")]
    public async Task<ActionResult<List<OCPendenteDto>>> ListarOCPendenteEntregaFutura(
        [FromQuery] string? filialDestinoId,
        [FromQuery] string? tipo,
        [FromQuery] string? status,
        [FromQuery] DateOnly? dataEstimadaFrom,
        [FromQuery] DateOnly? dataEstimadaTo)
    {
        try
        {
            var ocs = await _listarOCPendenteUseCase.ExecuteAsync(
                filialDestinoId, tipo, status, dataEstimadaFrom, dataEstimadaTo);

            return Ok(new { items = ocs });
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Code = "BAD_REQUEST",
                Message = ex.Message,
                CorrelationId = $"PAINEL:{Guid.NewGuid()}"
            });
        }
    }

    [HttpGet("pendencias-erro-vinculo")]
    public async Task<ActionResult<List<PendenciaErroVinculoDto>>> ListarPendenciasErroVinculo()
    {
        try
        {
            var pendencias = await _listarPendenciasErroVinculoUseCase.ExecuteAsync();
            return Ok(pendencias);
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Code = "BAD_REQUEST",
                Message = ex.Message,
                CorrelationId = $"PAINEL:{Guid.NewGuid()}"
            });
        }
    }

    [HttpGet("aprovacoes-pendentes")]
    public async Task<ActionResult<List<AprovacaoPendenteDto>>> ListarAprovacoesPendentes(
        [FromQuery] string? tipo,
        [FromQuery] string? status,
        [FromQuery] string? filialDestinoId)
    {
        try
        {
            var aprovacoes = await _listarAprovacoesPendentesUseCase.ExecuteAsync(tipo, status, filialDestinoId);
            return Ok(aprovacoes);
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Code = "BAD_REQUEST",
                Message = ex.Message,
                CorrelationId = $"PAINEL:{Guid.NewGuid()}"
            });
        }
    }
}
