# Workflow Transitions — Transferência de Materiais Entre Filiais

> **Objetivo:** Documentar transições do workflow com condições, responsáveis, validações e efeitos colaterais (auditoria, pendências, notificações).

**Fonte de verdade de estados:**
- [`docs/specs/transferencia-materiais/workflow-states.md`](./workflow-states.md)
- [`docs/data-models/data-model.md`](../../data-models/data-model.md) (`WorkflowStatus`)
- [`docs/contracts/openapi.yaml`](../../contracts/openapi.yaml) (`WorkflowStatus`)

## Convenções

- **Responsáveis (papéis):** `FABRICA`, `FISCAL`, `ADM_FILIAL_ORIGEM`, `ADM_FILIAL_DESTINO`, `GESTOR_CONTRATO`, `SISTEMA`.
- **Auditoria mínima:**
  - Sempre registrar `WORKFLOW_TRANSICAO` com `{fromStatus,toStatus,reason?}`.
  - Registrar evento específico quando aplicável (ver [`docs/contracts/auditoria-eventos.md`](../../contracts/auditoria-eventos.md)).
- **Notificações:**
  - Conteúdo em [`docs/contracts/email-templates.md`](../../contracts/email-templates.md).
  - Disparo/destinatários em [`docs/OPERATIONS.md`](../../OPERATIONS.md).

---

## A) Transições Comuns (Compartilhadas)

| From | To | Condição | Responsável | Validações | Efeitos (mínimo) |
|---|---|---|---|---|---|
| OS_CRIADA | ROMANEIO_CONFERIDO | Romaneio conferido | ADM_FILIAL_ORIGEM ou FABRICA | OS existe e está ativa | Auditoria: `ROMANEIO_CONFERIDO`, `WORKFLOW_TRANSICAO` |
| ROMANEIO_CONFERIDO | NFE_EMITIDA | NFe emitida no contexto do processo | ADM_FILIAL_ORIGEM | Chave NFe presente | Auditoria: `NFE_EMITIDA`, `WORKFLOW_TRANSICAO` |
| NFE_EMITIDA | XML_OBTIDO | XML obtido (Qive/integração) | SISTEMA | idempotencyKey válido; XML presente | Auditoria: `NFE_XML_OBTIDO`, `WORKFLOW_TRANSICAO` |
| XML_OBTIDO | NFE_VALIDADA_OK | Fiscal valida como correta | FISCAL | XML parseável; decisão registrada | Auditoria: `FISCAL_NFE_VALIDADA`, `WORKFLOW_TRANSICAO` |
| XML_OBTIDO | NFE_VALIDADA_NOK | Fiscal valida como incorreta | FISCAL | motivoCategoria + motivoDetalhe (quando aplicável) | Auditoria: `FISCAL_NFE_VALIDADA`, `PENDENCIA_ABERTA` (tipo `NFE_INCORRETA`), `WORKFLOW_TRANSICAO`; Notificação: `PENDENCIA_ABERTA` |
| NFE_VALIDADA_NOK | CORRECAO_EMISSAO_OU_VINCULO | Pendência criada para correção | SISTEMA | pendência existente | Auditoria: `PENDENCIA_ABERTA`, `WORKFLOW_TRANSICAO` |
| CORRECAO_EMISSAO_OU_VINCULO | NFE_EMITIDA | Correção realizada e nova emissão | ADM_FILIAL_ORIGEM ou ADM_FILIAL_DESTINO (depende motivo) | pendência resolvida | Auditoria: `PENDENCIA_RESOLVIDA`, `NFE_EMITIDA`, `WORKFLOW_TRANSICAO` |
| * | PROCESSO_CANCELADO | Cancelamento operacional | GESTOR_CONTRATO (ou papel definido) | motivo obrigatório | Auditoria: `PROCESSO_CANCELADO`, `WORKFLOW_TRANSICAO`; Notificação: `CANCELAMENTO_PROCESSO` |

---

## B) Fluxo F1 — Compra Direta

