# Glossário de Termos Técnicos

> Termos do domínio usados na documentação do projeto "Transferência de Materiais Entre Filiais".

## OS (Ordem de Serviço)
Documento interno que autoriza e controla a fabricação de determinado material ou serviço.

## OC (Ordem de Compra)
Documento formal que registra a solicitação de aquisição de materiais ou serviços junto a um fornecedor.

## NFe (Nota Fiscal Eletrônica)
Documento fiscal digital que registra uma operação de circulação de mercadorias ou prestação de serviços.

## DANFE (Documento Auxiliar da Nota Fiscal Eletrônica)
Representação gráfica simplificada da NFe, usada para acompanhar o transporte da mercadoria.

## Romaneio
Documento que lista os itens carregados em um veículo para controle de transporte e conferência no destino.

## Medição
Processo de verificação de quantidade e qualidade dos serviços ou materiais entregues, usado para liberação de pagamento ou aceite contratual.

## Estoque Não Contábil
Registro de movimentações de materiais que ainda não impactam diretamente a contabilidade oficial da empresa (por exemplo, itens em trânsito ou pendentes de conferência).

---

## Nomenclatura Padrão (Obrigatório)

### Papéis (Roles) — Nomes Oficiais

Usar sempre exatamente os nomes abaixo nos documentos e exemplos:

- **ADM_FILIAL_ORIGEM**: Administrativo responsável por ações na filial de origem.
- **ADM_FILIAL_DESTINO**: Administrativo responsável por ações na filial de destino.
- **FISCAL**: Responsável por validar NFe (correta/incorreta) e registrar motivo quando incorreta.
- **FABRICA**: Responsável por marcar fabricação/conclusão de material quando aplicável.
- **GESTOR_CONTRATO**: Responsável por governança e decisões operacionais (ex.: cancelamentos, alertas).
- **SISTEMA**: Ações automatizadas (jobs, integrações, transições técnicas).

> Se houver necessidade de novos papéis, registrar em [`TBD.md`](./specs/transferencia-materiais/TBD.md) e atualizar este glossário.

### Entidades — Nomes Oficiais

- **OS**: Ordem de Serviço.
- **OC**: Ordem de Compra.
- **NFe**: Nota Fiscal eletrônica.
- **Vínculo**: Associação entre OS e NFe (e OC quando aplicável).
- **Pendência**: Item operacional a resolver, com ownerRole.
- **Notificação**: Registro de e-mail (enfileirada/enviada/falhou).
- **AuditoriaEvento**: Registro append-only de eventos.

### Estados do Workflow — Padrão

- O estado é um identificador **UPPER_SNAKE_CASE** e deve existir no enum `WorkflowStatus`.
- Nenhum documento deve inventar estados fora do catálogo.
- **Fonte de verdade:**
  - [`docs/specs/transferencia-materiais/workflow-states.md`](./specs/transferencia-materiais/workflow-states.md)
  - [`docs/data-models/data-model.md`](./data-models/data-model.md)
  - [`docs/contracts/openapi.yaml`](./contracts/openapi.yaml)

### Convenções de Termos

- Preferir "**Adm. Filial Origem/Destino**" em textos corridos e manter o role oficial (ex.: `ADM_FILIAL_DESTINO`) em tabelas, exemplos e artefatos técnicos.
- "Gestor do Contrato" é o termo preferido (evitar variantes).
- "Vínculo" é o termo preferido (evitar "amarrar", "ligar", "relacionar" em especificações técnicas).

### Correlação (Correlation)

- `correlationType` ∈ {`OS`, `OC`, `NFE`, `VINCULO`}
- `correlationId`:
  - OS: `os.id`
  - OC: `oc.id`
  - NFE: `nfe.chaveAcesso`
  - VINCULO: `vinculo.id`

---

**Referências:**
- Estados canônicos: [`workflow-states.md`](./specs/transferencia-materiais/workflow-states.md)
- Transições: [`workflow-transitions.md`](./specs/transferencia-materiais/workflow-transitions.md)
- TBDs: [`TBD.md`](./specs/transferencia-materiais/TBD.md)

**Desenvolvido por [ness.](https://github.com/resper1965/nTransfer)**
