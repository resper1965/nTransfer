# Commits para Fechar as 5 Lacunas Críticas

## Ordem Prática de Commits

### 1. Commit do data-model.md (LAC-01)

```bash
git add docs/data-models/data-model.md
git commit -m "docs: LAC-01 - Expandir data-model.md com estrutura completa

- Adicionar convenções (UUIDs, DateTimeOffset, decimal, varchar, jsonb)
- Definir enums canônicos (FluxType, WorkflowStatus, VinculoStatus, etc.)
- Especificar entidades completas (OS, OC, NFe, Vínculo, Pendência, Notificação, Anexo, AuditoriaEvento, ProcessedMessage)
- Incluir campos, tipos, constraints (PK, FK, unique, check)
- Definir relacionamentos e índices
- Expandir de 20 para 512 linhas

Refs: docs/AUDITORIA_DOCUMENTACAO.md LAC-01"
```

### 2. Commit do workflow-states.md (LAC-02)

```bash
git add docs/specs/transferencia-materiais/workflow-states.md
git commit -m "docs: LAC-02 - Criar workflow-states.md como fonte única de estados

- Definir enum canônico WorkflowStatus (40+ estados)
- Mapear estados por fluxo (F1, F2, F3, Medição)
- Estabelecer normas de consistência obrigatórias
- Garantir alinhamento entre diagrams.md, openapi.yaml e data-model.md
- Criar documento de 180 linhas

Refs: docs/AUDITORIA_DOCUMENTACAO.md LAC-02"
```

### 3. Patch no openapi.yaml (LAC-02 + LAC-03 + LAC-04)

```bash
git add docs/contracts/openapi.yaml docs/specs/transferencia-materiais/diagrams.md
git commit -m "docs: LAC-02+LAC-03+LAC-04 - Patch OpenAPI e alinhar diagramas

LAC-02:
- Atualizar WorkflowStatus enum com referência a workflow-states.md
- Alinhar diagrams.md com estados canônicos

LAC-03:
- Adicionar schemas OC, NFe, Pendencia, Notificacao com enums inline
- Adicionar query parameters do painel (required: false)

LAC-04:
- Criar enum AuditoriaEventType (22 tipos)
- Ajustar AuditoriaEvento.eventType para usar \$ref
- Ajustar required fields (incluir correlationType)

Arquivos:
- openapi.yaml: 812 linhas (versão 0.3.0)
- diagrams.md: estados alinhados ao catálogo canônico

Refs: docs/AUDITORIA_DOCUMENTACAO.md LAC-02, LAC-03, LAC-04"
```

### 4. Commit do auditoria-eventos.md (LAC-04)

```bash
git add docs/contracts/auditoria-eventos.md docs/OPERATIONS.md docs/ARCHITECTURE.md docs/README.md docs/specs/transferencia-materiais/examples.md
git commit -m "docs: LAC-04 - Criar catálogo de eventos de auditoria

- Criar auditoria-eventos.md (479 linhas)
- Documentar 22 eventos com payload mínimo padronizado
- Incluir exemplos JSON por evento
- Definir regras de implementação (imutabilidade, correlação, timestamp)
- Adicionar referências cruzadas

Arquivos atualizados:
- OPERATIONS.md: referência ao catálogo
- ARCHITECTURE.md: referência ao catálogo
- README.md: link no índice
- examples.md: 3 exemplos expandidos

Refs: docs/AUDITORIA_DOCUMENTACAO.md LAC-04"
```

### 5. Commit do email-templates.md (LAC-05)

```bash
git add docs/contracts/email-templates.md docs/AUDITORIA_DOCUMENTACAO.md
git commit -m "docs: LAC-05 - Simplificar templates de e-mail (plain text)

- Formato simplificado: plain text (MVP)
- Variáveis no formato {{variavel}} (chaves duplas)
- 8 templates documentados (CHEGADA_MATERIAL, NFE_ENTRADA_DISPONIVEL, NFE_SAIDA_PRONTA_IMPRESSAO, PENDENCIA_ABERTA, CANCELAMENTO_PROCESSO, LEMBRETE_7_DIAS_ENTREGA_ESTIMADA, LEMBRETE_30_DIAS_DESTINO, MEDICAO_CONCLUIDA)
- Tabela de destinatários por tipo
- Variáveis disponíveis documentadas
- Regras de implementação

Arquivo: 248 linhas (formato simplificado)

Refs: docs/AUDITORIA_DOCUMENTACAO.md LAC-05"
```

## Checklist de Validação (Após Commits)

### openapi.yaml

- ✅ Todas as referências `$ref` resolvem
- ✅ `WorkflowStatus` está idêntico ao catálogo do modelo
- ✅ `AuditoriaEvento.eventType` usa enum (`$ref: "#/components/schemas/AuditoriaEventType"`)

### diagrams.md

- ✅ Substituir nomes antigos pelos nomes do catálogo (sem "estados extra")
- ✅ Todos os estados usados estão no `workflow-states.md`

### Links entre docs

- ✅ `OPERATIONS.md` deve apontar para `docs/contracts/email-templates.md`
- ✅ `ARCHITECTURE.md` deve apontar para `docs/contracts/auditoria-eventos.md`

## Validação Automática

Execute para validar:

```bash
# Validar referências $ref no OpenAPI
grep -E "^\s+\$ref:" docs/contracts/openapi.yaml | sed 's/.*#\/components\/schemas\///' | sed 's/"}//' | sed 's/"$//' | sort -u > /tmp/refs.txt
grep -E "^    [A-Z][a-zA-Z]*:" docs/contracts/openapi.yaml | sed 's/:$//' | sed 's/^    //' | sort -u > /tmp/schemas.txt
comm -23 /tmp/refs.txt /tmp/schemas.txt || echo "✅ Todas as referências resolvem"

# Validar links
grep -q "email-templates.md" docs/OPERATIONS.md && echo "✅ OPERATIONS.md → email-templates.md" || echo "❌ Link faltando"
grep -q "auditoria-eventos.md" docs/ARCHITECTURE.md && echo "✅ ARCHITECTURE.md → auditoria-eventos.md" || echo "❌ Link faltando"
```
