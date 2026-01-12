using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Domain.Entities;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TransferenciaMateriais.Application.UseCases.Anexos;

public class CriarAnexoUseCase
{
    private readonly ApplicationDbContext _context;

    public CriarAnexoUseCase(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AnexoDto> ExecuteAsync(AnexoCreateRequest request)
    {
        // Validar correlationType e correlationId
        var correlationType = request.CorrelationType.ToUpper();
        if (!new[] { "OS", "OC", "NFE", "VINCULO" }.Contains(correlationType))
        {
            throw new ArgumentException($"CorrelationType inválido: {correlationType}");
        }

        // Verificar se a entidade correlacionada existe
        bool existe = correlationType switch
        {
            "OS" => await _context.OrdensServico.AnyAsync(o => o.Id.ToString() == request.CorrelationId),
            "OC" => await _context.OrdensCompra.AnyAsync(oc => oc.Id.ToString() == request.CorrelationId),
            "NFE" => await _context.NotasFiscais.AnyAsync(n => n.ChaveAcesso == request.CorrelationId),
            "VINCULO" => await _context.Vinculos.AnyAsync(v => v.Id.ToString() == request.CorrelationId),
            _ => false
        };

        if (!existe)
        {
            throw new InvalidOperationException(
                $"{correlationType} com ID '{request.CorrelationId}' não encontrado.");
        }

        var anexo = new Anexo
        {
            Id = Guid.NewGuid(),
            CorrelationType = correlationType,
            CorrelationId = request.CorrelationId,
            Tipo = request.Tipo,
            StorageRef = request.StorageRef,
            FileName = request.FileName,
            ContentType = request.ContentType,
            SizeBytes = request.SizeBytes,
            UploadedBy = request.UploadedBy,
            UploadedAt = DateTimeOffset.UtcNow
        };

        _context.Anexos.Add(anexo);
        await _context.SaveChangesAsync();

        return new AnexoDto
        {
            Id = anexo.Id,
            CorrelationType = anexo.CorrelationType,
            CorrelationId = anexo.CorrelationId,
            Tipo = anexo.Tipo,
            StorageRef = anexo.StorageRef,
            FileName = anexo.FileName,
            ContentType = anexo.ContentType,
            SizeBytes = anexo.SizeBytes,
            UploadedBy = anexo.UploadedBy,
            UploadedAt = anexo.UploadedAt
        };
    }
}
