using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransferenciaMateriais.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "anexo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CorrelationType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CorrelationId = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Tipo = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    StorageRef = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    UploadedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UploadedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_anexo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "auditoria_evento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    CorrelationType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CorrelationId = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    ActorRole = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ActorId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PayloadJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auditoria_evento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "nfe",
                columns: table => new
                {
                    ChaveAcesso = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Numero = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Serie = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CnpjEmitente = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CnpjDestinatario = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DataEmissao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    XmlRef = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ValidacaoStatus = table.Column<int>(type: "integer", nullable: false),
                    MotivoCategoria = table.Column<int>(type: "integer", nullable: true),
                    MotivoDetalhe = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ReceivedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nfe", x => x.ChaveAcesso);
                });

            migrationBuilder.CreateTable(
                name: "notificacao_email",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CorrelationType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CorrelationId = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    DestinatariosTo = table.Column<string>(type: "text", nullable: false),
                    DestinatariosCc = table.Column<string>(type: "text", nullable: true),
                    Assunto = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Corpo = table.Column<string>(type: "text", nullable: false),
                    ProviderMessageId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Erro = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notificacao_email", x => x.Id);
                    table.CheckConstraint("CHK_notificacao_status_enum", "status IN ('ENFILEIRADA', 'ENVIADA', 'FALHOU')");
                });

            migrationBuilder.CreateTable(
                name: "os",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Numero = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FluxType = table.Column<int>(type: "integer", nullable: false),
                    FilialDestinoId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    QuantidadePlanejada = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    DataEstimadaEntrega = table.Column<DateOnly>(type: "date", nullable: true),
                    StatusWorkflow = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_os", x => x.Id);
                    table.CheckConstraint("CHK_os_quantidadePlanejada_pos", "quantidade_planejada > 0");
                });

            migrationBuilder.CreateTable(
                name: "pendencia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CorrelationType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CorrelationId = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(800)", maxLength: 800, nullable: false),
                    OwnerRole = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DueAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ResolvedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pendencia", x => x.Id);
                    table.CheckConstraint("CHK_pendencia_status_enum", "status IN ('ABERTA', 'EM_ANDAMENTO', 'RESOLVIDA', 'CANCELADA')");
                });

            migrationBuilder.CreateTable(
                name: "processed_message",
                columns: table => new
                {
                    IdempotencyKey = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Source = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ReceivedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ProcessedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Result = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Error = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_processed_message", x => new { x.IdempotencyKey, x.Source });
                });

            migrationBuilder.CreateTable(
                name: "oc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Numero = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OsId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oc", x => x.Id);
                    table.ForeignKey(
                        name: "FK_oc_os_OsId",
                        column: x => x.OsId,
                        principalTable: "os",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vinculo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OsId = table.Column<Guid>(type: "uuid", nullable: false),
                    OcId = table.Column<Guid>(type: "uuid", nullable: true),
                    NfeChaveAcesso = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DivergenciaQuantidade = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    Observacao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vinculo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vinculo_nfe_NfeChaveAcesso",
                        column: x => x.NfeChaveAcesso,
                        principalTable: "nfe",
                        principalColumn: "ChaveAcesso",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vinculo_oc_OcId",
                        column: x => x.OcId,
                        principalTable: "oc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vinculo_os_OsId",
                        column: x => x.OsId,
                        principalTable: "os",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_anexo_CorrelationType_CorrelationId",
                table: "anexo",
                columns: new[] { "CorrelationType", "CorrelationId" });

            migrationBuilder.CreateIndex(
                name: "IX_anexo_Tipo",
                table: "anexo",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_evento_CorrelationType_CorrelationId_Timestamp",
                table: "auditoria_evento",
                columns: new[] { "CorrelationType", "CorrelationId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_evento_EventType_Timestamp",
                table: "auditoria_evento",
                columns: new[] { "EventType", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_nfe_DataEmissao",
                table: "nfe",
                column: "DataEmissao");

            migrationBuilder.CreateIndex(
                name: "IX_nfe_Tipo_ValidacaoStatus",
                table: "nfe",
                columns: new[] { "Tipo", "ValidacaoStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_notificacao_email_CorrelationType_CorrelationId",
                table: "notificacao_email",
                columns: new[] { "CorrelationType", "CorrelationId" });

            migrationBuilder.CreateIndex(
                name: "IX_notificacao_email_Status_Tipo",
                table: "notificacao_email",
                columns: new[] { "Status", "Tipo" });

            migrationBuilder.CreateIndex(
                name: "IX_oc_Numero",
                table: "oc",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_oc_OsId_Status",
                table: "oc",
                columns: new[] { "OsId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_os_FilialDestinoId_StatusWorkflow",
                table: "os",
                columns: new[] { "FilialDestinoId", "StatusWorkflow" });

            migrationBuilder.CreateIndex(
                name: "IX_os_FluxType",
                table: "os",
                column: "FluxType");

            migrationBuilder.CreateIndex(
                name: "IX_os_Numero",
                table: "os",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pendencia_CorrelationType_CorrelationId",
                table: "pendencia",
                columns: new[] { "CorrelationType", "CorrelationId" });

            migrationBuilder.CreateIndex(
                name: "IX_pendencia_DueAt",
                table: "pendencia",
                column: "DueAt");

            migrationBuilder.CreateIndex(
                name: "IX_pendencia_Tipo_Status",
                table: "pendencia",
                columns: new[] { "Tipo", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_processed_message_Source_IdempotencyKey",
                table: "processed_message",
                columns: new[] { "Source", "IdempotencyKey" });

            migrationBuilder.CreateIndex(
                name: "IX_vinculo_NfeChaveAcesso",
                table: "vinculo",
                column: "NfeChaveAcesso");

            migrationBuilder.CreateIndex(
                name: "IX_vinculo_OcId",
                table: "vinculo",
                column: "OcId");

            migrationBuilder.CreateIndex(
                name: "IX_vinculo_OsId",
                table: "vinculo",
                column: "OsId");

            migrationBuilder.CreateIndex(
                name: "IX_vinculo_OsId_OcId_NfeChaveAcesso",
                table: "vinculo",
                columns: new[] { "OsId", "OcId", "NfeChaveAcesso" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vinculo_Status",
                table: "vinculo",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "anexo");

            migrationBuilder.DropTable(
                name: "auditoria_evento");

            migrationBuilder.DropTable(
                name: "notificacao_email");

            migrationBuilder.DropTable(
                name: "pendencia");

            migrationBuilder.DropTable(
                name: "processed_message");

            migrationBuilder.DropTable(
                name: "vinculo");

            migrationBuilder.DropTable(
                name: "nfe");

            migrationBuilder.DropTable(
                name: "oc");

            migrationBuilder.DropTable(
                name: "os");
        }
    }
}
