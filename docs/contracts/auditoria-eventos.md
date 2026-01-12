# Catálogo de Eventos de Auditoria — Transferência de Materiais Entre Filiais

> **Catálogo de Eventos** — Padronização de `eventType` (enum) e payload mínimo por evento para rastreabilidade, troubleshooting e conformidade.
> Referências: [openapi.yaml](./openapi.yaml), [data-model.md](../data-models/data-model.md), [OPERATIONS.md](../OPERATIONS.md)

## 1. Fonte de Verdade

- **Enum:** `AuditoriaEventType` no OpenAPI (`docs/contracts/openapi.yaml` — `components/schemas/AuditoriaEventType`)
- **Modelo:** `auditoria_evento` em `docs/data-models/data-model.md` (Seção 3.8)
- **Requisito:** [RNF-01](../specs/transferencia-materiais/SPEC.md#requisitos-não-funcionais-rnf) — Auditoria imutável

## 2. Padrão de Payload

Todos os eventos devem incluir no payload, quando aplicável:

- **Identificadores:** `osId`, `ocId`, `nfeChaveAcesso`, `vinculoId`
- **Transições:** `fromStatus`, `toStatus` (para transições de estado)
- **Decisões:** `reason` (quando houver decisão/cancelamento)
- **Correlação:** `correlationType`, `correlationId` (já presente no evento, mas pode ser repetido no payload para facilitar consultas)

## 3. Eventos e Payload Mínimo

### 3.1 OS_CRIADA

**Payload mínimo:**
```json
{
  "osId": "os_01JABC...",
  "osNumero": "OS-2025-000123",
  "fluxType": "COMPRA_DIRETA",
  "filialDestinoId": "FIL-DEST-01",
  "quantidadePlanejada": 10
}
```

**Campos obrigatórios:** `osId`, `osNumero`, `fluxType`, `filialDestinoId`, `quantidadePlanejada`

---

### 3.2 OS_DATA_ESTIMADA_ATUALIZADA

**Payload mínimo:**
```json
{
  "osId": "os_01JABC...",
  "osNumero": "OS-2025-000123",
  "dataEstimadaAnterior": "2025-02-15",
  "dataEstimadaNova": "2025-02-20"
}
```

**Campos obrigatórios:** `osId`, `osNumero`, `dataEstimadaNova`  
**Campos opcionais:** `dataEstimadaAnterior` (null se primeira vez)

---

### 3.3 MATERIAL_FABRICADO

**Payload mínimo:**
```json
{
  "osId": "os_01JABC...",
  "osNumero": "OS-2025-000123",
  "fabricante": "Fabricante XYZ",
  "data": "2025-01-15T10:00:00Z"
}
```

**Campos obrigatórios:** `osId`, `osNumero`  
**Campos opcionais:** `fabricante`, `data`

---

### 3.4 ROMANEIO_CONFERIDO

**Payload mínimo:**
```json
{
  "osId": "os_01JABC...",
  "osNumero": "OS-2025-000123",
  "inspetorId": "user_123",
  "resultado": "APROVADO",
  "observacao": "Romaneio conferido, quantidade correta"
}
```

**Campos obrigatórios:** `osId`, `osNumero`, `resultado`  
**Campos opcionais:** `inspetorId`, `observacao`

---

### 3.5 NFE_EMITIDA

**Payload mínimo:**
```json
{
  "nfeChaveAcesso": "35250123456789000123550010000012341000012345",
  "tipo": "VENDA",
  "numero": "123",
  "serie": "1",
  "cnpjEmitente": "12345678000190",
  "cnpjDestinatario": "98765432000110"
}
```

**Campos obrigatórios:** `nfeChaveAcesso`, `tipo`  
**Campos opcionais:** `numero`, `serie`, `cnpjEmitente`, `cnpjDestinatario`

---

### 3.6 NFE_XML_OBTIDO

**Payload mínimo:**
```json
{
  "nfeChaveAcesso": "35250123456789000123550010000012341000012345",
  "xmlRef": "storage://xml/3525...2345.xml",
  "source": "QIVE",
  "idempotencyKey": "35250123456789000123550010000012341000012345"
}
```

**Campos obrigatórios:** `nfeChaveAcesso`, `xmlRef`, `source`  
**Campos opcionais:** `idempotencyKey` (sugestão: usar `chaveAcesso`)

**Nota:** `source` indica origem da integração (ex.: `QIVE`, `RM`, `MANUAL`)

---

### 3.7 FISCAL_NFE_VALIDADA

**Payload mínimo:**
```json
{
  "nfeChaveAcesso": "35250123456789000123550010000012341000012345",
  "decisao": "INCORRETA",
  "motivoCategoria": "EMISSAO_NFE",
  "motivoDetalhe": "CFOP incorreto na remessa"
}
```

**Campos obrigatórios:** `nfeChaveAcesso`, `decisao`  
**Campos opcionais:** `motivoCategoria`, `motivoDetalhe` (obrigatórios se `decisao = INCORRETA`)

**Valores de `decisao`:** `CORRETA`, `INCORRETA`  
**Valores de `motivoCategoria`:** `EMISSAO_NFE`, `VINCULO_ADM_FILIAL`

---

### 3.8 VINCULO_CRIADO

**Payload mínimo:**
```json
{
  "vinculoId": "vin_01JXYZ...",
  "osId": "os_01JABC...",
  "ocId": null,
  "nfeChaveAcesso": "35250123456789000123550010000012341000012345",
  "divergenciaQuantidade": -1
}
```

**Campos obrigatórios:** `vinculoId`, `osId`, `nfeChaveAcesso`  
**Campos opcionais:** `ocId` (obrigatório no fluxo F3), `divergenciaQuantidade`

**Nota:** `divergenciaQuantidade` pode ser negativo (não bloqueante — [RB-03](../specs/transferencia-materiais/SPEC.md#regras-de-negócio-rb))

---

### 3.9 ERRO_VINCULO_IDENTIFICADO

**Payload mínimo:**
```json
{
  "vinculoId": "vin_01JXYZ...",
  "osId": "os_01JABC...",
  "ocId": null,
  "nfeChaveAcesso": "35250123456789000123550010000012341000012345",
  "descricao": "NFe não corresponde à OS: filial destino divergente"
}
```

**Campos obrigatórios:** `osId`, `descricao`  
**Campos opcionais:** `vinculoId`, `ocId`, `nfeChaveAcesso`

---

### 3.10 VINCULO_CORRIGIDO

**Payload mínimo:**
```json
{
  "vinculoId": "vin_01JXYZ...",
  "osId": "os_01JABC...",
  "ocId": null,
  "nfeChaveAcesso": "35250123456789000123550010000012341000012345",
  "ajusteDescricao": "Vínculo corrigido: NFe correta vinculada"
}
```

**Campos obrigatórios:** `vinculoId`, `osId`, `nfeChaveAcesso`  
**Campos opcionais:** `ocId`, `ajusteDescricao`

---

### 3.11 ESTOQUE_ORIGEM_ATUALIZADO

**Payload mínimo:**
```json
{
  "osId": "os_01JABC...",
  "filialOrigemId": "FIL-ORIG-01",
  "quantidade": 10,
  "movimento": "ENTRADA_ORIGEM"
}
```

**Campos obrigatórios:** `osId`, `quantidade`, `movimento`  
**Campos opcionais:** `filialOrigemId`

**Valores de `movimento`:** `ENTRADA_ORIGEM`, `SAIDA_ORIGEM`

**Nota:** Atualização de estoque não contábil ([RB-07](../specs/transferencia-materiais/SPEC.md#regras-de-negócio-rb))

---

### 3.12 NFE_SAIDA_ORIGEM_EMITIDA

**Payload mínimo:**
```json
{
  "osId": "os_01JABC...",
  "nfeChaveAcesso": "35250123456789000123550010000012341000012345",
  "filialOrigemId": "FIL-ORIG-01"
}
```

**Campos obrigatórios:** `osId`, `nfeChaveAcesso`  
**Campos opcionais:** `filialOrigemId`

---

### 3.13 CHEGADA_MATERIAL_DESTINO

**Payload mínimo:**
```json
{
  "osId": "os_01JABC...",
  "filialDestinoId": "FIL-DEST-01",
  "dataChegada": "2025-01-20T14:30:00Z"
}
```

**Campos obrigatórios:** `osId`, `filialDestinoId`  
**Campos opcionais:** `dataChegada`

---

### 3.14 ANEXO_ADICIONADO

**Payload mínimo:**
```json
{
  "anexoId": "anexo_01JDEF...",
  "correlationType": "OS",
  "correlationId": "os_01JABC...",
  "tipo": "NFE_ASSINADA_CONFERIDA",
  "storageRef": "s3://bucket/key.pdf",
  "fileName": "NFe_assinada.pdf"
}
```

**Campos obrigatórios:** `anexoId`, `correlationType`, `correlationId`, `tipo`, `storageRef`  
**Campos opcionais:** `fileName`

**Nota:** Anexo obrigatório na entrada destino ([RB-04](../specs/transferencia-materiais/SPEC.md#regras-de-negócio-rb))

---

### 3.15 ENTRADA_DESTINO_CONCLUIDA

**Payload mínimo:**
```json
{
  "osId": "os_01JABC...",
  "filialDestinoId": "FIL-DEST-01",
  "anexosCount": 1,
  "checkAnexoObrigatorio": true
}
```

**Campos obrigatórios:** `osId`, `filialDestinoId`, `anexosCount`, `checkAnexoObrigatorio`

**Nota:** `checkAnexoObrigatorio` deve ser `true` para permitir conclusão ([RB-04](../specs/transferencia-materiais/SPEC.md#regras-de-negócio-rb))

---

### 3.16 PENDENCIA_ABERTA

**Payload mínimo:**
```json
{
  "pendenciaId": "pend_01JGHI...",
  "tipo": "FALTA_ANEXO_OBRIGATORIO",
  "status": "ABERTA",
  "correlationType": "OS",
  "correlationId": "os_01JABC...",
  "descricao": "Anexo obrigatório faltante na entrada destino"
}
```

**Campos obrigatórios:** `pendenciaId`, `tipo`, `status`, `correlationType`, `correlationId`, `descricao`

---

### 3.17 PENDENCIA_RESOLVIDA

**Payload mínimo:**
```json
{
  "pendenciaId": "pend_01JGHI...",
  "tipo": "FALTA_ANEXO_OBRIGATORIO",
  "status": "RESOLVIDA",
  "correlationType": "OS",
  "correlationId": "os_01JABC...",
  "descricao": "Anexo obrigatório adicionado"
}
```

**Campos obrigatórios:** `pendenciaId`, `tipo`, `status`, `correlationType`, `correlationId`, `descricao`

---

### 3.18 NOTIFICACAO_ENFILEIRADA

**Payload mínimo:**
```json
{
  "notificacaoId": "notif_01JKLM...",
  "tipo": "CHEGADA_MATERIAL",
  "to": ["adm.destino@example.com"],
  "subject": "Material chegou na filial destino — OS OS-2025-000123"
}
```

**Campos obrigatórios:** `notificacaoId`, `tipo`, `to`, `subject`

---

### 3.19 NOTIFICACAO_ENVIADA

**Payload mínimo:**
```json
{
  "notificacaoId": "notif_01JKLM...",
  "tipo": "CHEGADA_MATERIAL",
  "to": ["adm.destino@example.com"],
  "subject": "Material chegou na filial destino — OS OS-2025-000123",
  "providerMessageId": "msg-123456"
}
```

**Campos obrigatórios:** `notificacaoId`, `tipo`, `to`, `subject`  
**Campos opcionais:** `providerMessageId`

---

### 3.20 NOTIFICACAO_FALHOU

**Payload mínimo:**
```json
{
  "notificacaoId": "notif_01JKLM...",
  "tipo": "CHEGADA_MATERIAL",
  "to": ["adm.destino@example.com"],
  "subject": "Material chegou na filial destino — OS OS-2025-000123",
  "erro": "SMTP connection timeout"
}
```

**Campos obrigatórios:** `notificacaoId`, `tipo`, `to`, `subject`, `erro`

---

### 3.21 WORKFLOW_TRANSICAO

**Payload mínimo:**
```json
{
  "correlationType": "OS",
  "correlationId": "os_01JABC...",
  "fromStatus": "VINCULO_CRIADO",
  "toStatus": "ESTOQUE_ORIGEM_ATUALIZADO",
  "actorRole": "ADM_ORIGEM",
  "reason": "Estoque atualizado após vínculo confirmado"
}
```

**Campos obrigatórios:** `correlationType`, `correlationId`, `fromStatus`, `toStatus`, `actorRole`  
**Campos opcionais:** `reason`

**Nota:** Este evento é gerado automaticamente em toda transição de estado do workflow.

---

### 3.22 PROCESSO_CANCELADO

**Payload mínimo:**
```json
{
  "correlationType": "OS",
  "correlationId": "os_01JABC...",
  "motivo": "NFe incorreta: CFOP inválido",
  "referenciaPendenciaId": "pend_01JGHI..."
}
```

**Campos obrigatórios:** `correlationType`, `correlationId`, `motivo`  
**Campos opcionais:** `referenciaPendenciaId`

---

## 4. Exemplos de Uso

### 4.1 Consulta de Eventos por CorrelationId

```bash
GET /auditoria?correlationId=os_01JABC...&eventType=FISCAL_NFE_VALIDADA
```

**Resposta:**
```json
{
  "items": [
    {
      "id": "aud_01JXYZ...",
      "eventType": "FISCAL_NFE_VALIDADA",
      "correlationType": "OS",
      "correlationId": "os_01JABC...",
      "actorRole": "FISCAL",
      "actorId": "user_123",
      "timestamp": "2025-01-12T14:30:00Z",
      "payload": {
        "nfeChaveAcesso": "35250123456789000123550010000012341000012345",
        "decisao": "INCORRETA",
        "motivoCategoria": "EMISSAO_NFE",
        "motivoDetalhe": "CFOP incorreto na remessa"
      }
    }
  ]
}
```

### 4.2 Rastreabilidade Completa de um Processo

```bash
GET /auditoria?correlationId=os_01JABC...
```

Retorna todos os eventos relacionados à OS, ordenados por `timestamp`.

---

## 5. Regras de Implementação

1. **Imutabilidade:** Eventos nunca são atualizados ou deletados ([RNF-01](../specs/transferencia-materiais/SPEC.md#requisitos-não-funcionais-rnf))
2. **Payload mínimo:** Sempre incluir campos obrigatórios; campos opcionais apenas quando disponíveis
3. **Correlação:** `correlationType` e `correlationId` devem estar sempre presentes (no evento e, quando aplicável, no payload)
4. **Timestamp:** Sempre em UTC (ISO 8601)
5. **Actor:** `actorRole` obrigatório; `actorId` opcional (pode ser null para eventos automáticos)

---

## 6. Referências

- [openapi.yaml](./openapi.yaml) — Schema `AuditoriaEventType` e `AuditoriaEvento`
- [data-model.md](../data-models/data-model.md) — Tabela `auditoria_evento`
- [OPERATIONS.md](../OPERATIONS.md) — Seção 4. Auditoria
- [SPEC.md](../specs/transferencia-materiais/SPEC.md) — RNF-01, RF-08
