# Examples — Transferência de Materiais Entre Filiais

> Exemplos práticos de payloads para API, auditoria e erros.

## 1) Auditoria — WORKFLOW_TRANSICAO
```json
{
  "id": "b8d2e8a7-7c7b-4b52-9f4a-8d2dfb7e3b01",
  "eventType": "WORKFLOW_TRANSICAO",
  "correlationType": "OS",
  "correlationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "actorRole": "SISTEMA",
  "actorId": null,
  "timestamp": "2025-01-12T12:10:00Z",
  "payload": {
    "fromStatus": "NFE_EMITIDA",
    "toStatus": "XML_OBTIDO",
    "reason": "XML recebido via Qive"
  }
}
```

## 2) Auditoria — FISCAL_NFE_VALIDADA (CORRETA)
```json
{
  "id": "d7edc2f4-2a3d-4c9b-b9d9-2c8e3c5d1a22",
  "eventType": "FISCAL_NFE_VALIDADA",
  "correlationType": "NFE",
  "correlationId": "35250123456789000123550010000012341000012345",
  "actorRole": "FISCAL",
  "actorId": "user:ricardo",
  "timestamp": "2025-01-12T12:20:00Z",
  "payload": {
    "nfeChaveAcesso": "35250123456789000123550010000012341000012345",
    "decisao": "CORRETA"
  }
}
```

## 3) Auditoria — FISCAL_NFE_VALIDADA (INCORRETA)
```json
{
  "id": "2f5a6f10-3b1c-4f52-bbde-4e6f8a5a1001",
  "eventType": "FISCAL_NFE_VALIDADA",
  "correlationType": "NFE",
  "correlationId": "35250123456789000123550010000012341000012345",
  "actorRole": "FISCAL",
  "actorId": "user:ricardo",
  "timestamp": "2025-01-12T12:23:00Z",
  "payload": {
    "nfeChaveAcesso": "35250123456789000123550010000012341000012345",
    "decisao": "INCORRETA",
    "motivoCategoria": "EMISSAO_NFE",
    "motivoDetalhe": "CFOP incompatível com o fluxo."
  }
}
```

## 4) Auditoria — VINCULO_CRIADO com divergência não bloqueante
```json
{
  "id": "a11d21f9-5b30-41b1-a33a-55f8d7f2a901",
  "eventType": "VINCULO_CRIADO",
  "correlationType": "VINCULO",
  "correlationId": "e5c9a4c9-3170-49ce-8b8e-9b0f33f0c111",
  "actorRole": "ADM_FILIAL_ORIGEM",
  "actorId": "user:operador01",
  "timestamp": "2025-01-12T12:35:00Z",
  "payload": {
    "vinculoId": "e5c9a4c9-3170-49ce-8b8e-9b0f33f0c111",
    "osId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "ocId": null,
    "nfeChaveAcesso": "35250123456789000123550010000012341000012345",
    "divergenciaQuantidade": -1.0
  }
}
```

## 5) Auditoria — ANEXO_ADICIONADO
```json
{
  "id": "7a8b9c10-1112-1314-1516-171819202122",
  "eventType": "ANEXO_ADICIONADO",
  "correlationType": "OS",
  "correlationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "actorRole": "ADM_FILIAL_DESTINO",
  "actorId": "user:destino01",
  "timestamp": "2025-01-12T13:05:00Z",
  "payload": {
    "anexoId": "1b2c3d4e-5555-6666-7777-888899990000",
    "correlationType": "OS",
    "correlationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "tipo": "NFE_ASSINADA_CONFERIDA",
    "storageRef": "storage://anexos/os/3fa85.../nfe_assinada.pdf",
    "fileName": "nfe_assinada.pdf"
  }
}
```

## 6) Erro 400 — BadRequest (validação de request)
```json
{
  "code": "BAD_REQUEST",
  "message": "Campo obrigatório ausente.",
  "correlationId": "OS:3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "details": {
    "missing": ["nfeChaveAcesso"]
  }
}
```

## 7) Erro 409 — Conflict (idempotência / vínculo duplicado)
```json
{
  "code": "CONFLICT",
  "message": "Vínculo já existe para esta OS/NFe/OC.",
  "correlationId": "OS:3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "details": {
    "constraint": "UQ_vinculo_os_oc_nfe",
    "existingVinculoId": "e5c9a4c9-3170-49ce-8b8e-9b0f33f0c111"
  }
}
```

## 8) Erro 422 — UnprocessableEntity (regra de negócio)
```json
{
  "code": "BUSINESS_RULE_VIOLATION",
  "message": "Não é possível concluir a entrada no destino.",
  "correlationId": "OS:3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "details": {
    "statusWorkflow": "ENTRADA_DESTINO_PENDENTE_ANEXO"
  },
  "violations": [
    {
      "field": "anexos",
      "rule": "ANEXO_OBRIGATORIO",
      "message": "Anexe o documento obrigatório (ex.: NFE_ASSINADA_CONFERIDA) para concluir."
    }
  ]
}
```

## 9) Erro 403 — Forbidden (RBAC)
```json
{
  "code": "FORBIDDEN",
  "message": "Você não possui permissão para executar esta ação.",
  "correlationId": "OS:3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "details": {
    "requiredRole": "FISCAL",
    "currentRole": "ADM_FILIAL_DESTINO"
  }
}
```

---

**Referências:**
- Catálogo completo de eventos: [auditoria-eventos.md](../../contracts/auditoria-eventos.md)
- Schemas de erro: [openapi.yaml](../../contracts/openapi.yaml) (`ErrorResponse`, `ValidationErrorResponse`)

**Desenvolvido por [ness.](https://github.com/resper1965/nTransfer)**