| From | To | Condição | Responsável | Validações | Efeitos (mínimo) |
|---|---|---|---|---|---|
| OS_CRIADA | MATERIAL_FABRICADO | Fabricação concluída | FABRICA | OS ativa | Auditoria: `MATERIAL_FABRICADO`, `WORKFLOW_TRANSICAO` |
| NFE_VALIDADA_OK | VINCULO_CRIADO | Vínculo OS↔NFe criado | ADM_FILIAL_ORIGEM | idempotência (unique os/oc/nfe); registrar divergência se existir | Auditoria: `VINCULO_CRIADO`, `WORKFLOW_TRANSICAO` |
| VINCULO_CRIADO | ESTOQUE_ORIGEM_ATUALIZADO | Registro estoque não contábil (origem) | SISTEMA | RF-05 (detalhe pode depender de integração RM) | Auditoria: `ESTOQUE_ORIGEM_ATUALIZADO`, `WORKFLOW_TRANSICAO` |
| ESTOQUE_ORIGEM_ATUALIZADO | NFE_SAIDA_ORIGEM_EMITIDA | NFe de saída emitida/confirmada | ADM_FILIAL_ORIGEM | chave NFe saída válida | Auditoria: `NFE_SAIDA_ORIGEM_EMITIDA`, `WORKFLOW_TRANSICAO`; Notificação: `NFE_SAIDA_PRONTA_IMPRESSAO` |
| NFE_SAIDA_ORIGEM_EMITIDA | EM_TRANSITO | Material saiu da origem | ADM_FILIAL_ORIGEM | - | Auditoria: `WORKFLOW_TRANSICAO` |
| EM_TRANSITO | CHEGADA_MATERIAL_DESTINO | Chegada confirmada | ADM_FILIAL_DESTINO | - | Auditoria: `CHEGADA_MATERIAL_DESTINO`, `WORKFLOW_TRANSICAO`; Notificação: `CHEGADA_MATERIAL` |
| CHEGADA_MATERIAL_DESTINO | ENTRADA_DESTINO_PENDENTE_ANEXO | Processo exige anexo para concluir entrada | SISTEMA | definir tipo(s) de anexo obrigatório(s) (ex.: `NFE_ASSINADA_CONFERIDA`) | Auditoria: `WORKFLOW_TRANSICAO` |
| ENTRADA_DESTINO_PENDENTE_ANEXO | ENTRADA_DESTINO_CONCLUIDA | Anexo obrigatório anexado e conferido | ADM_FILIAL_DESTINO | anexo(s) obrigatórios presentes | Auditoria: `ANEXO_ADICIONADO` (ao anexar), `ENTRADA_DESTINO_CONCLUIDA`, `WORKFLOW_TRANSICAO`; Notificação: `NFE_ENTRADA_DISPONIVEL` (se aplicável) |
| ENTRADA_DESTINO_CONCLUIDA | ESTOQUE_DESTINO_ATUALIZADO | Registro estoque não contábil (destino) | SISTEMA | RF-05 | Auditoria: `WORKFLOW_TRANSICAO` |
| ESTOQUE_DESTINO_ATUALIZADO | PROCESSO_CONCLUIDO | Encerramento | SISTEMA | sem pendências críticas abertas | Auditoria: `WORKFLOW_TRANSICAO` |

---

## C) Fluxo F2 — Entrega Futura (mãe)

| From | To | Condição | Responsável | Validações | Efeitos (mínimo) |
|---|---|---|---|---|---|
| OS_CRIADA | OS_ATUALIZADA_DATA_ESTIMADA | Definida/atualizada data estimada | ADM_FILIAL_DESTINO ou GESTOR_CONTRATO | dataEstimadaEntrega válida | Auditoria: `OS_DATA_ESTIMADA_ATUALIZADA`, `WORKFLOW_TRANSICAO` |
| OS_ATUALIZADA_DATA_ESTIMADA | NFE_ENTREGA_FUTURA_EMITIDA | NFe entrega futura emitida | ADM_FILIAL_ORIGEM | chave NFe válida | Auditoria: `NFE_EMITIDA`, `WORKFLOW_TRANSICAO` |
| NFE_VALIDADA_OK | NFE_RECEBIDA_SEM_ESTOQUE | NFe válida, sem movimentar estoque | SISTEMA | regra do fluxo mãe | Auditoria: `WORKFLOW_TRANSICAO` |
| NFE_RECEBIDA_SEM_ESTOQUE | OC_CRIADA_PARA_REMESSA | OC criada para etapa filha | ADM_FILIAL_DESTINO | osId válido; número OC único | Auditoria: `WORKFLOW_TRANSICAO` |
| OC_CRIADA_PARA_REMESSA | AGUARDANDO_REMESSA | Aguardando remessa (filha) | SISTEMA | - | Auditoria: `WORKFLOW_TRANSICAO` |
| AGUARDANDO_REMESSA | ALERTA_7_DIAS_ENTREGA_ESTIMADA | Job diário detecta 7 dias para entrega | SISTEMA | dataEstimadaEntrega definida | Pendência (opcional) `ATRASO_ENTREGA_7_DIAS`; Notificação: `LEMBRETE_7_DIAS_ENTREGA_ESTIMADA`; Auditoria: `NOTIFICACAO_ENFILEIRADA`, `WORKFLOW_TRANSICAO` |
| AGUARDANDO_REMESSA | PROCESSO_CONCLUIDO | Encerramento da fase "mãe" | SISTEMA | sem pendências críticas abertas | Auditoria: `WORKFLOW_TRANSICAO` |

---

## D) Fluxo F3 — Entrega Futura (filha)

