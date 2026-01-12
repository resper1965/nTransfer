# Transferencia

**Sistema de Transferência de Materiais Entre Filiais** — Orquestrador de workflow para processos de transferência baseados em NFe, OS e OC.

Desenvolvido por **ness.**

**Repositório:** [https://github.com/resper1965/nTransfer](https://github.com/resper1965/nTransfer)


## 🔒 Artefatos Canônicos (Fontes de Verdade)

Estes documentos são **fonte única** para seus domínios. Qualquer alteração deve começar aqui:

### Estados do Workflow
- [`docs/specs/transferencia-materiais/workflow-states.md`](./docs/specs/transferencia-materiais/workflow-states.md) — Mapeamento por fluxo e regra de consistência
- [`docs/data-models/data-model.md`](./docs/data-models/data-model.md) — `WorkflowStatus` canônico (Seção 2.2)
- [`docs/contracts/openapi.yaml`](./docs/contracts/openapi.yaml) — `components/schemas/WorkflowStatus` (deve ser idêntico)

### Modelo de Dados
- [`docs/data-models/data-model.md`](./docs/data-models/data-model.md) — Entidades, campos, constraints, relacionamentos e índices

### Auditoria
- [`docs/contracts/auditoria-eventos.md`](./docs/contracts/auditoria-eventos.md) — Catálogo de `eventType` + payload mínimo por evento
- [`docs/contracts/openapi.yaml`](./docs/contracts/openapi.yaml) — `AuditoriaEventType` + `AuditoriaEvento`

### Notificações
- [`docs/contracts/email-templates.md`](./docs/contracts/email-templates.md) — Templates de e-mail (conteúdo/variáveis)
- [`docs/OPERATIONS.md`](./docs/OPERATIONS.md) — Disparos/destinatários/regras de envio (Seção "2. Notificações por E-mail")

---

## 📚 Documentação Completa

A documentação completa do projeto está em [`docs/`](./docs/):

### Documentação Principal
- **[Índice Completo](./docs/README.md)** — Todos os artefatos organizados
- **[Guia de Desenvolvimento](./docs/DEVELOPMENT_GUIDE.md)** — Setup local, comandos, testes (.NET 8)
- **[Arquitetura](./docs/ARCHITECTURE.md)** — Arquitetura técnica e componentes
- **[Operação](./docs/OPERATIONS.md)** — Painéis, notificações, auditoria

### Especificação
- **[PROJECT_MAP](./docs/specs/transferencia-materiais/PROJECT_MAP.md)** — Visão geral e objetivos
- **[SPEC](./docs/specs/transferencia-materiais/SPEC.md)** — Requisitos funcionais, regras de negócio, RNF
- **[PLAN](./docs/specs/transferencia-materiais/PLAN.md)** — Plano de implementação
- **[TASKS](./docs/specs/transferencia-materiais/TASKS.md)** — Tarefas acionáveis
- **[TBD](./docs/specs/transferencia-materiais/TBD.md)** — Pendências e decisões

### Contratos e Modelos
- **[OpenAPI](./docs/contracts/openapi.yaml)** — Especificação completa da API
- **[Data Model](./docs/data-models/data-model.md)** — Modelo de dados completo
- **[Workflow States](./docs/specs/transferencia-materiais/workflow-states.md)** — Estados canônicos
- **[Workflow Transitions](./docs/specs/transferencia-materiais/workflow-transitions.md)** — Transições documentadas
- **[Diagramas](./docs/specs/transferencia-materiais/diagrams.md)** — Fluxos e estados (Mermaid)

### Referência
- **[Glossário](./docs/glossario.md)** — Termos técnicos e nomenclatura padrão
- **[Exemplos](./docs/specs/transferencia-materiais/examples.md)** — Exemplos práticos de API e auditoria


## 🔧 Requisitos

- **.NET SDK 8.0** ou superior
- **Docker + Docker Compose** (para Postgres e Mailpit)
- **Make** (opcional, mas recomendado)
- **Git** (>= 2.40)

Para mais detalhes, consulte o [Guia de Desenvolvimento](./docs/DEVELOPMENT_GUIDE.md).

## 🔗 Links Úteis

- [Documentação Completa](./docs/README.md) — Índice de toda a documentação do projeto
