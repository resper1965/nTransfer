# Deploy Completo no Docker Desktop - PowerShell Script
# Execute este script no PowerShell do Windows

Write-Host "🚀 Deploy Completo no Docker Desktop - Transferência de Materiais Entre Filiais" -ForegroundColor Green
Write-Host ""

# Verificar se Docker está disponível
try {
    docker --version | Out-Null
    Write-Host "✅ Docker encontrado" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker não está instalado ou não está no PATH" -ForegroundColor Red
    Write-Host "   Instale o Docker Desktop: https://www.docker.com/products/docker-desktop" -ForegroundColor Yellow
    exit 1
}

# Verificar se Docker está rodando
try {
    docker ps | Out-Null
    Write-Host "✅ Docker está rodando" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker não está rodando" -ForegroundColor Red
    Write-Host "   Inicie o Docker Desktop e tente novamente" -ForegroundColor Yellow
    exit 1
}

# Verificar se .env.local existe
if (-not (Test-Path ".env.local")) {
    Write-Host "⚠️  Criando .env.local..." -ForegroundColor Yellow
    Copy-Item ".env.example" ".env.local"
    Write-Host "✅ .env.local criado" -ForegroundColor Green
}

Write-Host ""
Write-Host "📦 Passo 1: Parando containers existentes" -ForegroundColor Green
docker compose -f docker-compose.prod.yml down 2>$null
docker compose -f infra/docker-compose.yml down 2>$null

Write-Host ""
Write-Host "📦 Passo 2: Build da imagem Docker" -ForegroundColor Green
docker build -t transferencia-api:local .

Write-Host ""
Write-Host "📦 Passo 3: Subindo serviços auxiliares (PostgreSQL e Mailpit)" -ForegroundColor Green
docker compose -f infra/docker-compose.yml up -d

Write-Host ""
Write-Host "⏳ Aguardando PostgreSQL estar pronto..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Verificar se PostgreSQL está pronto
$maxAttempts = 30
$attempt = 0
$ready = $false

while ($attempt -lt $maxAttempts -and -not $ready) {
    $attempt++
    try {
        docker compose -f infra/docker-compose.yml exec -T postgres pg_isready -U transferencia 2>$null | Out-Null
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ PostgreSQL está pronto" -ForegroundColor Green
            $ready = $true
        }
    } catch {
        # Ignorar erros
    }
    
    if (-not $ready) {
        Write-Host "   Aguardando PostgreSQL... ($attempt/$maxAttempts)" -ForegroundColor Yellow
        Start-Sleep -Seconds 2
    }
}

Write-Host ""
Write-Host "📦 Passo 4: Subindo API" -ForegroundColor Green
docker compose -f docker-compose.prod.yml --env-file .env.local up -d

Write-Host ""
Write-Host "⏳ Aguardando API iniciar..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

Write-Host ""
Write-Host "✅ Deploy concluído!" -ForegroundColor Green
Write-Host ""
Write-Host "📊 Serviços disponíveis:" -ForegroundColor Cyan
Write-Host "   🌐 API: http://localhost:8080"
Write-Host "   🏥 Health: http://localhost:8080/health"
Write-Host "   📚 Swagger: http://localhost:8080"
Write-Host "   📧 Mailpit: http://localhost:8025"
Write-Host ""
Write-Host "📋 Comandos úteis:" -ForegroundColor Cyan
Write-Host "   Ver logs da API: docker compose -f docker-compose.prod.yml logs -f api"
Write-Host "   Status: docker compose -f docker-compose.prod.yml ps"
Write-Host "   Parar tudo: docker compose -f docker-compose.prod.yml down"
