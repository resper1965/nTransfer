# Workflow — Estados e Uso por Fluxo (Fonte Única)

> **Fonte Única de Verdade** — Estados do workflow para manter coerência entre diagramas, OpenAPI e modelo de dados.
> Referências: [data-model.md](../../data-models/data-model.md), [openapi.yaml](../../contracts/openapi.yaml), [diagrams.md](./diagrams.md)

## Objetivo

Centralizar estados do workflow para manter coerência entre:
- `docs/specs/transferencia-materiais/diagrams.md`
- `docs/contracts/openapi.yaml` (`WorkflowStatus`)
- `docs/data-models/data-model.md` (`WorkflowStatus`)

## Enum Canônico

O enum canônico é o `WorkflowStatus` definido em `docs/data-models/data-model.md` (Seção 2.2).

**Nenhum documento deve criar estado "novo" fora desse catálogo; se precisar, abrir item em `TBD.md`.**

## Mapeamento por Fluxo (Macro)

### F1 — Compra Direta

```
OS_CRIADA 
  → MATERIAL_FABRICADO 
  → ROMANEIO_CONFERIDO 
  → NFE_EMITIDA 
  → XML_OBTIDO 
  → (NFE_VALIDADA_OK | NFE_VALIDADA_NOK)
    - Se NOK: CORRECAO_EMISSAO_OU_VINCULO → NFE_EMITIDA (loop)
    - Se OK: VINCULO_CRIADO 
      → ESTOQUE_ORIGEM_ATUALIZADO 
      → NFE_SAIDA_ORIGEM_EMITIDA 
      → EM_TRANSITO 
      → CHEGADA_MATERIAL_DESTINO
      → ENTRADA_DESTINO_PENDENTE_ANEXO 
      → ENTRADA_DESTINO_CONCLUIDA 
      → ESTOQUE_DESTINO_ATUALIZADO 
      → PROCESSO_CONCLUIDO
```

**Estados específicos de F1:**
- `MATERIAL_FABRICADO`
- `ESTOQUE_ORIGEM_ATUALIZADO`
- `NFE_SAIDA_ORIGEM_EMITIDA`
- `EM_TRANSITO`
- `CHEGADA_MATERIAL_DESTINO`
- `ESTOQUE_DESTINO_ATUALIZADO`

### F2 — Entrega Futura (mãe)

```
OS_CRIADA 
  → OS_ATUALIZADA_DATA_ESTIMADA 
  → NFE_ENTREGA_FUTURA_EMITIDA 
  → XML_OBTIDO 
  → (NFE_VALIDADA_OK | NFE_VALIDADA_NOK)
    - Se OK: NFE_RECEBIDA_SEM_ESTOQUE 
      → OC_CRIADA_PARA_REMESSA 
      → AGUARDANDO_REMESSA 
      → PROCESSO_CONCLUIDO (fase mãe encerra)
    - Se NOK: CORRECAO_EMISSAO_OU_VINCULO → NFE_ENTREGA_FUTURA_EMITIDA (loop)
```

**Estados específicos de F2:**
- `OS_ATUALIZADA_DATA_ESTIMADA`
- `NFE_ENTREGA_FUTURA_EMITIDA`
- `NFE_RECEBIDA_SEM_ESTOQUE`
- `OC_CRIADA_PARA_REMESSA`
- `AGUARDANDO_REMESSA`
- `ALERTA_7_DIAS_ENTREGA_ESTIMADA` (estado "informativo" ou marcador operacional; pode ser implementado como pendência + notificação)

### F3 — Entrega Futura (filha)

```
OC_PENDENTE_ENTREGA_FUTURA 
  → APROVACAO_ENTREGA_PENDENTE 
  → (ENTREGA_APROVADA | ENTREGA_REPROVADA)
    - Se aprovada: NFE_REMESSA_EMITIDA 
      → XML_OBTIDO 
      → NFE_VALIDADA_OK 
      → VINCULADA_OS_OC_NFE 
      → ENTRADA_ORIGEM_CONCLUIDA 
      → SAIDA_ORIGEM_CONCLUIDA
      → EM_TRANSITO 
      → CHEGADA_MATERIAL_DESTINO 
      → ENTRADA_DESTINO_PENDENTE_ANEXO 
      → ENTRADA_DESTINO_CONCLUIDA 
      → ESTOQUE_DESTINO_ATUALIZADO 
      → PROCESSO_CONCLUIDO
    - Se reprovada: PROCESSO_CANCELADO (ou estado dedicado; se dedicar, registrar em TBD)
```

