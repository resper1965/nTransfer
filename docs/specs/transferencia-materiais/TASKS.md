# TASKS — Transferência de Materiais Entre Filiais

## T01 — Baseline de documentação (PLAN P1)
- [ ] Garantir links entre PROJECT_MAP, SPEC, PLAN, TASKS, TBD, diagrams, examples.
- DoD: navegação completa sem órfãos.

## T02 — Modelo de dados mínimo (PLAN P1, RF-01/02/08)
- [ ] Implementar entidades do `data-model.md`.
- DoD: migrations/schema versionados.

## T03 — State machine do workflow (PLAN P2, RF-02/03/05/09..11)
- [ ] Implementar F1/F2/F3 conforme `diagrams.md`.
- [ ] Validar RB-01..RB-10.
- DoD: testes unitários por transição.

## T04 — API interna (PLAN P5, RF-04/07/08)
- [ ] Implementar endpoints conforme `openapi.yaml`.
- DoD: OpenAPI reflete implementação + exemplos válidos.

## T05 — Integração (stub) Qive↔RM + idempotência (PLAN P4, RNF-02)
- [ ] Implementar `IntegrationAdapter` + recepção de NFe (XML/metadados).
- DoD: mesma chave NFe não duplica efeitos.

## T06 — Notificações e-mail (PLAN P6, RF-06)
- [ ] Disparar e registrar auditoria de e-mails (chegada, entrada, saída pronta, cancelamento, medição, 7/30 dias).
- DoD: templates + log do envio.

## T07 — Painéis mínimos (PLAN P5, RF-07)
- [ ] OC pendente entrega futura (mãe/filha)
- [ ] Pendências por erro de vínculo
- DoD: filtros básicos por filial/estado.

## T08 — Auditoria completa (PLAN P3, RNF-01)
- [ ] Eventos por transição e por decisão ("NFe correta?", reprova, cancelamento).
- DoD: trilha por OS/OC/NFe.

## T09 — Definir integração Qive↔RM (TBD-01)
- [ ] Decidir mecanismo e atualizar PLAN/SPEC/OpenAPI.
- DoD: decisão registrada no TBD como "fechado".

## T10 — Fechar dicionário técnico de movimentos (TBD-02)
- [ ] Preencher `movimentos-dicionario.md` ao final.
- DoD: mapeamento completo e revisado.

## T11 — Definir e implementar medição (TBD-04)
- [ ] Decidir RM vs rotina e implementar.
- DoD: etapa auditável (aprova/reprova se aplicável).

## T12 — Definir "caminhão no local" (TBD-07)
- [ ] Definir fonte (manual/integrado) e refletir no workflow.
- DoD: estado/evento registrado e auditado.
