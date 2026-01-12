# Política de Segurança

## Versão Suportada

Atualmente, apenas a versão mais recente do projeto recebe atualizações de segurança.

| Versão | Suportada          |
| ------- | ------------------ |
| 0.2.x   | :white_check_mark: |
| < 0.2   | :x:                |

## Reportando Vulnerabilidades

Se você descobrir uma vulnerabilidade de segurança, por favor **não** abra uma issue pública.

Em vez disso, envie um e-mail para a equipe de segurança da **ness.** com os seguintes detalhes:

- Descrição da vulnerabilidade
- Passos para reproduzir
- Impacto potencial
- Sugestões de correção (se houver)

**Tempo de resposta esperado:** 48 horas úteis

## Processo de Divulgação

1. A vulnerabilidade será analisada pela equipe de segurança
2. Uma correção será desenvolvida e testada
3. A correção será lançada em uma versão patch
4. A vulnerabilidade será divulgada publicamente após a correção estar disponível

## Boas Práticas de Segurança

### Para Desenvolvedores

- Nunca commitar credenciais ou secrets no repositório
- Usar variáveis de ambiente para configurações sensíveis
- Validar e sanitizar todas as entradas do usuário
- Seguir princípios de menor privilégio
- Manter dependências atualizadas

### Para Usuários

- Usar versões suportadas do software
- Manter o sistema operacional e dependências atualizadas
- Não expor credenciais em logs ou mensagens de erro
- Seguir as recomendações de configuração de segurança

## Dependências

Este projeto utiliza Dependabot para monitorar e atualizar automaticamente dependências. Todas as atualizações de segurança são priorizadas.

## Histórico de Segurança

Vulnerabilidades corrigidas serão documentadas nas release notes e no CHANGELOG.md.

---

**Desenvolvido por ness.**