**Estados específicos de F3:**
- `OC_PENDENTE_ENTREGA_FUTURA`
- `APROVACAO_ENTREGA_PENDENTE`
- `ENTREGA_APROVADA`
- `ENTREGA_REPROVADA`
- `NFE_REMESSA_EMITIDA`
- `VINCULADA_OS_OC_NFE`
- `ENTRADA_ORIGEM_CONCLUIDA`
- `SAIDA_ORIGEM_CONCLUIDA`
- `ALERTA_30_DIAS_DESTINO` (idem F2, pode ser pendência/notificação)

### Medição (TBD-04 fechado)

```
APROVACAO_MEDICAO_PENDENTE 
  → (MEDICAO_APROVADA | MEDICAO_REPROVADA)
```

**Observação:** Só vira estado efetivo se a aplicação registrar esta decisão.

**Estados específicos de Medição:**
- `APROVACAO_MEDICAO_PENDENTE` — Medição pendente no RM/Contratos
- `MEDICAO_APROVADA` — Medição aprovada no RM/Contratos (atualizado via integração)
- `MEDICAO_REPROVADA` — Medição reprovada no RM/Contratos (atualizado via integração)

### Estados Compartilhados

Estados usados em múltiplos fluxos:

- `OS_CRIADA` — F1, F2
- `ROMANEIO_CONFERIDO` — F1
- `NFE_EMITIDA` — F1, F2, F3 (genérico)
- `XML_OBTIDO` — F1, F2, F3
- `NFE_VALIDADA_OK` — F1, F2, F3
- `NFE_VALIDADA_NOK` — F1, F2, F3
- `CORRECAO_EMISSAO_OU_VINCULO` — F1, F2
- `VINCULO_CRIADO` — F1
- `VINCULO_CORRIGIDO` — F1 (quando corrige erro de vínculo)
- `ENTRADA_DESTINO_PENDENTE_ANEXO` — F1, F3
- `ENTRADA_DESTINO_CONCLUIDA` — F1, F3
- `PROCESSO_CONCLUIDO` — F1, F2, F3
- `PROCESSO_CANCELADO` — Todos (quando processo é cancelado)

## Norma de Consistência (Obrigatória)

1. **`WorkflowStatus` no OpenAPI deve ser idêntico ao modelo de dados.**
   - Referência: `docs/data-models/data-model.md` Seção 2.2
   - Referência: `docs/contracts/openapi.yaml` `components/schemas/WorkflowStatus`

2. **`diagrams.md` deve usar exclusivamente esses nomes (Mermaid).**
   - Referência: `docs/specs/transferencia-materiais/diagrams.md`
   - Diagramas de estado devem usar apenas estados do catálogo canônico

3. **Qualquer novo estado requer:**
   - Entrada em `TBD.md` (justificativa e impacto)
   - Atualização dos 3 artefatos:
     - `docs/data-models/data-model.md` (Seção 2.2)
     - `docs/contracts/openapi.yaml` (`WorkflowStatus` enum)
     - `docs/specs/transferencia-materiais/workflow-states.md` (este arquivo)

## Validação de Consistência

Para validar que todos os documentos estão alinhados:

1. **Verificar enum em `data-model.md`:**
   ```bash
   grep -A 50 "### 2.2 WorkflowStatus" docs/data-models/data-model.md
   ```

2. **Verificar enum em `openapi.yaml`:**
   ```bash
   grep -A 50 "WorkflowStatus:" docs/contracts/openapi.yaml
   ```

3. **Verificar uso em `diagrams.md`:**
   ```bash
   grep -E "(OS_|NFE_|VINCUL|ENTRADA|SAIDA|PROCESSO|MATERIAL|ROMANEIO|ESTOQUE|CHEGADA|EM_TRANSITO|OC_|APROVACAO|ENTREGA|MEDICAO|ALERTA)" docs/specs/transferencia-materiais/diagrams.md
   ```

4. **Comparar listas:** Todos os estados usados em `diagrams.md` devem estar no enum canônico.

## Referências

- [data-model.md](../../data-models/data-model.md) — Enum canônico (Seção 2.2)
- [openapi.yaml](../../contracts/openapi.yaml) — Schema `WorkflowStatus`
- [diagrams.md](./diagrams.md) — Diagramas de estado (Mermaid)
- [TBD.md](./TBD.md) — Pendências e novos estados
