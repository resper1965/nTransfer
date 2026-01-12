using Microsoft.AspNetCore.Mvc;
using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Application.UseCases.Notificacoes;
using TransferenciaMateriais.Domain.Enums;

namespace TransferenciaMateriais.Api.Controllers;

[ApiController]
[Route("notificacoes")]
[Tags("Notificacoes")]
public class NotificacoesController : ControllerBase
{
    private readonly ListarNotificacoesUseCase _listarNotificacoesUseCase;

    public NotificacoesController(ListarNotificacoesUseCase listarNotificacoesUseCase)
    {
        _listarNotificacoesUseCase = listarNotificacoesUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<List<NotificacaoDto>>> Listar(
        [FromQuery] string? correlationType,
        [FromQuery] string? correlationId,
        [FromQuery] NotificacaoStatus? status,
        [FromQuery] NotificacaoTipo? tipo)
    {
        try
        {
            var notificacoes = await _listarNotificacoesUseCase.ExecuteAsync(
                correlationType, correlationId, status, tipo);
            return Ok(notificacoes);
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Code = "BAD_REQUEST",
                Message = ex.Message,
                CorrelationId = $"NOTIFICACAO:{Guid.NewGuid()}"
            });
        }
    }
}
