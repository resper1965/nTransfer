using Microsoft.AspNetCore.Mvc;
using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Application.UseCases.OS;
using TransferenciaMateriais.Domain.Enums;

namespace TransferenciaMateriais.Api.Controllers;

[ApiController]
[Route("os")]
[Tags("OS")]
public class OSController : ControllerBase
{
    private readonly CriarOSUseCase _criarOSUseCase;
    private readonly ListarOSUseCase _listarOSUseCase;
    private readonly ObterOSPorIdUseCase _obterOSPorIdUseCase;
    private readonly AtualizarOSUseCase _atualizarOSUseCase;

    public OSController(
        CriarOSUseCase criarOSUseCase,
        ListarOSUseCase listarOSUseCase,
        ObterOSPorIdUseCase obterOSPorIdUseCase,
        AtualizarOSUseCase atualizarOSUseCase)
    {
        _criarOSUseCase = criarOSUseCase;
        _listarOSUseCase = listarOSUseCase;
        _obterOSPorIdUseCase = obterOSPorIdUseCase;
        _atualizarOSUseCase = atualizarOSUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<List<OSDto>>> Listar(
        [FromQuery] string? filialDestinoId,
        [FromQuery] WorkflowStatus? status,
        [FromQuery] FluxType? fluxType,
        [FromQuery] string? numero)
    {
        try
        {
            var osList = await _listarOSUseCase.ExecuteAsync(filialDestinoId, status, fluxType, numero);
            return Ok(osList);
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Code = "BAD_REQUEST",
                Message = ex.Message,
                CorrelationId = $"OS:{Guid.NewGuid()}"
            });
        }
    }

    [HttpGet("{osId}")]
    public async Task<ActionResult<OSDto>> ObterPorId([FromRoute] Guid osId)
    {
        var os = await _obterOSPorIdUseCase.ExecuteAsync(osId);
        if (os == null)
        {
            return NotFound(new ErrorResponseDto
            {
                Code = "NOT_FOUND",
                Message = $"OS com ID '{osId}' não encontrada.",
                CorrelationId = $"OS:{osId}"
            });
        }

        return Ok(os);
    }

    [HttpPost]
    public async Task<ActionResult<OSDto>> Criar([FromBody] OSCreateRequest request)
    {
        try
        {
            // TODO: Obter actorRole e actorId do contexto de autenticação
            var os = await _criarOSUseCase.ExecuteAsync(request, "ADM_FILIAL_ORIGEM");
            return CreatedAtAction(nameof(ObterPorId), new { osId = os.Id }, os);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ErrorResponseDto
            {
                Code = "CONFLICT",
                Message = ex.Message,
                CorrelationId = $"OS:{Guid.NewGuid()}"
            });
        }
        catch (ArgumentException ex)
        {
            return UnprocessableEntity(new ValidationErrorResponseDto
            {
                Code = "BUSINESS_RULE_VIOLATION",
                Message = ex.Message,
                CorrelationId = $"OS:{Guid.NewGuid()}",
                Violations = new List<ValidationViolationDto>
                {
                    new()
                    {
                        Field = "quantidadePlanejada",
                        Rule = "QUANTIDADE_POSITIVA",
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
                CorrelationId = $"OS:{Guid.NewGuid()}"
            });
        }
    }

    [HttpPatch("{osId}")]
    public async Task<ActionResult<OSDto>> Atualizar([FromRoute] Guid osId, [FromBody] OSUpdateRequest request)
    {
        try
        {
            var os = await _atualizarOSUseCase.ExecuteAsync(osId, request);
            if (os == null)
            {
                return NotFound(new ErrorResponseDto
                {
                    Code = "NOT_FOUND",
                    Message = $"OS com ID '{osId}' não encontrada.",
                    CorrelationId = $"OS:{osId}"
                });
            }

            return Ok(os);
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Code = "BAD_REQUEST",
                Message = ex.Message,
                CorrelationId = $"OS:{osId}"
            });
        }
    }
}
