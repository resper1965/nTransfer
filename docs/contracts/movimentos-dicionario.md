# Dicionário Técnico — Movimentos (TBD-02)

**Status:** AGUARDE DEFINIÇÃO

Este documento é intencionalmente mantido incompleto durante as fases iniciais do projeto, pois depende de:
- definição final de mapeamento de movimentos do RM/nFlow (ou equivalente),
- confirmação dos códigos reais e semântica operacional,
- decisão final sobre integração real vs stub (TBD-01),
- validação com stakeholders do processo.

**Referência canônica:**
- [`docs/specs/transferencia-materiais/TBD.md`](../specs/transferencia-materiais/TBD.md) — TBD-02

## Estrutura Esperada na Entrega Final

Cada item do dicionário deve conter:
- `codigoMovimento` (string)
- `nomeMovimento` (string)
- `descricao` (string)
- `quandoDispara` (estados/transições)
- `correlationType/correlationId` aplicável
- `impacto` (estoque/origem/destino)
- `regras` (RB/RF relacionadas)
- `exemplos` (payloads e auditoria)

---

**Desenvolvido por [ness.](https://github.com/resper1965/nTransfer)**
