using TransferenciaMateriais.Domain.Entities;
using TransferenciaMateriais.Domain.Enums;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TransferenciaMateriais.Application.UseCases.Anexos;

/// <summary>
/// Use Case para validar anexos obrigatórios antes de concluir entrada no destino.
/// </summary>
public class ValidarAnexosObrigatoriosUseCase
{
    private readonly ApplicationDbContext _context;

    public ValidarAnexosObrigatoriosUseCase(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ValidacaoAnexosResult> ExecuteAsync(Guid osId)
    {
        var os = await _context.OrdensServico.FindAsync(osId);
        if (os == null)
        {
            throw new InvalidOperationException($"OS com ID '{osId}' não encontrada.");
        }

        // Verificar se está em estado que exige anexo
        var estadosQueExigemAnexo = new[]
        {
            WorkflowStatus.ENTRADA_DESTINO_PENDENTE_ANEXO,
            WorkflowStatus.CHEGADA_MATERIAL_DESTINO
        };

        if (!estadosQueExigemAnexo.Contains(os.StatusWorkflow))
        {
            return new ValidacaoAnexosResult
            {
                Valido = true,
                Mensagem = "Estado atual não exige anexo obrigatório."
            };
        }

        // Buscar anexos obrigatórios para entrada no destino
        var anexosObrigatorios = new[] { "NFE_ASSINADA_CONFERIDA" };
        
        var anexos = await _context.Anexos
            .Where(a => 
                a.CorrelationType == "OS" &&
                a.CorrelationId == osId.ToString() &&
                anexosObrigatorios.Contains(a.Tipo))
            .ToListAsync();

        if (anexos.Count == 0)
        {
            return new ValidacaoAnexosResult
            {
                Valido = false,
                Mensagem = "É obrigatório anexar ao menos um documento antes de concluir a entrada no destino.",
                AnexosObrigatorios = anexosObrigatorios,
                AnexosEncontrados = Array.Empty<string>()
            };
        }

        return new ValidacaoAnexosResult
        {
            Valido = true,
            Mensagem = "Anexos obrigatórios presentes.",
            AnexosObrigatorios = anexosObrigatorios,
            AnexosEncontrados = anexos.Select(a => a.Tipo).ToArray()
        };
    }
}

public class ValidacaoAnexosResult
{
    public bool Valido { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public string[] AnexosObrigatorios { get; set; } = Array.Empty<string>();
    public string[] AnexosEncontrados { get; set; } = Array.Empty<string>();
}
