# Integração Qive ↔ RM — Contrato e Stub (MVP)

> **Status:** Stub/MVP — Implementação real pendente (TBD-01)  
> **Objetivo:** Documentar o contrato mínimo para a integração de eventos de NFe (via Qive) e interações com RM, incluindo um **stub** testável localmente enquanto TBD-01 não for definido.

## 1) Objetivo

Documentar o contrato mínimo para a integração de eventos de NFe (via Qive) e interações com RM, incluindo um **stub** testável localmente enquanto TBD-01 não for definido.

**Referências:**
- Modelo de dados: [`docs/data-models/data-model.md`](../data-models/data-model.md) (`nfe`, `processed_message`)
- OpenAPI: [`docs/contracts/openapi.yaml`](./openapi.yaml)
- Auditoria: [`docs/contracts/auditoria-eventos.md`](./auditoria-eventos.md)

## 2) Escopo (MVP)

- Receber eventos de NFe (XML/metadata) originados do Qive.
- Registrar idempotência (não processar evento repetido).
- Persistir NFe e, quando aplicável, produzir transição no workflow / abrir pendência.
- Integração RM: **stub** (comportamento definido aqui) até definição do contrato real (TBD-01).

## 3) Identificadores e Idempotência

### 3.1 IdempotencyKey

- O sistema deve aceitar um `idempotencyKey` por evento recebido.
- Persistir em `processed_message`:
  - `idempotencyKey`, `source=QIVE`, `receivedAt`, `result`
- Se a chave já existir:
  - retornar sucesso idempotente (`200` ou `202`)
  - não reprocessar efeitos colaterais (não duplicar NFe, não duplicar auditoria, não duplicar notificações)

Referência: [`docs/data-models/data-model.md`](../data-models/data-model.md) (entidade `ProcessedMessage`)

## 4) Contrato de Entrada (Evento Recebido do Qive) — MVP

### 4.1 Payload Esperado

```json
{
  "idempotencyKey": "qive-evt-000001",
  "eventType": "NFE_XML_AVAILABLE",
  "timestamp": "2025-01-12T12:10:00Z",
  "nfe": {
    "chaveAcesso": "3525...2345",
    "tipo": "ENTRADA_DESTINO",
    "xmlBase64": "PD94bWwgdmVyc2lvbj0iMS4wIj8+..."
  },
  "context": {
    "osNumero": "OS-98765",
    "ocNumero": "OC-12345"
  }
}
```

### 4.2 Processamento Mínimo

1. **Validar idempotencyKey** (obrigatório).

2. **Extrair nfe.chaveAcesso e persistir/atualizar nfe:**
   - `xmlRef` pode ser armazenado após gravar o XML no storage.

3. **Registrar auditoria:**
   - `NFE_XML_OBTIDO` com `source=QIVE` e `idempotencyKey`.

4. **Se houver mapeamento para processo (OS/OC):**
   - atualizar workflow para `XML_OBTIDO` (via `WORKFLOW_TRANSICAO`)

5. **Se não houver mapeamento:**
   - abrir pendência `INTEGRACAO_FALHOU` ou `ERRO_VINCULO` (conforme regra do domínio) e registrar auditoria.

## 5) Contrato de Saída / Interação com RM (Stub)

Enquanto TBD-01 não for definido, o RM deve ser encapsulado em um adapter `IRmAdapter`.

### 5.1 Operações Mínimas do Adapter

- `RegisterStockMovement(movement)` (RF-05 / estoque não contábil)
- `RegisterLink(os, oc, nfe)` (quando aplicável ao domínio)
- `GetProcessData(osNumero|ocNumero)` (opcional)

### 5.2 Comportamento do Stub (MVP)

Controlado por variável de ambiente `INTEGRATION_MODE`.

**`INTEGRATION_MODE=stub`:**
- Todas as operações retornam sucesso determinístico.
- Possibilidade de simular falha via `STUB_FAIL_RATE` ou `STUB_FORCE_FAIL=true`.
- Em caso de falha simulada:
  - registrar pendência `INTEGRACAO_FALHOU`
  - registrar auditoria (tipo a definir; usar `INTEGRACAO_FALHOU` se existir ou `PENDENCIA_ABERTA`)

**`INTEGRATION_MODE=real`:**
- Usa endpoints reais definidos quando TBD-01 for fechado.

## 6) Configuração (env vars)

- `INTEGRATION_MODE` = `stub` | `real`
- `QIVE_WEBHOOK_SECRET` = segredo de validação (quando aplicável)
- `STUB_FORCE_FAIL` = `true`|`false` (somente stub)
- `STUB_FAIL_RATE` = `0..1` (somente stub)
- `RM_API_BASE_URL` (somente real)
- `RM_API_TOKEN` (somente real)

## 7) Como Testar Localmente (Stub)

1. **Subir API + banco.**

2. **Setar `INTEGRATION_MODE=stub`.**

3. **POST do evento Qive para o endpoint de ingestão** (quando existir):

**Verificar:**
- `nfe` persistida
- `processed_message` gravado
- auditoria `NFE_XML_OBTIDO`
- transição para `XML_OBTIDO` quando houver correlação

## 8) Pontos TBD

- **TBD-01:** contrato real Qive↔RM (endpoints, autenticação, campos obrigatórios)
- Regras de correlação (OS/OC) a partir de evento Qive podem exigir ajustes conforme origem dos dados.

Referência: [`docs/specs/transferencia-materiais/TBD.md`](../specs/transferencia-materiais/TBD.md)

---

**Desenvolvido por [ness.](https://github.com/resper1965/nTransfer)**
