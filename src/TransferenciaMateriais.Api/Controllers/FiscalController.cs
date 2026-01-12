using Microsoft.AspNetCore.Mvc;
using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Application.UseCases.Fiscal;

namespace TransferenciaMateriais.Api.Controllers;

[ApiController]
[Route("fiscal")]
[Tags("Fiscal")]
public class FiscalController : ControllerBase
{
    private readonly ValidarNFeUseCase _validarNFeUseCase;

    public FiscalController(ValidarNFeUseCase validarNFeUseCase)
    {
        _validarNFeUseCase = validarNFeUseCase;
    }

    [HttpPost("nfe/{chaveAcesso}/validacao")]
    public async Task<ActionResult<NFeValidacaoResponse>> ValidarNFe(
        [FromRoute] string chaveAcesso,
        [FromBody] NFeValidacaoRequest request)
    {
        try
        {
            var response = await _validarNFeUseCase.ExecuteAsync(chaveAcesso, request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ErrorResponseDto
            {
                Code = "NOT_FOUND",
                Message = ex.Message,
                CorrelationId = $"NFE:{chaveAcesso}"
            });
        }
        catch (ArgumentException ex)
        {
            return UnprocessableEntity(new ValidationErrorResponseDto
            {
                Code = "BUSINESS_RULE_VIOLATION",
                Message = ex.Message,
                CorrelationId = $"NFE:{chaveAcesso}",
                Violations = new List<ValidationViolationDto>
                {
                    new()
                    {
                        Field = "motivo",
                        Rule = "MOTIVO_OBRIGATORIO",
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
                CorrelationId = $"NFE:{chaveAcesso}"
            });
        }
    }
}
