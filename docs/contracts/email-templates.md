# Templates de E-mail — Transferência de Materiais Entre Filiais

> **Templates de Notificação por E-mail** — Assuntos, corpos e variáveis disponíveis.
> Referências: [RF-06](../specs/transferencia-materiais/SPEC.md#requisitos-funcionais-rf), [OPERATIONS.md](../OPERATIONS.md), [RB-09](../specs/transferencia-materiais/SPEC.md#regras-de-negócio-rb), [RB-10](../specs/transferencia-materiais/SPEC.md#regras-de-negócio-rb)

## Regras Gerais

- Todas as mensagens devem incluir:
  - Identificador principal (OS/OC/NFe)
  - `correlationId`
  - Link para o detalhe do processo/painel (URL absoluta)
- Variáveis no formato `{{variavel}}`.
- Texto (MVP): plain text. HTML é opcional e pode ser adicionado depois.

## Variáveis Comuns

- `{{processoUrl}}` — URL absoluta para o detalhe do processo/painel
- `{{correlationId}}` — ID de correlação (OS/OC/NFe/Vínculo)
- `{{osNumero}}`, `{{osId}}` — Número e ID da OS
- `{{ocNumero}}`, `{{ocId}}` — Número e ID da OC
- `{{nfeChaveAcesso}}` — Chave de acesso da NFe
- `{{filialDestinoId}}` — ID da filial destino
- `{{statusWorkflow}}` — Estado atual do workflow
- `{{dataEstimadaEntrega}}` — Data estimada de entrega
- `{{pendenciaTipo}}`, `{{pendenciaDescricao}}` — Tipo e descrição da pendência

---

## 1) CHEGADA_MATERIAL

**Assunto:** Chegada de material — OS {{osNumero}}

**Corpo:**
```
Olá,
Registramos a chegada do material referente à OS {{osNumero}}.

- Filial destino: {{filialDestinoId}}
- Status atual: {{statusWorkflow}}
- Correlation ID: {{correlationId}}

Acesse o processo: {{processoUrl}}
```

**Destinatários:** Adm. Filial Destino  
**Critério:** Baseado em `os.filialDestinoId`

---

## 2) NFE_ENTRADA_DISPONIVEL

**Assunto:** NFe de entrada disponível — OS {{osNumero}}

**Corpo:**
```
Olá,
A NFe de entrada está disponível para o processo OS {{osNumero}}.

- Chave NFe: {{nfeChaveAcesso}}
- Status atual: {{statusWorkflow}}
- Correlation ID: {{correlationId}}

Acesse o processo: {{processoUrl}}
```

**Destinatários:** Adm. Filial Destino  
**Critério:** Baseado em `os.filialDestinoId`

---

## 3) NFE_SAIDA_PRONTA_IMPRESSAO

**Assunto:** NFe de saída pronta para impressão — OS {{osNumero}}

**Corpo:**
```
Olá,
A NFe de saída está pronta para impressão no processo OS {{osNumero}}.

- Chave NFe: {{nfeChaveAcesso}}
- Correlation ID: {{correlationId}}

Acesse o processo: {{processoUrl}}
```

**Destinatários:** Adm. Filial Origem  
**Critério:** Baseado em `os.filialOrigemId` (se existir) ou configuração

---

## 4) PENDENCIA_ABERTA

**Assunto:** Pendência aberta ({{pendenciaTipo}}) — OS {{osNumero}}

**Corpo:**
```
Olá,
Foi aberta uma pendência no processo OS {{osNumero}}.

- Tipo: {{pendenciaTipo}}
- Descrição: {{pendenciaDescricao}}
- Status atual: {{statusWorkflow}}
- Correlation ID: {{correlationId}}

Acesse o processo: {{processoUrl}}
```

**Destinatários:** Responsável pela pendência (`pendencia.ownerRole`)  
**Critério:** Baseado em `pendencia.ownerRole`

---

## 5) CANCELAMENTO_PROCESSO

**Assunto:** Processo cancelado — OS {{osNumero}}

**Corpo:**
```
Olá,
O processo OS {{osNumero}} foi cancelado.

- Motivo: {{pendenciaDescricao}}
- Correlation ID: {{correlationId}}

Acesse o processo: {{processoUrl}}
```

**Destinatários:** Responsáveis (Gestor do Contrato, Adm. Filial Origem, Adm. Filial Destino)  
**Critério:** Baseado em papéis relacionados ao processo

---

## 6) LEMBRETE_7_DIAS_ENTREGA_ESTIMADA

**Assunto:** Lembrete: 7 dias para entrega estimada — OS {{osNumero}}

**Corpo:**
```
Olá,
Faltam 7 dias para a data estimada de entrega do processo OS {{osNumero}}.

- Data estimada: {{dataEstimadaEntrega}}
- Status atual: {{statusWorkflow}}
- Correlation ID: {{correlationId}}

Acesse o processo: {{processoUrl}}
```

**Destinatários:** Gestor do Contrato  
**Critério:** Baseado em `os.dataEstimadaEntrega` (7 dias antes)  
**Regra:** [RB-10](../specs/transferencia-materiais/SPEC.md#regras-de-negócio-rb)

---

## 7) LEMBRETE_30_DIAS_DESTINO

**Assunto:** Alerta: 30 dias com ação pendente no destino — OS {{osNumero}}

**Corpo:**
```
Olá,
Há 30 dias existe ação pendente para a filial destino no processo OS {{osNumero}}.

- Filial destino: {{filialDestinoId}}
- Status atual: {{statusWorkflow}}
- Correlation ID: {{correlationId}}

Acesse o processo: {{processoUrl}}
```

**Destinatários:** Gestor do Contrato + Adm. Filial Destino  
**Critério:** Baseado em `os.filialDestinoId` e tempo desde criação  
**Regra:** [RB-09](../specs/transferencia-materiais/SPEC.md#regras-de-negócio-rb)

---

## 8) MEDICAO_CONCLUIDA

**Assunto:** Medição concluída — OS {{osNumero}}

**Corpo:**
```
Olá,
A medição do processo OS {{osNumero}} foi concluída.

- Status atual: {{statusWorkflow}}
- Correlation ID: {{correlationId}}

Acesse o processo: {{processoUrl}}
```

**Destinatários:** Gestor do Contrato  
**Critério:** Baseado em aprovação de medição  
**Nota:** TBD-04 fechado — Medição realizada no RM/Contratos com base na OS aberta no nFlow; notificação disparada quando integração atualiza estado.

---

## Tabela de Destinatários por Tipo

| Tipo de Notificação | Destinatários | Critério |
|---------------------|---------------|----------|
| `CHEGADA_MATERIAL` | Adm. Filial Destino | `os.filialDestinoId` |
| `NFE_ENTRADA_DISPONIVEL` | Adm. Filial Destino | `os.filialDestinoId` |
| `NFE_SAIDA_PRONTA_IMPRESSAO` | Adm. Filial Origem | `os.filialOrigemId` ou configuração |
| `PENDENCIA_ABERTA` | Responsável pela pendência | `pendencia.ownerRole` |
| `CANCELAMENTO_PROCESSO` | Responsáveis (Gestor, Adm. Origem, Adm. Destino) | Papéis relacionados ao processo |
| `LEMBRETE_7_DIAS_ENTREGA_ESTIMADA` | Gestor do Contrato | `os.dataEstimadaEntrega` (7 dias antes) |
| `LEMBRETE_30_DIAS_DESTINO` | Gestor do Contrato + Adm. Filial Destino | `os.filialDestinoId` + tempo desde criação |
| `MEDICAO_CONCLUIDA` | Gestor do Contrato | Aprovação de medição |

---

## Variáveis Disponíveis

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `{{processoUrl}}` | URL absoluta para o detalhe do processo/painel | `https://app.example.local/paineis/oc-pendente?osId=os_01JABC...` |
| `{{correlationId}}` | ID de correlação (OS/OC/NFe/Vínculo) | `os_01JABC...` |
| `{{osNumero}}` | Número da OS | `OS-2025-000123` |
| `{{osId}}` | ID da OS | `os_01JABC...` |
| `{{ocNumero}}` | Número da OC | `OC-12345` |
| `{{ocId}}` | ID da OC | `oc_01JDEF...` |
| `{{nfeChaveAcesso}}` | Chave de acesso da NFe | `35250123456789000123550010000012341000012345` |
| `{{filialDestinoId}}` | ID da filial destino | `FIL-DEST-01` |
| `{{statusWorkflow}}` | Estado atual do workflow | `ENTRADA_DESTINO_PENDENTE_ANEXO` |
| `{{dataEstimadaEntrega}}` | Data estimada de entrega | `2025-02-15` |
| `{{pendenciaTipo}}` | Tipo de pendência | `FALTA_ANEXO_OBRIGATORIO` |
| `{{pendenciaDescricao}}` | Descrição da pendência | `Anexo obrigatório faltante na entrada destino` |

---

## Regras de Implementação

1. **Variáveis:** Formato `{{variavel}}` (chaves duplas)
2. **Links:** Sempre absolutos (não relativos)
3. **Correlation ID:** Sempre presente no corpo
4. **Texto:** Plain text (MVP). HTML opcional para versões futuras
5. **Assunto:** Máximo 200 caracteres
6. **Footer:** Opcional no MVP (pode ser adicionado depois)

---

## Referências

- [OPERATIONS.md](../OPERATIONS.md) — Seção 2. Notificações por E-mail
- [SPEC.md](../specs/transferencia-materiais/SPEC.md#requisitos-funcionais-rf) — RF-06
- [data-model.md](../data-models/data-model.md) — Entidade Notificacao
- [auditoria-eventos.md](./auditoria-eventos.md) — Eventos `NOTIFICACAO_ENFILEIRADA`, `NOTIFICACAO_ENVIADA`, `NOTIFICACAO_FALHOU`
