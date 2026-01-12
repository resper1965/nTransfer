using Microsoft.EntityFrameworkCore;
using TransferenciaMateriais.Domain.Entities;

namespace TransferenciaMateriais.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<OrdemServico> OrdensServico { get; set; }
    public DbSet<OrdemCompra> OrdensCompra { get; set; }
    public DbSet<NotaFiscal> NotasFiscais { get; set; }
    public DbSet<Vinculo> Vinculos { get; set; }
    public DbSet<Pendencia> Pendencias { get; set; }
    public DbSet<Notificacao> Notificacoes { get; set; }
    public DbSet<Anexo> Anexos { get; set; }
    public DbSet<AuditoriaEvento> AuditoriaEventos { get; set; }
    public DbSet<ProcessedMessage> ProcessedMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // OS
        modelBuilder.Entity<OrdemServico>(entity =>
        {
            entity.ToTable("os");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Numero).IsUnique();
            entity.HasIndex(e => new { e.FilialDestinoId, e.StatusWorkflow });
            entity.HasIndex(e => e.FluxType);
            entity.Property(e => e.Numero).HasMaxLength(50);
            entity.Property(e => e.FilialDestinoId).HasMaxLength(50);
            entity.Property(e => e.QuantidadePlanejada).HasPrecision(18, 3);
            entity.HasCheckConstraint("CHK_os_quantidadePlanejada_pos", "quantidade_planejada > 0");
        });

        // OC
        modelBuilder.Entity<OrdemCompra>(entity =>
        {
            entity.ToTable("oc");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Numero).IsUnique();
            entity.HasIndex(e => new { e.OsId, e.Status });
            entity.Property(e => e.Numero).HasMaxLength(50);
            entity.Property(e => e.Tipo).HasMaxLength(30);
            entity.Property(e => e.Status).HasMaxLength(30);
            entity.HasOne(e => e.OrdemServico)
                .WithMany(o => o.OrdensCompra)
                .HasForeignKey(e => e.OsId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // NFe
        modelBuilder.Entity<NotaFiscal>(entity =>
        {
            entity.ToTable("nfe");
            entity.HasKey(e => e.ChaveAcesso);
            entity.HasIndex(e => new { e.Tipo, e.ValidacaoStatus });
            entity.HasIndex(e => e.DataEmissao);
            entity.Property(e => e.ChaveAcesso).HasMaxLength(60);
            entity.Property(e => e.Numero).HasMaxLength(50);
            entity.Property(e => e.Serie).HasMaxLength(20);
            entity.Property(e => e.CnpjEmitente).HasMaxLength(20);
            entity.Property(e => e.CnpjDestinatario).HasMaxLength(20);
            entity.Property(e => e.XmlRef).HasMaxLength(255);
            entity.Property(e => e.MotivoDetalhe).HasMaxLength(500);
        });

        // Vínculo
        modelBuilder.Entity<Vinculo>(entity =>
        {
            entity.ToTable("vinculo");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.OsId);
            entity.HasIndex(e => e.OcId);
            entity.HasIndex(e => e.NfeChaveAcesso);
            entity.HasIndex(e => e.Status);
            entity.Property(e => e.NfeChaveAcesso).HasMaxLength(60);
            entity.Property(e => e.DivergenciaQuantidade).HasPrecision(18, 3);
            entity.Property(e => e.Observacao).HasMaxLength(500);
            entity.HasOne(e => e.OrdemServico)
                .WithMany(o => o.Vinculos)
                .HasForeignKey(e => e.OsId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.OrdemCompra)
                .WithMany(oc => oc.Vinculos)
                .HasForeignKey(e => e.OcId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.NotaFiscal)
                .WithMany(n => n.Vinculos)
                .HasForeignKey(e => e.NfeChaveAcesso)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.OsId, e.OcId, e.NfeChaveAcesso }).IsUnique();
        });

        // Pendência
        modelBuilder.Entity<Pendencia>(entity =>
        {
            entity.ToTable("pendencia");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CorrelationType, e.CorrelationId });
            entity.HasIndex(e => new { e.Tipo, e.Status });
            entity.HasIndex(e => e.DueAt);
            entity.Property(e => e.CorrelationType).HasMaxLength(10);
            entity.Property(e => e.CorrelationId).HasMaxLength(80);
            entity.Property(e => e.Descricao).HasMaxLength(800);
            entity.Property(e => e.OwnerRole).HasMaxLength(50);
            entity.HasCheckConstraint("CHK_pendencia_status_enum", "status IN ('ABERTA', 'EM_ANDAMENTO', 'RESOLVIDA', 'CANCELADA')");
        });

        // Notificação
        modelBuilder.Entity<Notificacao>(entity =>
        {
            entity.ToTable("notificacao_email");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.Status, e.Tipo });
            entity.HasIndex(e => new { e.CorrelationType, e.CorrelationId });
            entity.Property(e => e.CorrelationType).HasMaxLength(10);
            entity.Property(e => e.CorrelationId).HasMaxLength(80);
            entity.Property(e => e.Assunto).HasMaxLength(200);
            entity.Property(e => e.ProviderMessageId).HasMaxLength(100);
            entity.Property(e => e.Erro).HasMaxLength(500);
            entity.HasCheckConstraint("CHK_notificacao_status_enum", "status IN ('ENFILEIRADA', 'ENVIADA', 'FALHOU')");
        });

        // Anexo
        modelBuilder.Entity<Anexo>(entity =>
        {
            entity.ToTable("anexo");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CorrelationType, e.CorrelationId });
            entity.HasIndex(e => e.Tipo);
            entity.Property(e => e.CorrelationType).HasMaxLength(10);
            entity.Property(e => e.CorrelationId).HasMaxLength(80);
            entity.Property(e => e.Tipo).HasMaxLength(80);
            entity.Property(e => e.StorageRef).HasMaxLength(255);
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.UploadedBy).HasMaxLength(100);
        });

        // AuditoriaEvento
        modelBuilder.Entity<AuditoriaEvento>(entity =>
        {
            entity.ToTable("auditoria_evento");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CorrelationType, e.CorrelationId, e.Timestamp });
            entity.HasIndex(e => new { e.EventType, e.Timestamp });
            entity.Property(e => e.CorrelationType).HasMaxLength(10);
            entity.Property(e => e.CorrelationId).HasMaxLength(80);
            entity.Property(e => e.ActorRole).HasMaxLength(50);
            entity.Property(e => e.ActorId).HasMaxLength(100);
        });

        // ProcessedMessage
        modelBuilder.Entity<ProcessedMessage>(entity =>
        {
            entity.ToTable("processed_message");
            entity.HasKey(e => new { e.IdempotencyKey, e.Source });
            entity.HasIndex(e => new { e.Source, e.IdempotencyKey });
            entity.Property(e => e.IdempotencyKey).HasMaxLength(120);
            entity.Property(e => e.Source).HasMaxLength(30);
            entity.Property(e => e.Result).HasMaxLength(20);
            entity.Property(e => e.Error).HasMaxLength(500);
        });
    }
}
