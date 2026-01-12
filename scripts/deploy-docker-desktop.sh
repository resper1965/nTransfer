#!/bin/bash
set -e

echo "🚀 Deploy Completo no Docker Desktop - Transferência de Materiais Entre Filiais"
echo ""

# Cores
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

# Verificar se Docker está disponível
if ! command -v docker &> /dev/null; then
    echo -e "${RED}❌ Docker não está no PATH${NC}"
    echo "   Certifique-se de que o Docker Desktop está rodando"
    echo "   E que o WSL está configurado para usar o Docker Desktop"
    exit 1
fi

# Verificar se Docker está rodando
if ! docker ps &> /dev/null; then
    echo -e "${RED}❌ Docker não está rodando${NC}"
    echo "   Inicie o Docker Desktop e tente novamente"
    exit 1
fi

# Verificar se .env.local existe
if [ ! -f .env.local ]; then
    echo -e "${YELLOW}⚠️  Criando .env.local...${NC}"
    cp .env.example .env.local
    echo -e "${GREEN}✅ .env.local criado${NC}"
fi

echo -e "${GREEN}📦 Passo 1: Parando containers existentes${NC}"
docker compose -f docker-compose.prod.yml down 2>/dev/null || true
docker compose -f infra/docker-compose.yml down 2>/dev/null || true

echo ""
echo -e "${GREEN}📦 Passo 2: Build da imagem Docker${NC}"
docker build -t transferencia-api:local .

echo ""
echo -e "${GREEN}📦 Passo 3: Subindo serviços auxiliares (PostgreSQL e Mailpit)${NC}"
docker compose -f infra/docker-compose.yml up -d

echo ""
echo -e "${YELLOW}⏳ Aguardando PostgreSQL estar pronto...${NC}"
sleep 5

# Verificar se PostgreSQL está pronto
for i in {1..30}; do
    if docker compose -f infra/docker-compose.yml exec -T postgres pg_isready -U transferencia > /dev/null 2>&1; then
        echo -e "${GREEN}✅ PostgreSQL está pronto${NC}"
        break
    fi
    if [ $i -eq 30 ]; then
        echo -e "${YELLOW}⚠️  PostgreSQL ainda não está pronto, mas continuando...${NC}"
    else
        echo -e "${YELLOW}   Aguardando PostgreSQL... ($i/30)${NC}"
        sleep 2
    fi
done

echo ""
echo -e "${GREEN}📦 Passo 4: Subindo API${NC}"
docker compose -f docker-compose.prod.yml --env-file .env.local up -d

echo ""
echo -e "${YELLOW}⏳ Aguardando API iniciar...${NC}"
sleep 5

echo ""
echo -e "${GREEN}✅ Deploy concluído!${NC}"
echo ""
echo "📊 Serviços disponíveis:"
echo "   🌐 API: http://localhost:8080"
echo "   🏥 Health: http://localhost:8080/health"
echo "   📚 Swagger: http://localhost:8080"
echo "   📧 Mailpit: http://localhost:8025"
echo ""
echo "📋 Comandos úteis:"
echo "   Ver logs da API: docker compose -f docker-compose.prod.yml logs -f api"
echo "   Ver logs do PostgreSQL: docker compose -f infra/docker-compose.yml logs postgres"
echo "   Status: docker compose -f docker-compose.prod.yml ps"
echo "   Parar tudo: docker compose -f docker-compose.prod.yml down && docker compose -f infra/docker-compose.yml down"
echo ""
echo "📦 Aplicar migrations:"
echo "   docker compose -f docker-compose.prod.yml exec api dotnet ef database update"
