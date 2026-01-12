# Troubleshooting - Deploy Local

## Problema: Servidor não sobe

### 1. Verificar se Docker está rodando

```bash
docker ps
```

Se não funcionar, inicie o Docker.

### 2. Verificar se .env.local existe

```bash
ls -la .env.local
```

Se não existir:
```bash
cp .env.example .env.local
```

### 3. Parar containers antigos

```bash
docker compose -f docker-compose.prod.yml down
docker compose -f infra/docker-compose.yml down
```

### 4. Subir serviços auxiliares primeiro

```bash
docker compose -f infra/docker-compose.yml up -d
```

Aguarde alguns segundos e verifique:
```bash
docker compose -f infra/docker-compose.yml ps
```

### 5. Build da imagem

```bash
docker build -t transferencia-api:local .
```

### 6. Subir API

```bash
docker compose -f docker-compose.prod.yml --env-file .env.local up -d
```

### 7. Verificar logs

```bash
# Logs da API
docker compose -f docker-compose.prod.yml logs -f api

# Logs do PostgreSQL
docker compose -f infra/docker-compose.yml logs postgres
```

### 8. Verificar se está rodando

```bash
# Listar containers
docker ps

# Testar health check
curl http://localhost:8080/health
```

## Erros Comuns

### Erro: "Cannot connect to database"

**Solução:**
1. Verifique se PostgreSQL está rodando:
   ```bash
   docker compose -f infra/docker-compose.yml ps postgres
   ```

2. Verifique a connection string no .env.local:
   ```bash
   cat .env.local | grep DATABASE_URL
   ```

3. Teste a conexão:
   ```bash
   docker compose -f infra/docker-compose.yml exec postgres psql -U transferencia -d transferencia_materiais -c "SELECT 1;"
   ```

### Erro: "Port already in use"

**Solução:**
1. Verifique qual processo está usando a porta:
   ```bash
   lsof -i :8080
   # ou
   netstat -tulpn | grep 8080
   ```

2. Pare o processo ou mude a porta no docker-compose.prod.yml

### Erro: "Image not found"

**Solução:**
1. Faça build da imagem:
   ```bash
   docker build -t transferencia-api:local .
   ```

### Erro: "Network not found"

**Solução:**
1. Crie a network manualmente:
   ```bash
   docker network create transferencia-network
   ```

   Ou simplesmente suba os serviços que a network será criada automaticamente.

## Deploy Simplificado (Passo a Passo)

```bash
# 1. Parar tudo
docker compose -f docker-compose.prod.yml down
docker compose -f infra/docker-compose.yml down

# 2. Criar .env.local se não existir
[ ! -f .env.local ] && cp .env.example .env.local

# 3. Subir PostgreSQL e Mailpit
docker compose -f infra/docker-compose.yml up -d

# 4. Aguardar PostgreSQL estar pronto
sleep 5

# 5. Build da imagem
docker build -t transferencia-api:local .

# 6. Subir API
docker compose -f docker-compose.prod.yml --env-file .env.local up -d

# 7. Verificar logs
docker compose -f docker-compose.prod.yml logs -f api
```

## Verificar Status

```bash
# Status de todos os containers
docker ps -a | grep transferencia

# Status via docker compose
docker compose -f docker-compose.prod.yml ps
docker compose -f infra/docker-compose.yml ps

# Testar endpoints
curl http://localhost:8080/health
curl http://localhost:8025  # Mailpit
```

## Limpar e Recomeçar

```bash
# Parar e remover containers
docker compose -f docker-compose.prod.yml down -v
docker compose -f infra/docker-compose.yml down -v

# Remover imagens
docker rmi transferencia-api:local 2>/dev/null || true

# Recomeçar
./scripts/deploy-local-simple.sh
```
