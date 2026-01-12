# Documentação — Transferência de Materiais Entre Filiais

Documentação completa da aplicação **Rotina de Transferência de Materiais Entre Filiais** — sistema de orquestração de workflow para processos de transferência entre filiais baseados em NFe, OS e OC.

**Desenvolvido por ness.**

**Repositório:** [https://github.com/resper1965/nTransfer](https://github.com/resper1965/nTransfer)

## 🔒 Fontes de Verdade (Obrigatório)

Para evitar divergências entre documentos, os seguintes artefatos são **fonte única** para seus domínios:

### 1) Estados do workflow (canônico)

- [`docs/specs/transferencia-materiais/workflow-states.md`](./specs/transferencia-materiais/workflow-states.md) — Mapeamento por fluxo e regra de consistência
- [`docs/data-models/data-model.md`](./data-models/data-model.md) — `WorkflowStatus` canônico (Seção 2.2)
- [`docs/contracts/openapi.yaml`](./contracts/openapi.yaml) — `components/schemas/WorkflowStatus` deve ser idêntico

**Regra:** Nenhum documento deve criar estado "novo" fora desse catálogo; se precisar, abrir item em [`TBD.md`](./specs/transferencia-materiais/TBD.md).

### 2) Modelo de dados (canônico)

- [`docs/data-models/data-model.md`](./data-models/data-model.md) — Entidades, campos, constraints, relacionamentos e índices

**Regra:** Qualquer alteração no modelo de dados deve ser feita primeiro em `data-model.md` e depois refletida nos outros documentos (OpenAPI, código, etc.).

### 3) Auditoria (canônico)

- [`docs/contracts/auditoria-eventos.md`](./contracts/auditoria-eventos.md) — Catálogo de `eventType` + payload mínimo por evento
- [`docs/contracts/openapi.yaml`](./contracts/openapi.yaml) — `AuditoriaEventType` + `AuditoriaEvento`

**Regra:** Novos tipos de eventos devem ser adicionados primeiro em `auditoria-eventos.md` e depois no OpenAPI.

### 4) Notificações (canônico)

- **Conteúdo/variáveis:** [`docs/contracts/email-templates.md`](./contracts/email-templates.md)
- **Disparos/destinatários/regras de envio:** [`docs/OPERATIONS.md`](./OPERATIONS.md) (Seção "2. Notificações por E-mail")
- **Persistência:** [`docs/data-models/data-model.md`](./data-models/data-model.md) (`notificacao_email`)

**Regra:** Templates de e-mail devem ser definidos em `email-templates.md`; regras operacionais em `OPERATIONS.md`.

---

## 📋 Estrutura de Documentação

### Visão do Produto
- **[PRD.md](./PRD.md)** — Product Requirements Document completo (problema, objetivos, jornadas, métricas)
- **[PROJECT_MAP.md](./specs/transferencia-materiais/PROJECT_MAP.md)** — Visão geral, objetivos, atores e fluxos críticos

### Especificação Técnica
- **[SPEC.md](./specs/transferencia-materiais/SPEC.md)** — Requisitos funcionais (RF), regras de negócio (RB), requisitos não funcionais (RNF), critérios de aceite (CA)
- **[CONSTITUTION.md](./specs/transferencia-materiais/CONSTITUTION.md)** — Princípios fundamentais, diretrizes de design e padrões
- **[PLAN.md](./specs/transferencia-materiais/PLAN.md)** — Estratégia de implementação e decisões técnicas
- **[TASKS.md](./specs/transferencia-materiais/TASKS.md)** — Lista de tarefas acionáveis (T01-T12)

### Arquitetura e Operação
- **[ARCHITECTURE.md](./ARCHITECTURE.md)** — Arquitetura técnica, componentes, boundaries, integrações
- **[OPERATIONS.md](./OPERATIONS.md)** — Operação: painéis, notificações, SLAs, auditoria, troubleshooting
- **[DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md)** — Guia de desenvolvimento (.NET 8): setup, testes, build
- **[DESIGN_GUIDE.md](./DESIGN_GUIDE.md)** — Guia de Design (UX/UI): princípios, padrões, componentes, fluxos críticos

### Fluxos e Diagramas
- **[diagrams.md](./specs/transferencia-materiais/diagrams.md)** — Diagramas de fluxo e estados (F1, F2, F3) em Mermaid
- **[workflow-states.md](./specs/transferencia-materiais/workflow-states.md)** — Estados do workflow (fonte única de verdade, mapeamento por fluxo)
- **[examples.md](./specs/transferencia-materiais/examples.md)** — Exemplos práticos: payloads API, eventos de auditoria, estados do workflow

### Modelos de Dados
- **[data-model.md](./data-models/data-model.md)** — Entidades e estrutura do banco de dados

### Contratos
- **[openapi.yaml](./contracts/openapi.yaml)** — Especificação OpenAPI 3.0.3 da API (schemas completos)
- **[email-templates.md](./contracts/email-templates.md)** — Templates de e-mail (assuntos, corpos, variáveis)
- **[auditoria-eventos.md](./contracts/auditoria-eventos.md)** — Catálogo de eventos de auditoria (payload mínimo por tipo)
- **[movimentos-dicionario.md](./contracts/movimentos-dicionario.md)** — Dicionário técnico de movimentos RM/nFlow/Qive

### Exemplos
- **[examples.md](./specs/transferencia-materiais/examples.md)** — Exemplos práticos de API e eventos

### Referência
- **[glossario.md](./glossario.md)** — Glossário de termos técnicos do domínio

### Auditorias
- **[Auditoria de Documentação](./audits/documentacao.md)** — Análise de lacunas e inconsistências na documentação
- **[Auditoria Final](./audits/final.md)** — Validação de prontidão para implementação
- **[Auditoria de Higienização](./audits/repo-higiene.md)** — Plano de organização e higienização do repositório

## 🔄 Ordem de Leitura Recomendada

### Para Entender o Produto
1. **[Glossário](./glossario.md)** — Familiarize-se com os termos técnicos (OS, OC, NFe, etc.)
2. **[PRD.md](./PRD.md)** — Entenda o problema, objetivos e jornadas do usuário
3. **[PROJECT_MAP.md](./specs/transferencia-materiais/PROJECT_MAP.md)** — Visão geral e contexto

### Para Desenvolver
4. **[SPEC.md](./specs/transferencia-materiais/SPEC.md)** — Requisitos funcionais, regras de negócio, critérios de aceite
5. **[diagrams.md](./specs/transferencia-materiais/diagrams.md)** — Visualize os 3 fluxos (F1, F2, F3) e estados
6. **[ARCHITECTURE.md](./ARCHITECTURE.md)** — Entenda a arquitetura técnica e componentes
7. **[DESIGN_GUIDE.md](./DESIGN_GUIDE.md)** — Princípios e padrões de UX/UI
8. **[data-model.md](./data-models/data-model.md)** — Estrutura de dados e relacionamentos
9. **[openapi.yaml](./contracts/openapi.yaml)** — Contratos da API (endpoints, schemas)
10. **[examples.md](./specs/transferencia-materiais/examples.md)** — Exemplos práticos de uso
11. **[DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md)** — Setup local, testes, build (.NET 8)

### Para Operar
12. **[OPERATIONS.md](./OPERATIONS.md)** — Painéis, notificações, auditoria, troubleshooting

### Para Planejar
13. **[PLAN.md](./specs/transferencia-materiais/PLAN.md)** — Estratégia de implementação
14. **[TASKS.md](./specs/transferencia-materiais/TASKS.md)** — Tarefas acionáveis
15. **[TBD.md](./specs/transferencia-materiais/TBD.md)** — Verifique pendências antes de implementar

## 📝 Convenções e Referências

- **TBD-XX** = To Be Defined (ver [TBD.md](./specs/transferencia-materiais/TBD.md))
- **RB-XX** = Regra de Negócio (ver [SPEC.md](./specs/transferencia-materiais/SPEC.md#regras-de-negócio-rb))
- **RF-XX** = Requisito Funcional (ver [SPEC.md](./specs/transferencia-materiais/SPEC.md#requisitos-funcionais-rf))
- **RNF-XX** = Requisito Não Funcional (ver [SPEC.md](./specs/transferencia-materiais/SPEC.md#requisitos-não-funcionais-rnf))
- **CA-XX** = Critério de Aceite (ver [SPEC.md](./specs/transferencia-materiais/SPEC.md#critérios-de-aceite-ca))
- **T-XX** = Tarefa (ver [TASKS.md](./specs/transferencia-materiais/TASKS.md))
- **F1, F2, F3** = Fluxos (ver [diagrams.md](./specs/transferencia-materiais/diagrams.md))

## 🔗 Links Rápidos

### Documentos Principais
- [PRD](./PRD.md) — Product Requirements Document
- [Arquitetura](./ARCHITECTURE.md) — Arquitetura técnica
- [Design (UX/UI)](./DESIGN_GUIDE.md) — Princípios e padrões de interface
- [Operação](./OPERATIONS.md) — Painéis, notificações, auditoria
- [Guia de Desenvolvimento](./DEVELOPMENT_GUIDE.md) — Setup e desenvolvimento

### Especificação
- [SPEC](./specs/transferencia-materiais/SPEC.md) — Requisitos e regras
- [Diagramas](./specs/transferencia-materiais/diagrams.md) — Fluxos e estados
- [Exemplos](./specs/transferencia-materiais/examples.md) — Exemplos práticos
- [Rastreabilidade](./RASTREABILIDADE.md) — Matriz de rastreabilidade

### Referência
- [Glossário](./glossario.md) — Termos técnicos
- [OpenAPI](./contracts/openapi.yaml) — Contratos da API
- [Data Model](./data-models/data-model.md) — Modelo de dados
- [TBD](./specs/transferencia-materiais/TBD.md) — Pendências

## 📌 Status do Projeto

- ✅ Documentação completa criada
- ✅ Stack técnica definida (.NET 8) — [TBD-05](./specs/transferencia-materiais/TBD.md#tbd-05--stack-técnico-backend-frontend-banco) fechado
- ✅ Estrutura .NET criada (Clean Architecture)
- ✅ Repositório GitHub: [https://github.com/resper1965/nTransfer](https://github.com/resper1965/nTransfer)
- ⏳ Aguardando definições críticas (ver [TBD.md](./specs/transferencia-materiais/TBD.md))
- 📋 Pronto para iniciar implementação (TASKS T02 em diante)

## 🛠️ Próximos Passos

1. Resolver itens TBD críticos:
   - TBD-01: Mecanismo de integração Qive↔RM
   - TBD-03: Política "Aprova entrega?"
   - TBD-04: Medição (onde ocorre)
   - TBD-06: Mapeamento estados RM nFlow
   - TBD-07: "Caminhão no local"
2. Iniciar implementação seguindo [TASKS.md](./specs/transferencia-materiais/TASKS.md)
3. Validar documentação (ver [VALIDACAO.md](./VALIDACAO.md))

## ✅ Checklist de Validação

Antes de commit/PR, verificar:
- [ ] Build passa (`make build`)
- [ ] Testes passam (`make test`)
- [ ] Lint passa (`make lint`)
- [ ] Links funcionam (verificar manualmente)
- [ ] OpenAPI válido
- [ ] Diagramas Mermaid renderizam
- [ ] Nenhuma referência a ID inexistente

Ver [VALIDACAO.md](./VALIDACAO.md) para checklist completo.
