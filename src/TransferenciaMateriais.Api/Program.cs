using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TransferenciaMateriais.Infrastructure.Data;
using TransferenciaMateriais.Application.UseCases.OS;
using TransferenciaMateriais.Application.UseCases.Vinculo;
using TransferenciaMateriais.Application.UseCases.Fiscal;
using TransferenciaMateriais.Application.UseCases.Auditoria;
using TransferenciaMateriais.Application.UseCases.Integration;
using TransferenciaMateriais.Application.UseCases.Notification;
using TransferenciaMateriais.Application.Services;
using TransferenciaMateriais.Domain;
using TransferenciaMateriais.Infrastructure.Integration;
using TransferenciaMateriais.Infrastructure.Email;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Database=transferencia_materiais;Username=transferencia;Password=transferencia";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register Use Cases
// Register Use Cases
builder.Services.AddScoped<CriarOSUseCase>();
builder.Services.AddScoped<ListarOSUseCase>();
builder.Services.AddScoped<ObterOSPorIdUseCase>();
builder.Services.AddScoped<AtualizarOSUseCase>();
builder.Services.AddScoped<CriarVinculoUseCase>();
builder.Services.AddScoped<ListarVinculosUseCase>();
builder.Services.AddScoped<ValidarNFeUseCase>();
builder.Services.AddScoped<ListarAuditoriaUseCase>();
builder.Services.AddScoped<ProcessarNFeQiveUseCase>();
builder.Services.AddScoped<EnviarNotificacaoUseCase>();

// Register Services
builder.Services.AddScoped<NotificationService>();

// Register Integration Adapter (stub até TBD-01)
builder.Services.AddScoped<IIntegrationAdapter, StubIntegrationAdapter>();

// Register Email Sender (stub para desenvolvimento)
builder.Services.AddScoped<IEmailSender, StubEmailSender>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Transferência de Materiais Entre Filiais",
        Version = "0.2.0",
        Description = "API para orquestração de workflow de transferência de materiais entre filiais"
    });

    // Incluir comentários XML se existirem
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Transferência de Materiais v0.2.0");
        c.RoutePrefix = string.Empty; // Swagger na raiz em dev
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
