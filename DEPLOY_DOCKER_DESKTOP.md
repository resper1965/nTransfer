# Deploy Completo no Docker Desktop

## Pré-requisitos

1. **Docker Desktop** instalado e rodando
2. **WSL 2** configurado (se estiver usando WSL)
3. Docker Desktop configurado para usar WSL 2 backend

## Opção 1: Executar no PowerShell/CMD do Windows

Abra o PowerShell ou CMD no Windows e execute:

```powershell
# Navegar até o projeto (ajuste o caminho)
cd C:\caminho\para\Transferencia

# Ou se estiver no WSL, use o caminho do Windows:
cd \\wsl$\Ubuntu\home\resper\Transferencia
```

Depois execute:

```powershell
# 1. Parar containers existentes
docker compose -f docker-compose.prod.yml down
docker compose -f infra/docker-compose.yml down

# 2. Build da imagem
docker build -t transferencia-api:local .

# 3. Subir serviços auxiliares
docker compose -f infra/docker-compose.yml up -d

# 4. Aguardar PostgreSQL (30 segundos)
timeout /t 30

# 5. Subir API
docker compose -f docker-compose.prod.yml --env-file .env.local up -d

# 6. Verificar status
docker compose -f docker-compose.prod.yml ps
```

## Opção 2: Executar no WSL (se Docker estiver no PATH)

```bash
# 1. Parar containers existentes
docker compose -f docker-compose.prod.yml down
docker compose -f infra/docker-compose.yml down

# 2. Build da imagem
docker build -t transferencia-api:local .

# 3. Subir serviços auxiliares
docker compose -f infra/docker-compose.yml up -d

# 4. Aguardar PostgreSQL
sleep 10

# 5. Subir API
docker compose -f docker-compose.prod.yml --env-file .env.local up -d

# 6. Verificar status
docker compose -f docker-compose.prod.yml ps
```

## Opção 3: Usar o script automatizado

Se estiver no WSL e o Docker estiver no PATH:

```bash
./scripts/deploy-docker-desktop.sh
```

## Verificar se está funcionando

```bash
# Health check
curl http://localhost:8080/health

# Ou no PowerShell:
Invoke-WebRequest -Uri http://localhost:8080/health
```

## Aplicar Migrations

```bash
# No WSL ou PowerShell:
docker compose -f docker-compose.prod.yml exec api dotnet ef database update
```

## Serviços Disponíveis

Após o deploy:

- **API**: http://localhost:8080
- **Swagger**: http://localhost:8080
- **Health**: http://localhost:8080/health
- **Mailpit**: http://localhost:8025
- **PostgreSQL**: localhost:5432

## Troubleshooting

### Docker não encontrado no WSL

1. Abra o Docker Desktop
2. Vá em Settings → Resources → WSL Integration
3. Ative a integração com sua distribuição WSL
4. Reinicie o WSL: `wsl --shutdown` e abra novamente

### Porta já em uso

Se a porta 8080 estiver em uso:

1. Pare o servidor .NET que está rodando:
   ```bash
   pkill -f 'dotnet.*TransferenciaMateriais.Api'
   ```

2. Ou mude a porta no `docker-compose.prod.yml`:
   ```yaml
   ports:
     - "8081:8080"  # Mude para 8081
   ```

### PostgreSQL não conecta

Verifique os logs:

```bash
docker compose -f infra/docker-compose.yml logs postgres
```

## Parar tudo

```bash
docker compose -f docker-compose.prod.yml down
docker compose -f infra/docker-compose.yml down
```

Para remover volumes também:

```bash
docker compose -f docker-compose.prod.yml down -v
docker compose -f infra/docker-compose.yml down -v
```
