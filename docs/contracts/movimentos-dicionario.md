# Dicionário Técnico — Movimentos (TBD-02)

**Status:** FECHADO (2026-01-12)

**Decisão:** Dicionário técnico de movimentos será definido quando a integração real RM/nFlow for implementada (pós-MVP). Para o MVP, o sistema utiliza eventos de auditoria e estados de workflow como abstração dos movimentos reais.

**Referência canônica:**
- `docs/TBD.md` — TBD-02 (fechado)
- `docs/contracts/auditoria-eventos.md` — Eventos de auditoria (abstração de movimentos)

## Mapeamento MVP (Eventos de Auditoria como Abstração)

Durante o MVP, os "movimentos" são representados por eventos de auditoria:

| Evento de Auditoria | Equivalente a Movimento | Quando Dispara |
|---------------------|------------------------|----------------|
| `OS_CRIADA` | Criação de OS no RM | Criação de OS |
| `MATERIAL_FABRICADO` | Movimento de fabricação | Fabricação concluída |
| `NFE_EMITIDA` | Emissão de NFe | NFe emitida |
| `NFE_XML_OBTIDO` | Recebimento de XML | XML obtido via Qive |
| `VINCULO_CRIADO` | Vínculo OS/OC/NFe | Vínculo registrado |
| `ENTRADA_ORIGEM_CONCLUIDA` | Entrada na origem | Entrada concluída |
| `SAIDA_ORIGEM_CONCLUIDA` | Saída da origem | Saída concluída |
| `ENTRADA_DESTINO_CONCLUIDA` | Entrada no destino | Entrada no destino concluída |
| `ESTOQUE_ORIGEM_ATUALIZADO` | Atualização estoque origem | Estoque origem atualizado |
| `ESTOQUE_DESTINO_ATUALIZADO` | Atualização estoque destino | Estoque destino atualizado |

## Estrutura Final (Pós-MVP)

Quando a integração real for implementada, cada item do dicionário deve conter:
- `codigoMovimento` (string) — Código do movimento no RM/nFlow
- `nomeMovimento` (string) — Nome descritivo
- `descricao` (string) — Descrição detalhada
- `quandoDispara` (estados/transições) — Quando o movimento é disparado
- `correlationType/correlationId` aplicável — Correlação com entidades
- `impacto` (estoque/origem/destino) — Impacto no estoque
- `regras` (RB/RF relacionadas) — Regras de negócio aplicáveis
- `exemplos` (payloads e auditoria) — Exemplos práticos

---

**Desenvolvido por [ness.](https://github.com/resper1965/nTransfer)**
