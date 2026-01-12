using Microsoft.AspNetCore.Mvc;
using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Application.UseCases.Anexos;

namespace TransferenciaMateriais.Api.Controllers;

[ApiController]
[Route("anexos")]
[Tags("Anexos")]
public class AnexosController : ControllerBase
{
    private readonly ListarAnexosUseCase _listarAnexosUseCase;
    private readonly CriarAnexoUseCase _criarAnexoUseCase;

    public AnexosController(
        ListarAnexosUseCase listarAnexosUseCase,
        CriarAnexoUseCase criarAnexoUseCase)
    {
        _listarAnexosUseCase = listarAnexosUseCase;
        _criarAnexoUseCase = criarAnexoUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<List<AnexoDto>>> Listar(
        [FromQuery] string correlationType,
        [FromQuery] string correlationId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(correlationType) || string.IsNullOrWhiteSpace(correlationId))
            {
                return BadRequest(new ErrorResponseDto
                {
                    Code = "BAD_REQUEST",
                    Message = "correlationType e correlationId são obrigatórios.",
                    CorrelationId = $"ANEXO:{Guid.NewGuid()}"
                });
            }

            var anexos = await _listarAnexosUseCase.ExecuteAsync(correlationType, correlationId);
            return Ok(anexos);
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Code = "BAD_REQUEST",
                Message = ex.Message,
                CorrelationId = $"ANEXO:{Guid.NewGuid()}"
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<AnexoDto>> Criar([FromBody] AnexoCreateRequest request)
    {
        try
        {
            var anexo = await _criarAnexoUseCase.ExecuteAsync(request);
            return CreatedAtAction(nameof(Listar), new { 
                correlationType = anexo.CorrelationType, 
                correlationId = anexo.CorrelationId 
            }, anexo);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ErrorResponseDto
            {
                Code = "NOT_FOUND",
                Message = ex.Message,
                CorrelationId = $"ANEXO:{Guid.NewGuid()}"
            });
        }
        catch (ArgumentException ex)
        {
            return UnprocessableEntity(new ValidationErrorResponseDto
            {
                Code = "BUSINESS_RULE_VIOLATION",
                Message = ex.Message,
                CorrelationId = $"ANEXO:{Guid.NewGuid()}",
                Violations = new List<ValidationViolationDto>
                {
                    new()
                    {
                        Field = "correlationType",
                        Rule = "INVALID_VALUE",
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
                CorrelationId = $"ANEXO:{Guid.NewGuid()}"
            });
        }
    }
}
