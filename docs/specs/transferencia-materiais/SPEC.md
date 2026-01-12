# SPEC — Transferência de Materiais Entre Filiais

> Referências: `PLAN.md`, `TASKS.md`, `TBD.md`, `contracts/data-model.md`, `contracts/openapi.yaml`, `diagrams.md`, `examples.md`.

## Escopo
Rotina adicional (complementar) para orquestrar, registrar, auditar e notificar o processo de transferência entre filiais com base em OS/OC/NFe, conforme os três fluxos do documento.

Fluxos:
- F1: Compra Direta
- F2: Entrega Futura (mãe)
- F3: Entrega Futura (filha)

## Atores (papéis)
- Gestor Aprovador do Contrato
- Adm. Filial Origem
- Adm. Filial Destino
- Fiscal
- Financeiro
- Fábrica / Inspetor
- Fabricante

## Regras de negócio (RB)
RB-01: Cada OS vincula uma ou mais NFe.  
RB-02: A OS contém filial destino 1:1 e quantidade.  
RB-03: Não travar o processo em caso de diferença de quantidade (+/-) no vínculo OS↔NFe.  
RB-04: Ao menos um anexo é obrigatório para o Adm. da Filial de Destino (no recebimento/entrada).  
RB-05: Se "NFe correta?" = NÃO, cancelar etapas anteriores e detalhar motivo: (1) emissão NFe, (2) vínculo do Adm. Filial.  
RB-06: "Problemas na NFe emitida" → reprovar para orientar o fabricante a emitir nova NFe para vínculo.  
RB-07: Estoque não contábil deve ser atualizado conforme o fluxo (entrada origem/destino e baixa na saída quando aplicável).  
RB-08: Entrega futura (mãe): entrada da NFe de entrega futura **sem atualizar estoque**; criar OC para remessa futura.  
RB-09: Filial Destino: aviso em 30 dias para Gestor do Contrato e Adm. Filial Destino (quando aplicável).  
RB-10: Entrega futura (mãe): aviso em "7 dias para entrega estimada" ao Gestor do Contrato.  
RB-11: Gestor do Contrato sempre aprova entrega, independente de valor e filial. (TBD-03 fechado)  

## Requisitos funcionais (RF)
RF-01: Criar OS e registrar dados mínimos (filial destino, quantidade). (RB-01..RB-03)  
RF-02: Registrar vínculo OS↔NFe (e OS↔OC↔NFe no fluxo filha). (RB-01..RB-03)  
RF-03: Registrar validação fiscal "NFe correta?" com trilha e cancelamento de etapas anteriores quando incorreta. (RB-05)  
RF-04: Gerenciar anexos e bloquear conclusão da etapa de recebimento sem evidência mínima. (RB-04)  
RF-05: Registrar atualizações de estoque não contábil e baixa na saída conforme fluxo. (RB-07)  
RF-06: Notificações por e-mail nos pontos do fluxo (chegada material, NFe entrada, NFe saída pronta, cancelamento/pendências, conclusão de medição, alertas 7/30 dias). (RB-09, RB-10)  
RF-07: Painéis operacionais: OC pendente de entrega futura, aprovações pendentes, pendências por erro de vínculo.  
RF-08: Trilha de auditoria por OS/OC/NFe (toda transição e decisão).  
RF-09: Suportar estado/etapa "caminhão no local" como marcador do fluxo (origem TBD-07).  
RF-10: Suportar "aprova entrega?" como regra configurável. (RB-11, TBD-03 fechado)  
RF-11: Suportar "medição" como etapa do fluxo. A medição é realizada no RM/Contratos com base na OS aberta no nFlow; a rotina atualiza estados via integração quando a medição for concluída. (TBD-04 fechado)  

## Requisitos não funcionais (RNF)
RNF-01: Auditoria imutável de decisões e transições com correlação OS/OC/NFe.
RNF-02: Idempotência nas integrações (por exemplo, chave de acesso NFe como chave idempotente).
RNF-03: RBAC mínimo por papel.
RNF-04: Observabilidade: logs estruturados + correlation-id por processo.
RNF-05: Resiliência: falhas externas geram pendência + retentativa.

## Critérios de aceite (CA)
CA-01 (Vínculo com divergência):
- Dado OS com filial destino e quantidade,
- Quando vincular NFe à OS,
- Então registrar vínculo e não travar por divergência (+/-), registrando a divergência.

CA-02 (NFe incorreta):
- Dado NFe obtida (XML),
- Quando Fiscal marcar como incorreta,
- Então cancelar etapas anteriores e exigir motivo (emissão/vínculo).

CA-03 (Anexo obrigatório):
- Dado recebimento na filial destino,
- Quando tentar concluir entrada,
- Então bloquear sem ao menos um anexo.

CA-04 (Entrega futura mãe sem estoque):
- Dado NFe de entrega futura,
- Quando ativar entrada,
- Então registrar sem atualizar estoque e criar OC para futura remessa.

## Fora de escopo
- Regras fiscais detalhadas de CFOP/tributação.
- Conciliação financeira completa (apenas integração/acionamento do fluxo padrão de pagamentos).

## Dependências e TBD
- Integração Qive ↔ RM: TBD-01
- Estados do RM nFlow: TBD-06
- Caminhão no local: TBD-07
- Stack técnico: TBD-05
