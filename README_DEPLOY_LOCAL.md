# Deploy Local - Guia Rápido

## ⚠️ Problema: Servidor não sobe?

### Opção 1: Rodar SEM Docker (Mais Simples)

Se você não tem Docker instalado ou está tendo problemas:

```bash
# 1. Certifique-se de que PostgreSQL está rodando
#    Se tiver Docker:
docker compose -f infra/docker-compose.yml up -d postgres mailpit

#    Ou use um PostgreSQL local já instalado

# 2. Configure .env.local
cp .env.example .env.local
# Edite .env.local se necessário

# 3. Rode diretamente com .NET
make run-local

# Ou diretamente:
./scripts/run-local.sh
```

A API estará em:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger**: http://localhost:5000

### Opção 2: Com Docker (Requer Docker instalado)

```bash
# 1. Instalar Docker (se não tiver)
# Ubuntu/Debian:
sudo apt install docker.io docker-compose

# Ou via snap:
sudo snap install docker

# 2. Adicionar usuário ao grupo docker
sudo usermod -aG docker $USER
# Faça logout e login novamente

# 3. Rodar deploy
make deploy-local

# Ou script simplificado:
./scripts/deploy-local-simple.sh
```

## Verificar o que está acontecendo

### Sem Docker (rodando com dotnet run)

```bash
# Ver logs da aplicação
# Os logs aparecem diretamente no terminal

# Verificar se está rodando
curl http://localhost:5000/health
```

### Com Docker

```bash
# Ver status dos containers
docker ps

# Ver logs da API
docker compose -f docker-compose.prod.yml logs -f api

# Ver logs do PostgreSQL
docker compose -f infra/docker-compose.yml logs postgres

# Testar health check
curl http://localhost:8080/health
```

## Problemas Comuns

### 1. "Cannot connect to database"

**Solução:**
- Verifique se PostgreSQL está rodando
- Verifique a connection string no `.env.local`
- Teste a conexão: `psql -h localhost -U transferencia -d transferencia_materiais`

### 2. "Port already in use"

**Solução:**
- Mude a porta no `appsettings.json` ou `.env.local`
- Ou pare o processo que está usando a porta

### 3. "Docker not found"

**Solução:**
- Instale Docker (veja comandos acima)
- Ou use a Opção 1 (sem Docker)

### 4. "Migrations not applied"

**Solução:**
```bash
# Com Docker:
docker compose -f docker-compose.prod.yml exec api dotnet ef database update

# Sem Docker:
dotnet ef database update \
  --project src/TransferenciaMateriais.Infrastructure \
  --startup-project src/TransferenciaMateriais.Api
```

## Qual opção escolher?

- **Sem Docker**: Mais simples, mas requer PostgreSQL instalado localmente
- **Com Docker**: Mais isolado, tudo containerizado, mas requer Docker instalado

## Próximos Passos

1. Escolha uma opção acima
2. Siga os passos
3. Verifique os logs se houver problemas
4. Consulte `DEPLOY_TROUBLESHOOTING.md` para mais detalhes
