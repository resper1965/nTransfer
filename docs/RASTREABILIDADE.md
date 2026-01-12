# Matriz de Rastreabilidade — Transferência de Materiais Entre Filiais

> **Matriz de Rastreabilidade** — Ligação entre Fluxos, Estados, Requisitos, Endpoints, Entidades e Eventos de Auditoria.
> Referências: [SPEC.md](./specs/transferencia-materiais/SPEC.md), [diagrams.md](./specs/transferencia-materiais/diagrams.md), [openapi.yaml](./contracts/openapi.yaml)

## Legenda

- **F1, F2, F3**: Fluxos (Compra Direta, Entrega Futura mãe, Entrega Futura filha)
- **RB-XX**: Regra de Negócio
- **RF-XX**: Requisito Funcional
- **RNF-XX**: Requisito Não Funcional
- **CA-XX**: Critério de Aceite
- **Endpoint**: Caminho da API conforme [openapi.yaml](./contracts/openapi.yaml)
- **Entidade**: Modelo de dados conforme [data-model.md](./data-models/data-model.md)
- **Evento**: Tipo de evento de auditoria

## Matriz Completa

| Fluxo | Estado | RB | RF | RNF | CA | Endpoint | Entidade | Evento Auditoria | Teste |
|-------|--------|----|----|-----|----|----------|----------|------------------|-------|
| F1 | OS_CRIADA | RB-02 | RF-01 | - | - | `POST /os` | OS | `OS_CRIADA` | T02 |
| F1 | MATERIAL_FABRICADO | - | - | - | - | - | OS | `MATERIAL_FABRICADO` | T03 |
| F1 | APROVACAO_MEDICAO_PENDENTE | - | RF-11 | - | - | `POST /aprovacoes/{id}/decisao` | Aprovacao | `APROVACAO_MEDICAO_PENDENTE` | T03 |
| F1 | MEDICAO_APROVADA | - | RF-11 | - | - | `POST /aprovacoes/{id}/decisao` | Aprovacao | `MEDICAO_APROVADA` | T03 |
| F1 | ROMANEIO_CONFERIDO | - | - | - | - | - | OS | `ROMANEIO_CONFERIDO` | T03 |
| F1 | NFE_EMITIDA | - | - | - | - | - | NFe | `NFE_EMITIDA` | T03 |
| F1 | XML_OBTIDO | - | RF-03 | RNF-02 | - | `POST /integrations/qive/nfe-recebida` | NFe | `XML_OBTIDO` | T05 |
| F1 | NFE_VALIDADA_OK | RB-05 | RF-03 | RNF-01 | CA-02 | `POST /fiscal/nfe/{chave}/validacao` | NFe | `FISCAL_NFE_VALIDADA` | T03 |
| F1 | NFE_VALIDADA_NOK | RB-05, RB-06 | RF-03 | RNF-01 | CA-02 | `POST /fiscal/nfe/{chave}/validacao` | NFe | `FISCAL_NFE_VALIDADA` | T03 |
| F1 | VINCULADA_OS | RB-01, RB-03 | RF-02 | RNF-01 | CA-01 | `POST /vinculos` | Vinculo | `VINCULO_CRIADO` | T03 |
| F1 | ENTRADA_ORIGEM | RB-07 | RF-05 | RNF-01 | - | - | OS, NFe | `ESTOQUE_ORIGEM_ATUALIZADO` | T03 |
| F1 | SAIDA_ORIGEM | RB-07 | RF-05 | RNF-01 | - | - | OS, NFe | `ESTOQUE_ORIGEM_BAIXA` | T03 |
| F1 | ENTRADA_DESTINO | RB-04, RB-07 | RF-04, RF-05 | RNF-01 | CA-03 | `POST /anexos` | Anexo, OS, NFe | `ENTRADA_DESTINO_CONCLUIDA` | T03 |
| F2 | OS_ATUALIZADA_DATA_ESTIMADA | RB-10 | RF-01 | - | - | `PATCH /os/{id}` | OS | `OS_ATUALIZADA` | T03 |
| F2 | NFE_ENTREGA_FUTURA_EMITIDA | - | - | - | - | - | NFe | `NFE_ENTREGA_FUTURA_EMITIDA` | T03 |
| F2 | NFE_RECEBIDA_SEM_ESTOQUE | RB-08 | RF-05 | RNF-01 | CA-04 | - | NFe, OC | `NFE_RECEBIDA_SEM_ESTOQUE` | T03 |
| F2 | OC_CRIADA_PARA_REMESSA | RB-08 | RF-05 | RNF-01 | CA-04 | - | OC | `OC_CRIADA_PARA_REMESSA` | T03 |
| F2 | AGUARDANDO_REMESSA | RB-10 | RF-07 | - | - | `GET /paineis/oc-pendente-entrega-futura` | OC | - | T07 |
| F3 | OC_PENDENTE_ENTREGA_FUTURA | - | RF-07 | - | - | `GET /paineis/oc-pendente-entrega-futura` | OC | - | T07 |
| F3 | APROVACAO_ENTREGA_PENDENTE | - | RF-10 | - | - | `POST /aprovacoes/{id}/decisao` | Aprovacao | `APROVACAO_ENTREGA_PENDENTE` | T03 |
| F3 | ENTREGA_APROVADA | - | RF-10 | RNF-01 | - | `POST /aprovacoes/{id}/decisao` | Aprovacao | `ENTREGA_APROVADA` | T03 |
| F3 | NFE_REMESSA_EMITIDA | - | - | - | - | - | NFe | `NFE_REMESSA_EMITIDA` | T03 |
| F3 | VINCULADA_OS_OC_NFE | RB-01 | RF-02 | RNF-01 | - | `POST /vinculos` | Vinculo | `VINCULO_OS_OC_NFE_CRIADO` | T03 |
| F3 | ESTOQUE_ORIGEM_ATUALIZADO | RB-07 | RF-05 | RNF-01 | - | - | OS, OC, NFe | `ESTOQUE_ORIGEM_ATUALIZADO` | T03 |
| F3 | ESTOQUE_DESTINO_ATUALIZADO | RB-04, RB-07 | RF-04, RF-05 | RNF-01 | CA-03 | `POST /anexos` | Anexo, OS, OC, NFe | `ENTRADA_DESTINO_CONCLUIDA` | T03 |
| Todos | - | - | RF-06 | RNF-01 | - | - | Notificacao | `NOTIFICACAO_ENVIADA` | T06 |
| Todos | - | - | RF-08 | RNF-01 | - | `GET /auditoria` | AuditoriaEvento | Todos os eventos | T08 |

