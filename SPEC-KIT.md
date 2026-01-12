# GitHub Spec Kit - Guia de Uso

O **GitHub Spec Kit** é uma ferramenta de auxílio ao desenvolvimento que facilita o **Desenvolvimento Orientado a Especificações (Spec-Driven Development)**. Esta ferramenta **não faz parte da aplicação final**, sendo utilizada apenas durante o processo de desenvolvimento.

## Instalação

O spec-kit já está instalado no sistema via `uv`. Para verificar a instalação:

```bash
specify check
```

### Instalação Manual (se necessário)

Se precisar instalar ou atualizar o spec-kit:

```bash
# Instalação persistente (recomendado)
uv tool install specify-cli --from git+https://github.com/github/spec-kit.git

# Atualização
uv tool install specify-cli --force --from git+https://github.com/github/spec-kit.git

# Uso único (sem instalar)
uvx --from git+https://github.com/github/spec-kit.git specify init <PROJECT_NAME>
```

## Requisitos

- Python 3.8+
- `uv` (gerenciador de pacotes Python) - [Instalação do uv](https://github.com/astral-sh/uv)
- Node.js >= 22.0.0 (para a aplicação)
- npm >= 10.0.0 (para a aplicação)

## Comandos Disponíveis

### Via npm scripts (recomendado)

```bash
# Verificar instalação
npm run specify:check

# Inicializar o projeto com spec-kit
npm run specify:init

# Ver ajuda
npm run specify:help
```

### Via CLI direto

```bash
# Verificar instalação e configuração
specify check

# Inicializar novo projeto
specify init <nome-do-projeto>

# Inicializar no diretório atual
specify init . --ai claude
# ou
specify init --here --ai claude

# Ver ajuda
specify --help
```

## Fluxo de Trabalho com Spec Kit

### 1. Constituição do Projeto (`/speckit.constitution`)

Define os princípios e diretrizes de desenvolvimento do projeto. Use este comando para estabelecer:
- Princípios de design
- Padrões de código
- Diretrizes de arquitetura
- Convenções de nomenclatura

### 2. Especificação (`/speckit.specify`)

Define os requisitos e histórias de usuário. Use para:
- Descrever funcionalidades
- Definir casos de uso
- Estabelecer critérios de aceitação
- Documentar requisitos não-funcionais

### 3. Planejamento Técnico (`/speckit.plan`)

Cria planos de implementação técnica alinhados com a pilha tecnológica. Use para:
- Definir arquitetura técnica
- Escolher tecnologias e bibliotecas
- Planejar estrutura de pastas
- Estabelecer padrões de implementação

### 4. Geração de Tarefas (`/speckit.tasks`)

Gera listas de tarefas acionáveis para implementação. Use para:
- Quebrar funcionalidades em tarefas menores
- Priorizar implementação
- Criar checklist de desenvolvimento

### 5. Implementação (`/speckit.implement`)

Executa as tarefas para construir a funcionalidade conforme o plano. Use para:
- Gerar código baseado nas especificações
- Implementar funcionalidades seguindo o plano
- Validar implementação contra especificações

## Integração com Assistentes de IA

O Spec Kit é compatível com diversos assistentes de IA. Para verificar quais estão disponíveis:

```bash
specify check
```

### Assistentes Suportados

**IDE-based** (não requerem CLI separado):
- GitHub Copilot
- Cursor
- Windsurf
- Kilo Code
- Roo Code
- IBM Bob

**CLI-based** (requerem instalação separada):
- Claude Code
- Gemini CLI
- Qwen Code
- opencode
- Codex CLI
- Auggie CLI
- CodeBuddy
- Qoder CLI
- Amazon Q Developer CLI
- Amp
- SHAI

### Como Usar

1. **Inicialize o projeto com o assistente desejado:**
   ```bash
   specify init . --ai claude
   # ou
   specify init . --ai copilot
   ```

2. **Use os comandos `/speckit.*` no chat do assistente:**
   - Os comandos estarão disponíveis automaticamente após a inicialização
   - Certifique-se de estar no diretório do projeto ao usar o assistente

## Estrutura de Arquivos

O spec-kit pode gerar arquivos de especificação. Recomenda-se manter uma estrutura organizada:

```
projeto/
├── specs/              # Especificações do projeto
│   ├── constitution.md # Princípios e diretrizes
│   ├── requirements/   # Requisitos e histórias
│   └── plans/          # Planos técnicos
└── tasks/              # Tarefas geradas
```

## Notas Importantes

- O spec-kit é uma **ferramenta de desenvolvimento**, não parte da aplicação final
- Os arquivos gerados pelo spec-kit devem ser versionados (exceto caches temporários)
- Use o spec-kit para manter o código alinhado com as especificações
- As especificações devem ser atualizadas conforme o projeto evolui

## Recursos Adicionais

- [Repositório oficial](https://github.com/github/spec-kit)
- [Documentação do GitHub](https://docs.github.com)
- [Microsoft Learn - Spec-Driven Development](https://learn.microsoft.com/pt-br/training/modules/spec-driven-development-github-spec-kit-enterprise-developers/)
