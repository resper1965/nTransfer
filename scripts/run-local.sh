#!/bin/bash
set -e

echo "🚀 Rodar Localmente (SEM Docker) - Transferência de Materiais Entre Filiais"
echo ""

# Verificar se .NET está instalado
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK não está instalado"
    echo "   Instale: https://dotnet.microsoft.com/download"
    exit 1
fi

# Verificar se .env.local existe
if [ ! -f .env.local ]; then
    echo "⚠️  Criando .env.local..."
    cp .env.example .env.local
fi

echo "📋 Verificando pré-requisitos..."
echo ""

# Verificar se PostgreSQL está acessível
if command -v psql &> /dev/null; then
    echo "✅ psql encontrado"
else
    echo "⚠️  psql não encontrado (opcional para verificar conexão)"
fi

echo ""
echo "📦 Passo 1: Restaurar dependências"
dotnet restore

echo ""
echo "📦 Passo 2: Build do projeto"
dotnet build --no-restore

echo ""
echo "📦 Passo 3: Verificar serviços auxiliares"
echo ""
echo "⚠️  IMPORTANTE: Certifique-se de que os serviços estão rodando:"
echo "   1. PostgreSQL: Host=localhost, Port=5432"
echo "   2. Mailpit (opcional): http://localhost:8025"
echo ""
echo "   Para subir com Docker Compose:"
echo "   docker compose -f infra/docker-compose.yml up -d"
echo ""
read -p "Pressione Enter para continuar ou Ctrl+C para cancelar..."

echo ""
echo "📦 Passo 4: Rodar aplicação"
echo ""
echo "🌐 API estará disponível em:"
echo "   HTTP: http://localhost:5000"
echo "   HTTPS: https://localhost:5001"
echo "   Swagger: http://localhost:5000 (raiz)"
echo ""
echo "📋 Variáveis de ambiente serão carregadas de .env.local"
echo ""

# Carregar variáveis de ambiente do .env.local
export $(cat .env.local | grep -v '^#' | xargs)

# Rodar a aplicação
dotnet run --project src/TransferenciaMateriais.Api
