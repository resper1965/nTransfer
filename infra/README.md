# Infraestrutura Local

Serviços auxiliares para desenvolvimento local via Docker Compose.

## Serviços

### Postgres
- **Porta:** 5432
- **Usuário:** transferencia
- **Senha:** transferencia
- **Database:** transferencia_materiais
- **Connection String:** `Host=localhost;Port=5432;Database=transferencia_materiais;Username=transferencia;Password=transferencia`

### Mailpit
- **Web UI:** http://localhost:8025
- **SMTP:** localhost:1025
- Serviço para testar e-mails localmente (DEC-02: notificações via e-mail)

## Comandos

### Subir serviços
```bash
make up
# ou
docker compose -f infra/docker-compose.yml up -d
```

### Parar serviços
```bash
make down
# ou
docker compose -f infra/docker-compose.yml down
```

### Ver logs
```bash
docker compose -f infra/docker-compose.yml logs -f
```

### Limpar volumes (cuidado: apaga dados)
```bash
docker compose -f infra/docker-compose.yml down -v
```
