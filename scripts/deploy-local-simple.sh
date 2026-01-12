#!/bin/bash
set -e

echo "🚀 Deploy Local Simples - Transferência de Materiais Entre Filiais"
echo ""

# Verificar se Docker está instalado
if ! command -v docker &> /dev/null; then
    echo "❌ Docker não está instalado"
    exit 1
fi

# Verificar se .env.local existe
if [ ! -f .env.local ]; then
    echo "⚠️  Criando .env.local..."
    cp .env.example .env.local
fi

# Carregar variáveis
export $(cat .env.local | grep -v '^#' | xargs)

echo "📦 Build da imagem..."
docker build -t transferencia-api:local .

echo ""
echo "📦 Subindo serviços auxiliares..."
docker compose -f infra/docker-compose.yml up -d postgres mailpit

echo ""
echo "⏳ Aguardando PostgreSQL..."
sleep 5

echo ""
echo "📦 Subindo API..."
docker compose -f docker-compose.prod.yml --env-file .env.local up -d

echo ""
echo "✅ Deploy concluído!"
echo ""
echo "📊 Serviços:"
echo "   🌐 API: http://localhost:8080"
echo "   🏥 Health: http://localhost:8080/health"
echo "   📧 Mailpit: http://localhost:8025"
echo ""
echo "📋 Comandos:"
echo "   Logs: docker compose -f docker-compose.prod.yml logs -f api"
echo "   Parar: docker compose -f docker-compose.prod.yml down"