| From | To | Condição | Responsável | Validações | Efeitos (mínimo) |
|---|---|---|---|---|---|
| OC_PENDENTE_ENTREGA_FUTURA | APROVACAO_ENTREGA_PENDENTE | Inicia aprovação | ADM_FILIAL_DESTINO | - | Auditoria: `WORKFLOW_TRANSICAO`; Pendência opcional `APROVACAO_PENDENTE` |
| APROVACAO_ENTREGA_PENDENTE | ENTREGA_APROVADA | Aprovado por GESTOR_CONTRATO | GESTOR_CONTRATO | RB-11: sempre aprova, independente de valor e filial | Auditoria: `WORKFLOW_TRANSICAO` |
| APROVACAO_ENTREGA_PENDENTE | ENTREGA_REPROVADA | Reprovado (não aplicável para GESTOR_CONTRATO) | - | RB-11: GESTOR_CONTRATO sempre aprova; reprovação não se aplica | Auditoria: `WORKFLOW_TRANSICAO` (não aplicável) |
| ENTREGA_APROVADA | NFE_REMESSA_EMITIDA | NFe remessa emitida | ADM_FILIAL_ORIGEM | chave NFe válida | Auditoria: `NFE_EMITIDA`, `WORKFLOW_TRANSICAO` |
| NFE_VALIDADA_OK | VINCULADA_OS_OC_NFE | Vínculo OS↔OC↔NFe criado | ADM_FILIAL_ORIGEM ou SISTEMA | idempotência (unique); OC obrigatória | Auditoria: `VINCULO_CRIADO`, `WORKFLOW_TRANSICAO` |
| VINCULADA_OS_OC_NFE | ENTRADA_ORIGEM_CONCLUIDA | Entrada na origem concluída | ADM_FILIAL_ORIGEM | evidência mínima (se houver) | Auditoria: `WORKFLOW_TRANSICAO` |
| ENTRADA_ORIGEM_CONCLUIDA | SAIDA_ORIGEM_CONCLUIDA | Saída da origem concluída | ADM_FILIAL_ORIGEM | - | Auditoria: `WORKFLOW_TRANSICAO` |
| SAIDA_ORIGEM_CONCLUIDA | EM_TRANSITO | Em trânsito | SISTEMA | - | Auditoria: `WORKFLOW_TRANSICAO` |
| EM_TRANSITO | CHEGADA_MATERIAL_DESTINO | Chegada confirmada | ADM_FILIAL_DESTINO | - | Auditoria: `CHEGADA_MATERIAL_DESTINO`, `WORKFLOW_TRANSICAO`; Notificação: `CHEGADA_MATERIAL` |
| CHEGADA_MATERIAL_DESTINO | ENTRADA_DESTINO_PENDENTE_ANEXO | Exige anexo para concluir | SISTEMA | tipo(s) de anexo obrigatório(s) | Auditoria: `WORKFLOW_TRANSICAO` |
| ENTRADA_DESTINO_PENDENTE_ANEXO | ENTRADA_DESTINO_CONCLUIDA | Anexos ok | ADM_FILIAL_DESTINO | anexo(s) obrigatórios presentes | Auditoria: `ENTRADA_DESTINO_CONCLUIDA`, `WORKFLOW_TRANSICAO` |
| ENTRADA_DESTINO_CONCLUIDA | ESTOQUE_DESTINO_ATUALIZADO | Atualiza estoque destino | SISTEMA | RF-05 | Auditoria: `WORKFLOW_TRANSICAO` |
| ESTOQUE_DESTINO_ATUALIZADO | PROCESSO_CONCLUIDO | Encerramento | SISTEMA | sem pendências críticas abertas | Auditoria: `WORKFLOW_TRANSICAO` |
| * | ALERTA_30_DIAS_DESTINO | Job diário detecta 30 dias pendente no destino | SISTEMA | regra de "tempo em estado" | Pendência opcional `ATRASO_DESTINO_30_DIAS`; Notificação `LEMBRETE_30_DIAS_DESTINO`; Auditoria `NOTIFICACAO_*`, `WORKFLOW_TRANSICAO` |

---

## E) Medição (TBD-04 fechado)

A medição é realizada no RM/Contratos com base na OS aberta no nFlow. As transições de medição são atualizadas via integração quando a medição for concluída no RM.

| From | To | Condição | Responsável | Validações | Efeitos |
|---|---|---|---|---|---|
| APROVACAO_MEDICAO_PENDENTE | MEDICAO_APROVADA | Medição aprovada no RM/Contratos | SISTEMA (via integração RM) | resultado da medição no RM | Auditoria: `WORKFLOW_TRANSICAO`, `MEDICAO_APROVADA`; Notificação `MEDICAO_CONCLUIDA` |
| APROVACAO_MEDICAO_PENDENTE | MEDICAO_REPROVADA | Medição reprovada no RM/Contratos | SISTEMA (via integração RM) | resultado da medição no RM | Auditoria: `WORKFLOW_TRANSICAO`, `MEDICAO_REPROVADA`; Pendência `MEDICAO_PENDENTE` |

---

**Referências:**
- Estados canônicos: [`workflow-states.md`](./workflow-states.md)
- Catálogo de eventos: [`docs/contracts/auditoria-eventos.md`](../../contracts/auditoria-eventos.md)
- Templates de e-mail: [`docs/contracts/email-templates.md`](../../contracts/email-templates.md)
- Regras de disparo: [`docs/OPERATIONS.md`](../../OPERATIONS.md) (Seção "2. Notificações por E-mail")
- TBDs relacionados: [`TBD.md`](./TBD.md) (TBD-03 fechado, TBD-04 fechado)

**Desenvolvido por [ness.](https://github.com/resper1965/nTransfer)**
