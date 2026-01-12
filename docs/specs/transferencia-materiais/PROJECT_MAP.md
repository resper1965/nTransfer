# PROJECT_MAP — Transferência de Materiais Entre Filiais

## Visão geral
Aplicação/workflow para suportar o processo de transferência de materiais entre filiais com base em NFe, Ordem de Serviço de Fabricação (OS) e Ordem de Compra (OC), incluindo integrações Qive ↔ RM e rotinas de aprovação (medição, entrega, vínculo) para três variações de fluxo: Compra Direta; Simples Faturamento para Entrega Futura (mãe); e Entrega Futura (filha). [Fonte: fluxos do PDF]

## Objetivos
- Orquestrar etapas e aprovações do processo ponta a ponta (OS/OC/NFe).
- Garantir vínculo correto entre OS, OC e NFe (com tratamento de "erro de vínculo").
- Suportar cancelamento/reemissão de NFe quando houver problemas de emissão ou vínculo.
- Disparar avisos/notificações para os papéis envolvidos em pontos definidos do fluxo.

## Não-objetivos (por ora)
- Substituir os ERPs (RM) e ferramentas fiscais (Qive). O sistema orquestra e integra.
- Implementar contabilidade completa (somente refletir as ações de estoque "não contábil" conforme fluxo).

## Atores e sistemas
- Gestor Aprovador do Contrato
- Administrativo Filial Origem
- Administrativo Filial Destino
- Fiscal
- Financeiro
- Fábrica / Inspetor Alupar
- Fabricante
- Sistemas: RM, RM nFlow, Qive (integrado com RM)

## Fluxos críticos suportados
1) Compra Direta
2) Simples Faturamento para Entrega Futura (mãe)
3) Simples Faturamento para Entrega Futura (filha / remessa e entrega efetiva)

## Objetos principais (termos do domínio)
- OS (Ordem de Serviço de Fabricação)
- OC (Ordem de Compra) — incluindo movimento específico para Entrega Futura
- NFe: venda, entrada, saída (baixa filial origem), entrega futura, remessa
- DANFE, XML, Romaneio, Medição
- Estoque "Não Contábil" (entrada origem/destino; baixa origem na saída)

## Regras essenciais (extraídas do fluxo)
- Cada OS vincula uma ou mais NFe.
- OS contém Filial Destino 1:1 e Quantidade.
- Não travar o processo em caso de diferença de quantidade (+/-) no vínculo NFe↔OS.
- Ao menos um anexo é obrigatório para o Administrativo da Filial de Destino no recebimento/entrada.

## Observabilidade mínima
- Registrar trilhas de auditoria por etapa: quem aprovou/reprovou, timestamps, motivo de reprova/cancelamento.
- Registrar eventos de integração Qive↔RM (sucesso/erro, payload mínimo, correlação com OS/OC/NFe).

## Execução local / validação
Ver [TBD-05](../transferencia-materiais/TBD.md#tbd-05--stack-técnica) para definição da stack técnica e comandos padrão.
