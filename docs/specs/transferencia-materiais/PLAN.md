# PLAN — Transferência de Materiais Entre Filiais

> Este plano implementa RF-* e RB-* definidos em `SPEC.md`.

## Passos (P)
P1) Domínio e persistência mínima (RF-01, RF-02, RF-08)
- Modelar OS, OC, NFe, Vínculo, Aprovação, Pendência, Anexo, Notificação, Auditoria.
- Referência: `contracts/data-model.md`

P2) Máquina de estados / workflow (RF-02, RF-03, RF-05, RF-09..RF-11)
- Implementar estados para F1/F2/F3 (ver `diagrams.md`)
- Regras: RB-01..RB-10

P3) Auditoria e logs (RF-08, RNF-01, RNF-04)
- Evento de auditoria em toda transição/decisão.
- Correlation-id por OS/OC/NFe.

P4) Integração Qive ↔ RM (stub + adaptador) (RF-03, RNF-02, RNF-05)
- Criar interface `IntegrationAdapter` e "stub" inicial.
- Mecanismo definitivo: TBD-01

P5) API interna e painéis (RF-04, RF-06, RF-07)
- Implementar endpoints definidos em `contracts/openapi.yaml`.
- Painéis: OC pendente, pendências de vínculo, aprovações pendentes.
- **Mock/Protótipo de Frontend para Aprovação** (T13): validar UX antes da implementação completa.

P6) Notificações e-mail (RF-06)
- Templates simples + auditoria do envio.

P7) Fechamento dos TBD críticos
- TBD-03 (aprova entrega), TBD-04 (medição), TBD-06 (nFlow), TBD-07 (caminhão no local), TBD-05 (stack).

## Riscos
- Ambiguidade de mapeamento nFlow (TBD-06) pode exigir ajustes em integrações e estados.
