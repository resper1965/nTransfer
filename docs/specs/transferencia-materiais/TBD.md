# TBD — Transferência de Materiais Entre Filiais

## Objetivo
Centralizar definições pendentes (TBD) para evitar decisões implícitas.

## Pendências (TBD)

### TBD-01 — Mecanismo de integração Qive ↔ RM
- Status: **FECHADO** (2026-01-12)
- Decisão: **Stub mantido para MVP. Integração real será implementada em fase posterior com contrato definido conforme `contracts/integracao-qive-rm.md`.**
- Impacto: RF-03, RF-05, RNF-02
- Observação: Stub funcional implementado com modos ALWAYS_OK, FAIL_RATE, SCENARIO. Integração real será TBD futuro.

### TBD-02 — Dicionário técnico de movimentos
- Status: **FECHADO** (2026-01-12)
- Decisão: **Dicionário técnico de movimentos será definido quando a integração real RM/nFlow for implementada (pós-MVP). Para o MVP, o sistema utiliza eventos de auditoria e estados de workflow como abstração dos movimentos reais.**
- Impacto: parametrização e rastreabilidade de movimentos de estoque não contábil
- Artefato: [`docs/contracts/movimentos-dicionario.md`](../../contracts/movimentos-dicionario.md) (mapeamento MVP documentado)

### TBD-03 — Política "Aprova entrega?"
- Status: **FECHADO** (2026-01-12)
- Decisão: **Gestor do Contrato sempre aprova, independente de valor e filial.**
- Impacto: RBAC + transições (Compra Direta e Filha)
- Regra de Negócio: [RB-11](../SPEC.md#regras-de-negócio-rb)
- Observação: Aprovação é automática para o papel GESTOR_CONTRATO, sem necessidade de validação adicional.

### TBD-04 — Medição: onde ocorre e como registrar
- Status: **FECHADO** (2026-01-12)
- Decisão: **A medição é feita dentro do RM/Contratos com base na Ordem de Serviço aberta no nFlow.**
- Impacto: estados, evidências e auditoria
- Observação: A rotina de transferência não gerencia a medição diretamente; ela ocorre no RM/Contratos. Os estados de medição (`APROVACAO_MEDICAO_PENDENTE`, `MEDICAO_APROVADA`, `MEDICAO_REPROVADA`) podem ser atualizados via integração quando a medição for concluída no RM.
- Implementação: `ProcessarMedicaoConcluidaUseCase` criado.

### TBD-05 — Stack técnico (backend/frontend/banco)
- Status: **FECHADO** (2025-01-12)
- Decisão: .NET 8 (backend), PostgreSQL (banco), Mailpit (e-mail local)
- Impacto: padrões de teste, execução local, observabilidade
- Arquivos criados: `TransferenciaMateriais.sln`, `Directory.Build.props`, estrutura Clean Architecture, `Makefile`, `infra/docker-compose.yml`, `docs/DEVELOPMENT_GUIDE.md`

### TBD-06 — Mapeamento técnico de estados/transições no RM nFlow
- Status: Aguardando definição
- Impacto: integração, reconciliação de status, telas e automações

### TBD-07 — "Caminhão no local": como identificar
- Status: **FECHADO** (2026-01-12)
- Decisão: **Registro manual via API. Evento de auditoria `CAMINHAO_NO_LOCAL` registrado. Se no destino e estado `EM_TRANSITO`, transição automática para `CHEGADA_MATERIAL_DESTINO`.**
- Impacto: RF-09, estados do workflow
- Implementação: `RegistrarCaminhaoNoLocalUseCase` criado.

## Decisões fechadas
- DEC-01: Aplicação é rotina adicional (complementar).
- DEC-02: Notificações via e-mail.
