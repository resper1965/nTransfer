# Guia de Desenvolvimento — .NET

> Este guia define como desenvolver e validar a rotina Transferência de Materiais Entre Filiais localmente.
> Desenvolvido por **ness.**
> **Repositório:** [https://github.com/resper1965/nTransfer](https://github.com/resper1965/nTransfer)
> Stack escolhida: **.NET 8** (TBD-05 fechado em 2025-01-12).

## 1. Objetivo

Este guia define como desenvolver e validar a rotina Transferência de Materiais Entre Filiais localmente: setup, execução, testes, lint/build e estrutura esperada do projeto.

## 2. Pré-requisitos

- **.NET SDK 8.0** ou superior
- **Git** (>= 2.40)
- **Docker + Docker Compose** (para serviços auxiliares: Postgres e Mailpit)
- **Make** (opcional, mas recomendado para padronizar comandos)
- **Editor/IDE**: Cursor, Visual Studio, Rider ou VS Code com extensões C#

## 3. Estrutura do Projeto

### 3.1 Estrutura .NET (Clean Architecture)

```
/
├── src/
│   ├── TransferenciaMateriais.Api/          # ASP.NET Core (controllers, Program.cs)
│   ├── TransferenciaMateriais.Domain/        # Regras de negócio, state machine, RB-xx
│   ├── TransferenciaMateriais.Application/   # Use-cases, orquestração
│   └── TransferenciaMateriais.Infrastructure/# EF Core, Email, IntegrationAdapter
├── tests/
│   ├── TransferenciaMateriais.Domain.Tests/  # Testes unitários do domínio
│   ├── TransferenciaMateriais.Application.Tests/
│   └── TransferenciaMateriais.Api.Tests/    # Testes de integração da API
├── infra/
│   ├── docker-compose.yml                    # Postgres + Mailpit
│   └── README.md
├── docs/                                     # Documentação Spec Kit
├── TransferenciaMateriais.sln                # Solution file
├── Directory.Build.props                     # Configurações globais
├── Makefile                                 # Comandos padronizados
└── .env.example                              # Variáveis de ambiente exemplo
```

### 3.2 Separação de Responsabilidades

- **Domain**: Regras de negócio (RB-01..RB-10), state machine (F1/F2/F3), validações
- **Application**: Use-cases, orquestração de fluxos
- **Infrastructure**: Persistência (EF Core), e-mail, adaptador de integração (stub)
- **Api**: Controllers, Swagger, configuração de serviços

## 4. Setup Local

### 4.1 Variáveis de Ambiente

Copie `.env.example` para `.env` e ajuste se necessário:

```bash
cp .env.example .env
```

Variáveis principais:
- `DATABASE_URL`: Connection string do Postgres
- `EMAIL_PROVIDER`: `mailpit` (local) ou `smtp` (produção)
- `INTEGRATION_ADAPTER_MODE`: `stub` (até TBD-01 ser fechado)

### 4.2 Serviços Auxiliares (Docker)

Suba Postgres e Mailpit:

```bash
make up
# ou
docker compose -f infra/docker-compose.yml up -d
```

Verifique se os serviços estão rodando:

```bash
docker compose -f infra/docker-compose.yml ps
```

**Mailpit Web UI**: http://localhost:8025 (para visualizar e-mails enviados)

### 4.3 Instalação de Dependências

```bash
dotnet restore
```

## 5. Como Rodar o Projeto

### 5.1 Comandos Padronizados (Makefile)

```bash
make dev      # Roda a API em modo desenvolvimento
make test     # Roda todos os testes
make lint     # Formatação + análise de código
make build    # Compila a solução
make check    # lint + test (gate de PR)
make up       # Sobe serviços Docker
make down     # Para serviços Docker
```

### 5.2 Execução Local

```bash
make dev
# ou
dotnet run --project src/TransferenciaMateriais.Api
```

A API estará disponível em:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger**: http://localhost:5000 (raiz em desenvolvimento)

## 6. Como Rodar Testes

### 6.1 Pirâmide de Testes

**Unitários (obrigatórios no MVP)**:
- State machine do workflow (F1/F2/F3) - `Domain.Tests`
- Validações RB-01..RB-10 - `Domain.Tests`
- Idempotência (evento duplicado) - `Domain.Tests`
- Regras de anexos obrigatórios - `Domain.Tests`

**Integração (mínimo)**:
- API endpoints principais - `Api.Tests`
- Persistência (migrations + CRUD) - `Application.Tests`
- Adapters: e-mail (mock) e integração (stub) - `Application.Tests`

**E2E (opcional no início)**:
- Fluxos completos críticos (quando houver UI)

### 6.2 Comandos

```bash
make test
# ou
dotnet test
```

Para rodar testes de um projeto específico:

```bash
dotnet test tests/TransferenciaMateriais.Domain.Tests
```

### 6.3 Dados de Teste

Use exemplos de `docs/specs/transferencia-materiais/examples.md` como base para fixtures.

## 7. Lint / Format / Build

### 7.1 Formatação e Análise

```bash
make lint
# ou
dotnet format --verify-no-changes
dotnet build --no-restore
```

Para formatar automaticamente:

```bash
dotnet format
```

### 7.2 Build

```bash
make build
# ou
dotnet build
```

### 7.3 Regra de PR

PR só passa com `make check` verde (lint + test).

## 8. Migrations (EF Core) — LAC-17

Esta seção assume Entity Framework Core. Se outro ORM for usado, substituir pelos comandos equivalentes.

### 8.1 Instalar Ferramenta (Uma Vez)

```bash
dotnet tool install --global dotnet-ef
```

### 8.2 Criar Migration

```bash
dotnet ef migrations add <NomeDaMigration> \
  --project src/TransferenciaMateriais.Infrastructure \
  --startup-project src/TransferenciaMateriais.Api
```

### 8.3 Aplicar Migrations (Local)

```bash
dotnet ef database update \
  --project src/TransferenciaMateriais.Infrastructure \
  --startup-project src/TransferenciaMateriais.Api
```

### 8.4 Reverter Migration (Quando Necessário)

**Opção A — Voltar para uma migration específica:**

```bash
dotnet ef database update <NomeDaMigrationAnterior> \
  --project src/TransferenciaMateriais.Infrastructure \
  --startup-project src/TransferenciaMateriais.Api
```

**Opção B — Remover a última migration** (somente se ainda não aplicada em ambientes compartilhados):

```bash
dotnet ef migrations remove \
  --project src/TransferenciaMateriais.Infrastructure \
  --startup-project src/TransferenciaMateriais.Api
```

### 8.5 Estratégia Mínima de Rollout/Rollback

**Produção:**
- Aplicar migrations em janela controlada ou pipeline
- Manter backup/rollback do banco conforme política do projeto

**Rollback:**
- Preferir "forward-fix" quando houver dados críticos
- Se rollback for necessário, usar `database update <MigrationAnterior>` e validar integridade

## 9. Observações Importantes

- **Endpoints e schemas** devem permanecer coerentes com [`docs/contracts/openapi.yaml`](../contracts/openapi.yaml).
- **Estados do workflow** devem seguir [`docs/specs/transferencia-materiais/workflow-states.md`](../specs/transferencia-materiais/workflow-states.md).

## 10. Convenções de Desenvolvimento

### 9.1 Contratos Primeiro

- Alterações em endpoints → atualizar `docs/contracts/openapi.yaml`
- Alterações em domínio → atualizar SPEC/PLAN/TASKS quando impactarem RB/RF/CA

### 9.2 Referências Cruzadas Obrigatórias

Commits/PRs devem referenciar:
- Tarefa (Txx em `docs/specs/transferencia-materiais/TASKS.md`)
- Requisito (RF-xx/RB-xx em `docs/specs/transferencia-materiais/SPEC.md`), quando aplicável

### 9.3 TBD Controlado

Qualquer decisão não definida → `docs/specs/transferencia-materiais/TBD.md`

Quando um TBD for fechado:
- Registrar decisão final
- Data
- Impacto nos arquivos alterados

## 11. Observabilidade

### 10.1 Logs

Logs estruturados com correlation-id (RNF-04):
- Usar `ILogger<T>` do .NET
- Incluir correlation-id (OS/OC/NFe) em todas as mensagens relevantes

### 10.2 Auditoria

Toda transição/decisão deve gerar evento de auditoria (RNF-01):
- Implementar em `Domain` (interface)
- Persistir em `Infrastructure` (EF Core)

## 12. Referências

- [CONSTITUTION.md](../specs/transferencia-materiais/CONSTITUTION.md) - Princípios e diretrizes
- [SPEC.md](../specs/transferencia-materiais/SPEC.md) - Requisitos funcionais e regras de negócio
- [PLAN.md](../specs/transferencia-materiais/PLAN.md) - Estratégia de implementação
- [TASKS.md](../specs/transferencia-materiais/TASKS.md) - Tarefas acionáveis
- [TBD.md](../specs/transferencia-materiais/TBD.md) - Pendências
- [OpenAPI](../contracts/openapi.yaml) - Contratos da API
