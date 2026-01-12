# Setup com Docker Desktop

## Passo 1: Subir PostgreSQL e Mailpit

No Docker Desktop, execute:

```bash
docker compose -f infra/docker-compose.yml up -d
```

Ou, se estiver na pasta `infra`:

```bash
docker compose up -d
```

Isso vai subir:
- **PostgreSQL** na porta `5432`
- **Mailpit** na porta `8025` (web UI) e `1025` (SMTP)

## Passo 2: Verificar se está rodando

```bash
docker compose -f infra/docker-compose.yml ps
```

Você deve ver:
- `transferencia-postgres` - Status: Up
- `transferencia-mailpit` - Status: Up

## Passo 3: Aplicar Migrations

Com o PostgreSQL rodando, aplique as migrations:

```bash
dotnet ef database update \
  --project src/TransferenciaMateriais.Infrastructure \
  --startup-project src/TransferenciaMateriais.Api
```

## Passo 4: Verificar conexão

Teste se a API consegue conectar:

```bash
curl http://localhost:5000/health
```

## Configuração da Connection String

A connection string no `.env.local` já está configurada para:

```
Host=localhost;Port=5432;Database=transferencia_materiais;Username=transferencia;Password=transferencia
```

Isso funciona tanto para:
- Docker Desktop (PostgreSQL na porta 5432)
- WSL (se o Docker Desktop estiver configurado para expor portas)

## Troubleshooting

### PostgreSQL não conecta

1. Verifique se o container está rodando:
   ```bash
   docker ps | grep postgres
   ```

2. Verifique os logs:
   ```bash
   docker compose -f infra/docker-compose.yml logs postgres
   ```

3. Teste a conexão manualmente:
   ```bash
   docker compose -f infra/docker-compose.yml exec postgres psql -U transferencia -d transferencia_materiais -c "SELECT 1;"
   ```

### Porta 5432 já em uso

Se a porta 5432 já estiver em uso por outro PostgreSQL:

1. Pare o PostgreSQL local:
   ```bash
   sudo systemctl stop postgresql
   ```

2. Ou mude a porta no `infra/docker-compose.yml`:
   ```yaml
   ports:
     - "5433:5432"  # Mude para 5433
   ```

3. E atualize o `.env.local`:
   ```
   DATABASE_URL=Host=localhost;Port=5433;Database=transferencia_materiais;Username=transferencia;Password=transferencia
   ```

## Serviços Disponíveis

Após subir tudo:

- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000
- **Health**: http://localhost:5000/health
- **Mailpit Web UI**: http://localhost:8025
- **PostgreSQL**: localhost:5432

## Parar os serviços

```bash
docker compose -f infra/docker-compose.yml down
```

Para remover também os volumes (dados do banco):

```bash
docker compose -f infra/docker-compose.yml down -v
```
