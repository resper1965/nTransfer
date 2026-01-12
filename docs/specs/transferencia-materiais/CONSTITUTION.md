# speckit.constitution — Princípios do Projeto

> Desenvolvido por **ness.**

## Propósito
Estabelecer princípios e diretrizes para especificação, implementação e evolução da rotina "Transferência de Materiais Entre Filiais".

## Princípios (obrigatórios)
1. **Sistema complementar**
   - Esta aplicação é uma rotina adicional; não substitui RM / RM nFlow / Qive.
   - O sistema deve operar como orquestrador, registrador e auditor do processo.

2. **Fonte de verdade e não-alucinação**
   - Decisões e regras devem estar em SPEC/PLAN/TASKS.
   - Qualquer ponto não especificado deve ser marcado como **TBD** (ver `docs/specs/transferencia-materiais/TBD.md`).

3. **Auditabilidade por padrão**
   - Toda transição de estado, aprovação, reprovação e validação fiscal deve gerar evento de auditoria com:
     - quem, quando, o quê, motivo e correlação (OS/OC/NFe).

4. **Idempotência e resiliência**
   - Entradas duplicadas de integrações (por exemplo, mesma chave de NFe) não podem duplicar efeitos.
   - Falhas de integração não podem "matar" o processo: devem abrir pendência e permitir retentativa.

5. **RBAC mínimo e segregação**
   - Ações sensíveis devem ser restritas por papéis (ex.: Fiscal valida NFe, Adm Filial Destino anexa evidências).

6. **Evidência obrigatória quando o fluxo exige**
   - Onde o fluxo exigir evidência/anexo, o sistema deve bloquear conclusão da etapa até atender a regra.

7. **Evolução por contrato**
   - APIs, eventos e modelos devem evoluir versionados com backward-compatibility sempre que possível.

## Convenções de documentos
- SPEC: RB-* (regras) e RF-* (funcionais), RNF-* (não funcionais), CA-* (aceite).
- PLAN: passos numerados P1..Pn, cada passo referencia RF/RB.
- TASKS: tarefas T01..Txx referenciam PLAN e RF/RB.
- TBD: tudo que não estiver decidido deve estar aqui, sem duplicação em outros docs.
