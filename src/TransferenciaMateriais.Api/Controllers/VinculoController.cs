using Microsoft.AspNetCore.Mvc;
using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Application.UseCases.Vinculo;

namespace TransferenciaMateriais.Api.Controllers;

[ApiController]
[Route("vinculos")]
[Tags("Vinculos")]
public class VinculoController : ControllerBase
{
    private readonly CriarVinculoUseCase _criarVinculoUseCase;
    private readonly ListarVinculosUseCase _listarVinculosUseCase;

    public VinculoController(
        CriarVinculoUseCase criarVinculoUseCase,
        ListarVinculosUseCase listarVinculosUseCase)
    {
        _criarVinculoUseCase = criarVinculoUseCase;
        _listarVinculosUseCase = listarVinculosUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<List<VinculoDto>>> Listar(
        [FromQuery] Guid? osId,
        [FromQuery] Guid? ocId,
        [FromQuery] string? nfeChaveAcesso)
    {
        try
        {
            var vinculos = await _listarVinculosUseCase.ExecuteAsync(osId, ocId, nfeChaveAcesso);
            return Ok(vinculos);
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Code = "BAD_REQUEST",
                Message = ex.Message,
                CorrelationId = $"VINCULO:{Guid.NewGuid()}"
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<VinculoDto>> Criar([FromBody] VinculoCreateRequest request)
    {
        try
        {
            var vinculo = await _criarVinculoUseCase.ExecuteAsync(request);
            return CreatedAtAction(nameof(Listar), new { osId = vinculo.OsId }, vinculo);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("já existe"))
            {
                return Conflict(new ErrorResponseDto
                {
                    Code = "CONFLICT",
                    Message = ex.Message,
                    CorrelationId = $"VINCULO:{Guid.NewGuid()}",
                    Details = new Dictionary<string, object>
                    {
                        { "constraint", "UQ_vinculo_os_oc_nfe" },
                        { "recommendedAction", "Use GET /vinculos para consultar o vínculo existente." }
                    }
                });
            }

            return UnprocessableEntity(new ValidationErrorResponseDto
            {
                Code = "BUSINESS_RULE_VIOLATION",
                Message = ex.Message,
                CorrelationId = $"VINCULO:{Guid.NewGuid()}",
                Violations = new List<ValidationViolationDto>
                {
                    new()
                    {
                        Field = "osId",
                        Rule = "OS_NOT_FOUND",
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
                CorrelationId = $"VINCULO:{Guid.NewGuid()}"
            });
        }
    }
}
