# Guia de Contribuição

Obrigado por considerar contribuir para o projeto **Transferência de Materiais Entre Filiais**!

## Como Contribuir

### Reportando Bugs

Se você encontrou um bug:

1. Verifique se já existe uma issue aberta sobre o problema
2. Se não existir, crie uma nova issue com:
   - Descrição clara do problema
   - Passos para reproduzir
   - Comportamento esperado vs. comportamento atual
   - Ambiente (OS, versão do .NET, etc.)
   - Logs relevantes (se aplicável)

### Sugerindo Melhorias

Para sugerir uma nova funcionalidade ou melhoria:

1. Verifique se já existe uma issue ou discussão sobre o tema
2. Crie uma issue descrevendo:
   - O problema que a funcionalidade resolveria
   - Como você imagina que funcionaria
   - Benefícios e casos de uso

### Contribuindo com Código

1. **Fork o repositório**
2. **Crie uma branch** para sua feature/fix:
   ```bash
   git checkout -b feature/minha-feature
   ```
3. **Siga os padrões do projeto:**
   - Use `.editorconfig` para formatação
   - Siga as convenções de nomenclatura do .NET
   - Adicione testes para novas funcionalidades
   - Atualize a documentação quando necessário
4. **Commit suas mudanças:**
   ```bash
   git commit -m "feat: adiciona nova funcionalidade"
   ```
   Use [Conventional Commits](https://www.conventionalcommits.org/):
   - `feat:` para novas funcionalidades
   - `fix:` para correções de bugs
   - `docs:` para mudanças na documentação
   - `refactor:` para refatorações
   - `test:` para testes
   - `chore:` para tarefas de manutenção
5. **Push para sua branch:**
   ```bash
   git push origin feature/minha-feature
   ```
6. **Abra um Pull Request:**
   - Preencha o template de PR
   - Descreva suas mudanças
   - Referencie issues relacionadas (se houver)
   - Aguarde revisão

## Padrões de Código

### Formatação
- Use `.editorconfig` (configurado automaticamente na maioria dos IDEs)
- Execute `dotnet format` antes de commitar

### Testes
- Adicione testes unitários para novas funcionalidades
- Mantenha cobertura de código acima de 80%
- Execute `dotnet test` antes de commitar

### Documentação
- Atualize a documentação quando necessário
- Adicione comentários XML para APIs públicas
- Atualize `CHANGELOG.md` para mudanças significativas

## Processo de Code Review

1. Todos os PRs precisam de pelo menos uma aprovação
2. O CI deve passar (build, testes, lint)
3. Code coverage não deve diminuir
4. Feedback será fornecido de forma construtiva

## Dúvidas?

Se tiver dúvidas sobre como contribuir, abra uma issue ou entre em contato com a equipe.

---

**Desenvolvido por ness.**
