using Microsoft.AspNetCore.Mvc;
using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Application.UseCases.Aprovacoes;

namespace TransferenciaMateriais.Api.Controllers;

[ApiController]
[Route("aprovacoes")]
[Tags("Aprovacoes")]
public class AprovacoesController : ControllerBase
{
    private readonly ListarAprovacoesUseCase _listarAprovacoesUseCase;
    private readonly ProcessarDecisaoAprovacaoUseCase _processarDecisaoAprovacaoUseCase;

    public AprovacoesController(
        ListarAprovacoesUseCase listarAprovacoesUseCase,
        ProcessarDecisaoAprovacaoUseCase processarDecisaoAprovacaoUseCase)
    {
        _listarAprovacoesUseCase = listarAprovacoesUseCase;
        _processarDecisaoAprovacaoUseCase = processarDecisaoAprovacaoUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<List<AprovacaoDto>>> Listar(
        [FromQuery] string? tipo,
        [FromQuery] string? status)
    {
        try
        {
            var aprovacoes = await _listarAprovacoesUseCase.ExecuteAsync(tipo, status);
            return Ok(aprovacoes);
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Code = "BAD_REQUEST",
                Message = ex.Message,
                CorrelationId = $"APROVACAO:{Guid.NewGuid()}"
            });
        }
    }

    [HttpPost("{aprovacaoId}/decisao")]
    public async Task<ActionResult> ProcessarDecisao(
        [FromRoute] Guid aprovacaoId,
        [FromBody] AprovacaoDecisaoRequest request)
    {
        try
        {
            if (request.Decisao != "APROVADA" && request.Decisao != "REPROVADA")
            {
                return UnprocessableEntity(new ValidationErrorResponseDto
                {
                    Code = "BUSINESS_RULE_VIOLATION",
                    Message = "Decisão deve ser 'APROVADA' ou 'REPROVADA'.",
                    CorrelationId = $"APROVACAO:{aprovacaoId}",
                    Violations = new List<ValidationViolationDto>
                    {
                        new()
                        {
                            Field = "decisao",
                            Rule = "INVALID_VALUE",
                            Message = "Decisão deve ser 'APROVADA' ou 'REPROVADA'."
                        }
                    }
                });
            }

            await _processarDecisaoAprovacaoUseCase.ProcessarAsync(
                aprovacaoId, 
                request.Decisao, 
                request.Motivo);

            return Ok(new { message = "Decisão processada com sucesso." });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ErrorResponseDto
            {
                Code = "NOT_FOUND",
                Message = ex.Message,
                CorrelationId = $"APROVACAO:{aprovacaoId}"
            });
        }
        catch (ArgumentException ex)
        {
            return UnprocessableEntity(new ValidationErrorResponseDto
            {
                Code = "BUSINESS_RULE_VIOLATION",
                Message = ex.Message,
                CorrelationId = $"APROVACAO:{aprovacaoId}",
                Violations = new List<ValidationViolationDto>
                {
                    new()
                    {
                        Field = "motivo",
                        Rule = "REQUIRED",
                        Message = ex.Message
                    }
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Code = "BAD_REQUEST",
                Message = ex.Message,
                CorrelationId = $"APROVACAO:{aprovacaoId}"
            });
        }
    }
}