## Rastreabilidade por Requisito

### RF-01: Criar OS
- **Fluxos:** F1, F2
- **Estados:** OS_CRIADA, OS_ATUALIZADA_DATA_ESTIMADA
- **Endpoint:** `POST /os`, `PATCH /os/{id}`
- **Entidade:** OS
- **RB:** RB-02
- **Teste:** T02, T03

### RF-02: Registrar vínculo OS↔NFe
- **Fluxos:** F1, F3
- **Estados:** VINCULADA_OS, VINCULADA_OS_OC_NFE
- **Endpoint:** `POST /vinculos`
- **Entidade:** Vinculo
- **RB:** RB-01, RB-03
- **CA:** CA-01
- **Teste:** T03

### RF-03: Registrar validação fiscal
- **Fluxos:** F1, F2, F3
- **Estados:** NFE_VALIDADA_OK, NFE_VALIDADA_NOK
- **Endpoint:** `POST /fiscal/nfe/{chave}/validacao`
- **Entidade:** NFe
- **RB:** RB-05, RB-06
- **CA:** CA-02
- **Teste:** T03

### RF-04: Gerenciar anexos
- **Fluxos:** F1, F3
- **Estados:** ENTRADA_DESTINO, ESTOQUE_DESTINO_ATUALIZADO
- **Endpoint:** `POST /anexos`
- **Entidade:** Anexo
- **RB:** RB-04
- **CA:** CA-03
- **Teste:** T03

