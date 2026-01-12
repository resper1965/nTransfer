#!/bin/bash
set -e

echo "🚀 Deploy Local - Transferência de Materiais Entre Filiais"
echo ""

# Cores para output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Verificar se Docker está instalado
if ! command -v docker &> /dev/null; then
    echo -e "${RED}❌ Docker não está instalado${NC}"
    echo "   Instale Docker: https://docs.docker.com/get-docker/"
    exit 1
fi

# Verificar se Docker Compose está instalado
if ! command -v docker compose &> /dev/null; then
    echo -e "${RED}❌ Docker Compose não está instalado${NC}"
    echo "   Instale Docker Compose: https://docs.docker.com/compose/install/"
    exit 1
fi

# Verificar se .env.local existe
if [ ! -f .env.local ]; then
    echo -e "${YELLOW}⚠️  Arquivo .env.local não encontrado${NC}"
    echo "   Criando .env.local a partir de .env.example..."
    cp .env.example .env.local
    echo -e "${GREEN}✅ .env.local criado${NC}"
    echo -e "${YELLOW}   Edite .env.local com suas configurações antes de continuar${NC}"
    exit 1
fi

# Carregar variáveis de ambiente
export $(cat .env.local | grep -v '^#' | xargs)

echo -e "${GREEN}📦 Passo 1: Build da imagem Docker${NC}"
docker build -t transferencia-api:local .

echo ""
echo -e "${GREEN}📦 Passo 2: Subindo serviços auxiliares (PostgreSQL)${NC}"
docker compose -f infra/docker-compose.yml up -d postgres

echo ""
echo -e "${YELLOW}⏳ Aguardando PostgreSQL estar pronto...${NC}"
sleep 5

# Verificar se PostgreSQL está pronto
until docker compose -f infra/docker-compose.yml exec -T postgres pg_isready -U transferencia > /dev/null 2>&1; do
    echo -e "${YELLOW}   Aguardando PostgreSQL...${NC}"
    sleep 2
done
echo -e "${GREEN}✅ PostgreSQL está pronto${NC}"

echo ""
echo -e "${GREEN}📦 Passo 3: Aplicando migrations${NC}"
# Migrations serão aplicadas automaticamente quando a API subir
# Ou podem ser executadas manualmente depois:
echo -e "${YELLOW}   Migrations serão aplicadas quando a API iniciar${NC}"
echo "   Para aplicar manualmente depois:"
echo "   docker compose -f docker-compose.prod.yml exec api dotnet ef database update"

echo ""
echo -e "${GREEN}📦 Passo 4: Subindo API e serviços${NC}"
# Usar docker compose com env file
export $(cat .env.local | grep -v '^#' | xargs)
docker compose -f docker-compose.prod.yml up -d

echo ""
echo -e "${GREEN}✅ Deploy local concluído!${NC}"
echo ""
echo "📊 Serviços disponíveis:"
echo "   🌐 API: http://localhost:8080"
echo "   🏥 Health: http://localhost:8080/health"
echo "   📧 Mailpit: http://localhost:8025"
echo ""
echo "📋 Comandos úteis:"
echo "   Ver logs: docker compose -f docker-compose.prod.yml logs -f api"
echo "   Parar: docker compose -f docker-compose.prod.yml down"
echo "   Status: docker compose -f docker-compose.prod.yml ps"
echo ""
