using TransferenciaMateriais.Application.DTOs;
using TransferenciaMateriais.Domain.Entities;
using TransferenciaMateriais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TransferenciaMateriais.Application.UseCases.Anexos;

public class ListarAnexosUseCase
{
    private readonly ApplicationDbContext _context;

    public ListarAnexosUseCase(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AnexoDto>> ExecuteAsync(string correlationType, string correlationId)
    {
        var anexos = await _context.Anexos
            .Where(a => 
                a.CorrelationType == correlationType &&
                a.CorrelationId == correlationId)
            .OrderByDescending(a => a.UploadedAt)
            .ToListAsync();

        return anexos.Select(a => new AnexoDto
        {
            Id = a.Id,
            CorrelationType = a.CorrelationType,
            CorrelationId = a.CorrelationId,
            Tipo = a.Tipo,
            StorageRef = a.StorageRef,
            FileName = a.FileName,
            ContentType = a.ContentType,
            SizeBytes = a.SizeBytes,
            UploadedBy = a.UploadedBy,
            UploadedAt = a.UploadedAt
        }).ToList();
    }
}
