# TBD — Transferência de Materiais Entre Filiais

## Objetivo
Centralizar definições pendentes (TBD) para evitar decisões implícitas.

## Pendências (TBD)

### TBD-01 — Mecanismo de integração Qive ↔ RM
- Status: Aguardando definição
- Opções: webhook | polling | fila/eventos | híbrido
- Impacto: arquitetura de integração, latência, segurança, idempotência
- Referências: PLAN (P4), OpenAPI `/integrations/qive/*`

### TBD-02 — Dicionário técnico de movimentos (aguarde definição)
**Status:** AGUARDE DEFINIÇÃO  
**Descrição:** O dicionário técnico de movimentos (RM nFlow / estoque não contábil / eventos equivalentes) será consolidado ao final do projeto, após estabilização dos fluxos e confirmação do mapeamento real de movimentos e seus códigos.  
**Artefato:** [`docs/contracts/movimentos-dicionario.md`](../../contracts/movimentos-dicionario.md) (intencionalmente incompleto por enquanto).  
**Entrega final:** dicionário completo (códigos, descrições, origem/destino, impacto no workflow, mapeamento RM/nFlow/Qive).  
**Impacto:** parametrização e rastreabilidade de movimentos de estoque não contábil.

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

### TBD-05 — Stack técnico (backend/frontend/banco)
- Status: **FECHADO** (2025-01-12)
- Decisão: .NET 8 (backend), PostgreSQL (banco), Mailpit (e-mail local)
- Impacto: padrões de teste, execução local, observabilidade
- Arquivos criados: `TransferenciaMateriais.sln`, `Directory.Build.props`, estrutura Clean Architecture, `Makefile`, `infra/docker-compose.yml`, `docs/DEVELOPMENT_GUIDE.md`

### TBD-06 — Mapeamento técnico de estados/transições no RM nFlow
- Status: Aguardando definição
- Impacto: integração, reconciliação de status, telas e automações

### TBD-07 — "Caminhão no local": como identificar
- Status: Aguardando definição
- Observação: aparece como elemento/legenda do fluxo
- Opções: input manual | evento externo | integração logística

## Decisões fechadas
- DEC-01: Aplicação é rotina adicional (complementar).
- DEC-02: Notificações via e-mail.
