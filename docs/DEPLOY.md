# Guia de Deploy — Transferência de Materiais Entre Filiais

> Este guia define como fazer deploy do sistema em diferentes ambientes.
> Desenvolvido por **ness.**
> **Repositório:** [https://github.com/resper1965/nTransfer](https://github.com/resper1965/nTransfer)

## 1. Pré-requisitos

- **Docker** e **Docker Compose** instalados
- **Git** configurado
- Acesso ao repositório GitHub
- Variáveis de ambiente configuradas

## 1.1 Configuração de Secrets

### Gerar Secrets Aleatórios

```bash
# Gerar secrets aleatórios para produção
chmod +x scripts/generate-secrets.sh
./scripts/generate-secrets.sh
```

### Arquivos de Configuração

- **`.env.example`** — Template com valores mock e exemplos reais
- **`.env.local`** — Configuração local (não commitado)
- **`.env.production.example`** — Template para produção

### Configurar Ambiente Local

```bash
# 1. Copiar template
cp .env.example .env.local

# 2. Editar com suas configurações
nano .env.local  # ou use seu editor preferido
```

### Valores Mock vs Reais

**Mock (Desenvolvimento):**
- Database: `localhost:5432` (Docker Compose)
- Email: Mailpit local (`localhost:1025`)
- Integration: Stub mode (`ALWAYS_OK`)

**Reais (Produção):**
- Database: Neon, Supabase, AWS RDS, etc.
- Email: SendGrid, AWS SES, SMTP real
- Integration: URLs e API keys reais

## 2. Opções de Deploy

### 2.1 Deploy Local com Docker Compose

#### Opção 1: Script Automatizado (Recomendado)

```bash
# Deploy completo automatizado
make deploy-local

# Ou diretamente:
chmod +x scripts/deploy-local.sh
./scripts/deploy-local.sh
```

O script faz automaticamente:
1. ✅ Verifica pré-requisitos (Docker, Docker Compose)
2. ✅ Cria `.env.local` se não existir
3. ✅ Build da imagem Docker
4. ✅ Sobe PostgreSQL
5. ✅ Aplica migrations
6. ✅ Sobe API

#### Opção 2: Manual

```bash
# 1. Configure as variáveis de ambiente
cp .env.example .env.local
# Edite .env.local com suas configurações

# 2. Faça build e suba os serviços
docker compose -f docker-compose.prod.yml --env-file .env.local up -d --build

# 3. Execute as migrations (se necessário)
docker compose -f docker-compose.prod.yml exec api dotnet ef database update \
  --project /app/src/TransferenciaMateriais.Infrastructure \
  --startup-project /app/src/TransferenciaMateriais.Api

# 4. Verifique os logs
docker compose -f docker-compose.prod.yml logs -f api
```

A API estará disponível em:
- **HTTP**: http://localhost:8080
- **Health Check**: http://localhost:8080/health
- **Swagger**: http://localhost:8080/swagger (se habilitado)
- **Mailpit**: http://localhost:8025 (visualizar e-mails)

### 2.2 Deploy com GitHub Container Registry

O workflow `.github/workflows/deploy.yml` já está configurado para:

1. **Build automático** quando há push para `main`
2. **Push da imagem** para GitHub Container Registry (`ghcr.io`)
3. **Deploy automático** (configurar conforme sua plataforma)

#### 2.2.1 Configurar Secrets no GitHub

1. Vá em **Settings** → **Secrets and variables** → **Actions**
2. Adicione os secrets necessários:
   - `DATABASE_URL`: Connection string do PostgreSQL
   - `EMAIL_SMTP_HOST`: Host SMTP
   - `EMAIL_SMTP_USER`: Usuário SMTP
   - `EMAIL_SMTP_PASSWORD`: Senha SMTP

#### 2.2.2 Deploy Manual

```bash
# 1. Faça push para main (dispara build automático)
git push origin main

# 2. Ou dispare manualmente via GitHub Actions UI
# Actions → Deploy → Run workflow
```

### 2.3 Deploy em Azure Container Apps

#### Pré-requisitos

```bash
# Instalar Azure CLI
az login
az extension add --name containerapp
```

#### Configuração

1. **Criar Resource Group e Container App**:

```bash
# Criar resource group
az group create \
  --name transferencia-rg \
  --location eastus

# Criar Container Registry (se não tiver)
az acr create \
  --resource-group transferencia-rg \
  --name transferenciaacr \
  --sku Basic

# Criar Container App Environment
az containerapp env create \
  --name transferencia-env \
  --resource-group transferencia-rg \
  --location eastus

# Criar Container App
az containerapp create \
  --name transferencia-api \
  --resource-group transferencia-rg \
  --environment transferencia-env \
  --image ghcr.io/resper1965/nTransfer:main \
  --target-port 8080 \
  --ingress external \
  --env-vars \
    "ASPNETCORE_ENVIRONMENT=Production" \
    "ConnectionStrings__DefaultConnection=<sua-connection-string>" \
    "Email__Provider=smtp" \
    "Email__SmtpHost=<smtp-host>" \
    "Email__SmtpUser=<smtp-user>" \
    "Email__SmtpPassword=<smtp-password>"
```

2. **Atualizar workflow de deploy**:

Edite `.github/workflows/deploy.yml` e adicione no step `deploy-staging`:

```yaml
- name: Deploy to Azure Container Apps
  run: |
    az containerapp update \
      --name transferencia-api \
      --resource-group transferencia-rg \
      --image ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:${{ github.sha }}
```

### 2.4 Deploy em Railway

1. **Conectar repositório**:
   - Acesse [Railway](https://railway.app)
   - Conecte o repositório GitHub
   - Selecione o projeto

2. **Configurar variáveis**:
   - Vá em **Variables**
   - Adicione:
     - `DATABASE_URL`
     - `EMAIL_SMTP_HOST`
     - `EMAIL_SMTP_USER`
     - `EMAIL_SMTP_PASSWORD`

3. **Deploy automático**:
   - Railway detecta o `Dockerfile` automaticamente
   - Faz deploy a cada push para `main`

### 2.5 Deploy em Render

1. **Criar novo Web Service**:
   - Acesse [Render](https://render.com)
   - **New** → **Web Service**
   - Conecte o repositório GitHub

2. **Configurações**:
   - **Name**: `transferencia-api`
   - **Environment**: `Docker`
   - **Dockerfile Path**: `Dockerfile`
   - **Docker Context**: `.`

3. **Variáveis de ambiente**:
   - Adicione as variáveis necessárias

4. **Deploy automático**:
   - Render faz deploy a cada push para `main`

## 3. Migrations

### 3.1 Executar Migrations no Deploy

Adicione ao workflow de deploy ou ao Dockerfile:

```bash
# No Dockerfile (antes do ENTRYPOINT)
RUN dotnet ef database update \
  --project src/TransferenciaMateriais.Infrastructure \
  --startup-project src/TransferenciaMateriais.Api \
  --connection "$ConnectionStrings__DefaultConnection"
```

Ou execute manualmente após o deploy:

```bash
docker compose -f docker-compose.prod.yml exec api dotnet ef database update \
  --project /app/src/TransferenciaMateriais.Infrastructure \
  --startup-project /app/src/TransferenciaMateriais.Api
```

## 4. Health Checks

A API expõe um endpoint de health check:

```bash
curl http://localhost:8080/health
```

Resposta esperada:
```json
{
  "status": "Healthy",
  "timestamp": "2025-01-12T10:00:00Z"
}
```

## 5. Monitoramento

### 5.1 Logs

```bash
# Docker Compose
docker compose -f docker-compose.prod.yml logs -f api

# Kubernetes
kubectl logs -f deployment/transferencia-api

# Azure Container Apps
az containerapp logs show \
  --name transferencia-api \
  --resource-group transferencia-rg \
  --follow
```

### 5.2 Métricas

- Health check endpoint: `/health`
- Swagger (se habilitado): `/swagger`

## 6. Rollback

### 6.1 Docker Compose

```bash
# Voltar para versão anterior
docker compose -f docker-compose.prod.yml pull
docker compose -f docker-compose.prod.yml up -d
```

### 6.2 Azure Container Apps

```bash
az containerapp revision restart \
  --name transferencia-api \
  --resource-group transferencia-rg \
  --revision <revision-anterior>
```

## 7. Troubleshooting

### 7.1 Container não inicia

```bash
# Verificar logs
docker compose -f docker-compose.prod.yml logs api

# Verificar health check
curl http://localhost:8080/health
```

### 7.2 Erro de conexão com banco

```bash
# Verificar se PostgreSQL está rodando
docker compose -f docker-compose.prod.yml ps postgres

# Testar conexão
docker compose -f docker-compose.prod.yml exec postgres psql -U transferencia -d transferencia_materiais
```

### 7.3 Migrations não aplicadas

```bash
# Verificar migrations pendentes
docker compose -f docker-compose.prod.yml exec api dotnet ef migrations list \
  --project /app/src/TransferenciaMateriais.Infrastructure \
  --startup-project /app/src/TransferenciaMateriais.Api

# Aplicar migrations manualmente
docker compose -f docker-compose.prod.yml exec api dotnet ef database update \
  --project /app/src/TransferenciaMateriais.Infrastructure \
  --startup-project /app/src/TransferenciaMateriais.Api
```

## 8. Próximos Passos

- [ ] Configurar CI/CD completo
- [ ] Adicionar monitoramento (Application Insights, DataDog, etc.)
- [ ] Configurar backup automático do banco
- [ ] Configurar SSL/TLS
- [ ] Configurar rate limiting
- [ ] Configurar autenticação/autorização
