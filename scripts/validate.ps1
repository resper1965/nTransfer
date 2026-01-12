# PowerShell script para validação (Windows)
# Valida OpenAPI, build .NET, testes e links

Write-Host "🔍 Validando OpenAPI..." -ForegroundColor Cyan

if (Get-Command swagger-cli -ErrorAction SilentlyContinue) {
    swagger-cli validate docs/contracts/openapi.yaml
} elseif (Get-Command redocly -ErrorAction SilentlyContinue) {
    redocly lint docs/contracts/openapi.yaml
} else {
    Write-Host "⚠️  swagger-cli ou redocly não encontrado. Pulando validação OpenAPI." -ForegroundColor Yellow
    Write-Host "   Instale com: npm install -g @apidevtools/swagger-cli" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "🔍 Validando build .NET..." -ForegroundColor Cyan
dotnet build --no-restore

Write-Host ""
Write-Host "🔍 Validando testes..." -ForegroundColor Cyan
dotnet test --no-build

Write-Host ""
Write-Host "✅ Validações concluídas!" -ForegroundColor Green