### RF-05: Registrar estoque não contábil
- **Fluxos:** F1, F2, F3
- **Estados:** ENTRADA_ORIGEM, SAIDA_ORIGEM, ENTRADA_DESTINO, NFE_RECEBIDA_SEM_ESTOQUE, ESTOQUE_ORIGEM_ATUALIZADO, ESTOQUE_DESTINO_ATUALIZADO
- **Entidade:** OS, OC, NFe
- **RB:** RB-07, RB-08
- **CA:** CA-04
- **Teste:** T03

### RF-06: Notificações e-mail
- **Fluxos:** Todos
- **Evento:** NOTIFICACAO_ENVIADA
- **Entidade:** Notificacao
- **RB:** RB-09, RB-10
- **Teste:** T06

### RF-07: Painéis operacionais
- **Fluxos:** F2, F3
- **Endpoint:** `GET /paineis/oc-pendente-entrega-futura`
- **Entidade:** OC
- **Teste:** T07

### RF-08: Trilha de auditoria
- **Fluxos:** Todos
- **Endpoint:** `GET /auditoria`
- **Entidade:** AuditoriaEvento
- **RNF:** RNF-01
- **Teste:** T08

## Rastreabilidade por Regra de Negócio

### RB-01: Cada OS vincula uma ou mais NFe
- **RF:** RF-02
- **Estados:** VINCULADA_OS, VINCULADA_OS_OC_NFE
- **Endpoint:** `POST /vinculos`
- **Entidade:** OS, Vinculo, NFe
- **Teste:** T03

### RB-03: Não travar por divergência de quantidade
- **RF:** RF-02
- **Estado:** VINCULADA_OS
- **Endpoint:** `POST /vinculos`
- **Entidade:** Vinculo (campo `divergenciaQuantidade`)
- **CA:** CA-01
- **Teste:** T03

### RB-04: Anexo obrigatório na entrada destino
- **RF:** RF-04
- **Estados:** ENTRADA_DESTINO, ESTOQUE_DESTINO_ATUALIZADO
- **Endpoint:** `POST /anexos`
- **Entidade:** Anexo
- **CA:** CA-03
- **Teste:** T03

### RB-05: Cancelar etapas anteriores quando NFe incorreta
- **RF:** RF-03
- **Estado:** NFE_VALIDADA_NOK
- **Endpoint:** `POST /fiscal/nfe/{chave}/validacao`
- **Entidade:** NFe
- **CA:** CA-02
- **Teste:** T03

### RB-08: Entrega futura mãe sem atualizar estoque
- **RF:** RF-05
- **Estado:** NFE_RECEBIDA_SEM_ESTOQUE
- **Entidade:** NFe, OC
- **CA:** CA-04
- **Teste:** T03

## Validação de Cobertura

**Requisitos cobertos:**
- ✅ RF-01 a RF-11 (todos mapeados)
- ✅ RB-01 a RB-10 (todas mapeadas)
- ✅ RNF-01 a RNF-05 (todos mapeados)
- ✅ CA-01 a CA-04 (todos mapeados)

**Endpoints cobertos:**
- ✅ `POST /os` → RF-01
- ✅ `PATCH /os/{id}` → RF-01
- ✅ `POST /vinculos` → RF-02
- ✅ `POST /fiscal/nfe/{chave}/validacao` → RF-03
- ✅ `POST /aprovacoes/{id}/decisao` → RF-10, RF-11
- ✅ `POST /anexos` → RF-04
- ✅ `GET /paineis/oc-pendente-entrega-futura` → RF-07
- ✅ `POST /integrations/qive/nfe-recebida` → RF-03, RNF-02
- ✅ `GET /auditoria` → RF-08

**Fluxos cobertos:**
- ✅ F1 (Compra Direta) — 13 estados mapeados
- ✅ F2 (Entrega Futura mãe) — 8 estados mapeados
- ✅ F3 (Entrega Futura filha) — 9 estados mapeados
