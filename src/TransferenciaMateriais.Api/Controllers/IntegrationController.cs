using Microsoft.AspNetCore.Mvc;
using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Application.UseCases.Integration;

namespace TransferenciaMateriais.Api.Controllers;

[ApiController]
[Route("integrations")]
[Tags("Integrations")]
public class IntegrationController : ControllerBase
{
    private readonly ProcessarNFeQiveUseCase _processarNFeQiveUseCase;

    public IntegrationController(ProcessarNFeQiveUseCase processarNFeQiveUseCase)
    {
        _processarNFeQiveUseCase = processarNFeQiveUseCase;
    }

    [HttpPost("qive/nfe-recebida")]
    public async Task<ActionResult<QiveNFeRecebidaEventDto>> ReceberNFeQive(
        [FromBody] QiveNFeRecebidaEventDto request)
    {
        try
        {
            // Validar idempotencyKey obrigatório
            if (string.IsNullOrWhiteSpace(request.IdempotencyKey))
            {
                return BadRequest(new ErrorResponseDto
                {
                    Code = "BAD_REQUEST",
                    Message = "IdempotencyKey é obrigatório.",
                    CorrelationId = $"QIVE:{Guid.NewGuid()}"
                });
            }

            var result = await _processarNFeQiveUseCase.ExecuteAsync(request);
            
            // Retornar 202 Accepted (processamento assíncrono/idempotente)
            return Accepted(result);
        }
        catch (ArgumentException ex)
        {
            return UnprocessableEntity(new ValidationErrorResponseDto
            {
                Code = "BUSINESS_RULE_VIOLATION",
                Message = ex.Message,
                CorrelationId = $"QIVE:{request.IdempotencyKey}",
                Violations = new List<ValidationViolationDto>
                {
                    new()
                    {
                        Field = "idempotencyKey",
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
                CorrelationId = $"QIVE:{request.IdempotencyKey ?? Guid.NewGuid().ToString()}"
            });
        }
    }
}
