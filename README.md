# Transferencia

**Sistema de Transferência de Materiais Entre Filiais** — Orquestrador de workflow para processos de transferência baseados em NFe, OS e OC.

Desenvolvido por **ness.**

**Repositório:** [https://github.com/resper1965/nTransfer](https://github.com/resper1965/nTransfer)

Aplicação desenvolvida com auxílio do GitHub Spec Kit.

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

## 🛠️ Ferramentas de Desenvolvimento

Este projeto utiliza o **GitHub Spec Kit** como ferramenta de auxílio ao desenvolvimento. O spec-kit **não faz parte da aplicação final**, sendo usado apenas durante o processo de desenvolvimento para facilitar o Spec-Driven Development.

### Setup Inicial

Para configurar o spec-kit no projeto:

```bash
# Opção 1: Usar o script de setup
./.spec-kit-setup.sh

# Opção 2: Instalação manual
uv tool install specify-cli --from git+https://github.com/github/spec-kit.git
```

### Verificar Instalação

```bash
specify check
```

### Inicializar Projeto com Spec Kit

```bash
# Inicializar no diretório atual com assistente de IA
specify init . --ai claude
# ou
specify init . --ai copilot
```

### Comandos Úteis

```bash
# Via npm scripts
npm run specify:check    # Verificar instalação
npm run specify:init     # Inicializar projeto
npm run specify:help    # Ver ajuda

# Via CLI direto
specify check           # Verificar instalação
specify init . --ai <assistente>  # Inicializar
specify --help          # Ver ajuda
```

## 📚 Documentação

Para mais informações sobre como usar o Spec Kit, consulte:
- [SPEC-KIT.md](./SPEC-KIT.md) - Guia completo de uso do Spec Kit

## 🔧 Requisitos

- **.NET SDK 8.0** ou superior
- **Docker + Docker Compose** (para Postgres e Mailpit)
- **Make** (opcional, mas recomendado)
- **Git** (>= 2.40)

Para mais detalhes, consulte o [Guia de Desenvolvimento](./docs/DEVELOPMENT_GUIDE.md).

## 📖 Comandos do Spec Kit

Após inicializar o projeto, os seguintes comandos estarão disponíveis no chat do seu assistente de IA:

- `/speckit.constitution` - Cria ou atualiza os princípios e diretrizes de desenvolvimento
- `/speckit.specify` - Define os requisitos e histórias de usuário
- `/speckit.plan` - Cria planos de implementação técnica
- `/speckit.tasks` - Gera listas de tarefas acionáveis
- `/speckit.implement` - Executa as tarefas para construir a funcionalidade

## 🔗 Links Úteis

- [GitHub Spec Kit](https://github.com/github/spec-kit)
- [Documentação Oficial](https://github.github.io/spec-kit/)
- [Microsoft Learn - Spec-Driven Development](https://learn.microsoft.com/pt-br/training/modules/spec-driven-development-github-spec-kit-enterprise-developers/)
