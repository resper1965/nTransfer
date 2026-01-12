# Higienização e Organização do Repositório

**Data:** 2026-01-12  
**Base:** @codebase  
**Objetivo:** Reorganizar o repositório com melhores práticas, reduzindo duplicidade, melhorando navegabilidade e prevenindo regressões.

---

## 1. Resumo Executivo

### Estado Atual
- **Estrutura:** Organizada, mas com oportunidades de melhoria
- **Documentação:** Rica e completa (40+ arquivos .md em `docs/`)
- **Problemas identificados:** 3 arquivos de auditoria (potencial duplicação conceitual), ausência de scripts de validação, links internos não validados automaticamente
- **Risco:** Baixo (estrutura já está boa, melhorias são incrementais)

### Recomendação
- **Ação:** Migração incremental em 5 PRs pequenos
- **Prioridade:** Média (não bloqueia desenvolvimento, mas melhora manutenibilidade)
- **Esforço estimado:** 2-3 horas de trabalho

### Resultado Esperado
- Estrutura de pastas mais clara e navegável
- Validações automáticas (OpenAPI, build, links)
- Documentação consolidada sem duplicação conceitual
- CI mínimo rodando validações

---

## 2. Inventário Atual (com evidências)

### 2.1 Estrutura de Pastas

```
/
├── .github/
│   ├── workflows/
│   │   ├── ci.yml
│   │   ├── codeql.yml
│   │   └── dependency-review.yml
│   ├── ISSUE_TEMPLATE/
│   │   ├── bug_report.md
│   │   └── feature_request.md
│   ├── dependabot.yml
│   └── PULL_REQUEST_TEMPLATE.md
├── docs/
│   ├── specs/
│   │   └── transferencia-materiais/
│   │       ├── CONSTITUTION.md
│   │       ├── PROJECT_MAP.md
│   │       ├── SPEC.md
│   │       ├── PLAN.md
│   │       ├── TASKS.md
│   │       ├── TBD.md
│   │       ├── workflow-states.md
│   │       ├── workflow-transitions.md
│   │       ├── diagrams.md
│   │       └── examples.md
│   ├── contracts/
│   │   ├── openapi.yaml
│   │   ├── auditoria-eventos.md
│   │   ├── email-templates.md
│   │   ├── integracao-qive-rm.md
│   │   └── movimentos-dicionario.md
│   ├── data-models/
│   │   └── data-model.md
│   ├── README.md
│   ├── ARCHITECTURE.md
│   ├── OPERATIONS.md
│   ├── DEVELOPMENT_GUIDE.md
│   ├── DESIGN_GUIDE.md
│   ├── PRD.md
│   ├── glossario.md
│   ├── audits/
│   │   ├── documentacao.md
│   │   ├── final.md
│   │   └── repo-higiene.md
│   └── (outros arquivos)
├── src/
│   ├── TransferenciaMateriais.Api/
│   ├── TransferenciaMateriais.Domain/
│   ├── TransferenciaMateriais.Application/
│   └── TransferenciaMateriais.Infrastructure/
├── tests/
│   ├── TransferenciaMateriais.Domain.Tests/
│   ├── TransferenciaMateriais.Application.Tests/
│   └── TransferenciaMateriais.Api.Tests/
├── infra/
│   ├── docker-compose.yml
│   └── README.md
├── .gitignore
├── .editorconfig
├── Directory.Build.props
├── Makefile
├── package.json
├── README.md
├── LICENSE
├── SECURITY.md
├── CONTRIBUTING.md
└── TransferenciaMateriais.sln
```

### 2.2 Arquivos na Raiz

**Documentação:**
- `README.md` ✅ (principal)
- `LICENSE` ✅
- `SECURITY.md` ✅
- `CONTRIBUTING.md` ✅

**Configuração:**
- `.gitignore` ✅
- `.editorconfig` ✅
- `Directory.Build.props` ✅
- `Makefile` ✅
- `package.json` ✅ (para Spec Kit)
- `TransferenciaMateriais.sln` ✅

**Scripts:**
- `.spec-kit-setup.sh` ✅

### 2.3 Documentação em `docs/`

**Contagem:**
- ~40 arquivos `.md` em `docs/`
- 1 arquivo `.yaml` (OpenAPI) em `docs/contracts/`
- Estrutura organizada por domínio (`specs/`, `contracts/`, `data-models/`)

**Arquivos de auditoria:**
- `docs/audits/documentacao.md` (484 linhas) — Auditoria de lacunas na documentação
- `docs/audits/final.md` (434 linhas) — Auditoria final de prontidão para implementação
- `docs/audits/repo-higiene.md` (este arquivo) — Auditoria de higienização do repo

### 2.4 Scripts e Automação

**Scripts existentes:**
- `Makefile` — Comandos padronizados (up, down, dev, test, lint, build)
- `.spec-kit-setup.sh` — Setup do Spec Kit
- Nenhum script de validação de OpenAPI ou links

**CI/CD:**
- `.github/workflows/ci.yml` — Build e testes
- `.github/workflows/codeql.yml` — Análise de segurança
- `.github/workflows/dependency-review.yml` — Revisão de dependências

### 2.5 Contratos e Especificações

**OpenAPI:**
- `docs/contracts/openapi.yaml` ✅ (980 linhas, OpenAPI 3.0.3)

**Outros contratos:**
- `docs/contracts/auditoria-eventos.md` ✅
- `docs/contracts/email-templates.md` ✅
- `docs/contracts/integracao-qive-rm.md` ✅
- `docs/contracts/movimentos-dicionario.md` ✅

---

## 3. Problemas Encontrados (priorizados)

### 3.1 Críticos (Bloqueadores) — Nenhum

Nenhum problema crítico identificado. A estrutura atual é funcional.

### 3.2 Altos (Melhorias Importantes)

#### P1: Arquivos de Auditoria com Potencial Duplicação Conceitual
- **Problema:** 3 arquivos de auditoria (agora consolidados em `docs/audits/`)
- **Impacto:** Confusão sobre qual auditoria consultar, possível duplicação de conteúdo
- **Solução:** Consolidar em estrutura clara ou mover para subpasta `docs/audits/`
- **Esforço:** Baixo (apenas reorganização)

#### P2: Ausência de Scripts de Validação
- **Problema:** Não há scripts para validar OpenAPI, links internos, ou consistência de docs
- **Impacto:** Regressões podem passar despercebidas
- **Solução:** Criar `scripts/validate.sh` (ou `.ps1` para Windows)
- **Esforço:** Médio (2-3 horas)

### 3.3 Médios (Melhorias Incrementais)

#### P3: Links Internos Não Validados
- **Problema:** Links em docs não são validados automaticamente
- **Impacto:** Links quebrados podem passar despercebidos
- **Solução:** Adicionar validação de links no script de validação
- **Esforço:** Baixo (1 hora)

#### P4: Ausência de Índice Centralizado de Documentação
- **Problema:** `docs/README.md` existe, mas pode ser melhorado com índice mais claro
- **Impacto:** Navegabilidade pode ser melhorada
- **Solução:** Melhorar `docs/README.md` com índice estruturado
- **Esforço:** Baixo (30 minutos)

### 3.4 Baixos (Nice to Have)

#### P5: Convenções de Nomenclatura Não Documentadas
- **Problema:** Não há documento explícito sobre convenções de nomes de arquivos
- **Impacto:** Inconsistências futuras
- **Solução:** Adicionar seção em `CONTRIBUTING.md` ou criar `docs/CONVENTIONS.md`
- **Esforço:** Baixo (30 minutos)

---

## 4. Estrutura Recomendada (Estado Desejado)

### 4.1 Estrutura de Pastas

```
/
├── .github/
│   ├── workflows/
│   │   ├── ci.yml
│   │   ├── codeql.yml
│   │   └── dependency-review.yml
│   ├── ISSUE_TEMPLATE/
│   ├── dependabot.yml
│   └── PULL_REQUEST_TEMPLATE.md
├── docs/
│   ├── specs/
│   │   └── transferencia-materiais/
│   │       └── (mantém estrutura atual)
│   ├── contracts/
│   │   └── (mantém estrutura atual)
│   ├── data-models/
│   │   └── (mantém estrutura atual)
│   ├── audits/                    # ✅ CRIADO: Auditorias consolidadas
│   │   ├── documentacao.md        # ✅ Renomeado de AUDITORIA_DOCUMENTACAO.md
│   │   ├── final.md               # ✅ Renomeado de AUDITORIA_FINAL.md
│   │   └── repo-higiene.md        # ✅ Este arquivo (renomeado)
│   ├── README.md                  # Índice principal melhorado
│   ├── ARCHITECTURE.md
│   ├── OPERATIONS.md
│   ├── DEVELOPMENT_GUIDE.md
│   ├── DESIGN_GUIDE.md
│   ├── PRD.md
│   └── glossario.md
├── scripts/                       # NOVO: Scripts de validação e utilitários
│   ├── validate.sh                # Validação OpenAPI + build + links
│   ├── validate.ps1               # Versão PowerShell
│   └── check-links.sh             # Validação de links em docs
├── src/
│   └── (mantém estrutura atual)
├── tests/
│   └── (mantém estrutura atual)
├── infra/
│   └── (mantém estrutura atual)
├── .gitignore
├── .editorconfig
├── Directory.Build.props
├── Makefile
├── package.json
├── README.md
├── LICENSE
├── SECURITY.md
└── CONTRIBUTING.md
```

### 4.2 Convenções de Nomenclatura

**Arquivos Markdown:**
- **kebab-case** para novos arquivos (ex.: `workflow-states.md`, `email-templates.md`)
- **UPPERCASE** mantido para arquivos Spec Kit (ex.: `SPEC.md`, `PLAN.md`, `TASKS.md`)
- **PascalCase** mantido para arquivos principais (ex.: `README.md`, `ARCHITECTURE.md`)

**Pastas:**
- **kebab-case** para novas pastas (ex.: `data-models/`, `contracts/`)
- **snake_case** evitado

**Scripts:**
- **kebab-case** com extensão apropriada (ex.: `validate.sh`, `check-links.sh`)

### 4.3 Fontes de Verdade (Canônicas)

**Já documentadas em `docs/README.md`:**
- Workflow: `docs/specs/transferencia-materiais/workflow-states.md`
- Modelo de dados: `docs/data-models/data-model.md`
- OpenAPI: `docs/contracts/openapi.yaml`
- Auditoria: `docs/contracts/auditoria-eventos.md`
- Notificações: `docs/contracts/email-templates.md` + `docs/OPERATIONS.md`

**Manter e reforçar:**
- Nenhum documento deve criar estados/enums fora das fontes canônicas
- TBDs centralizados em `docs/specs/transferencia-materiais/TBD.md`

### 4.4 Política de TBDs

**Já implementada:**
- TBDs centralizados em `docs/specs/transferencia-materiais/TBD.md`
- Referências a TBD-XX em outros documentos apontam para `TBD.md`

**Manter:**
- Nenhum "TBD solto" fora de `TBD.md`
- TBDs fechados marcados como "FECHADO" com data e decisão

---

## 5. Plano de Migração (PR1..PR5)

### PR1: Ajustar `.gitignore` e Configurações Base

**Objetivo:** Garantir que arquivos indevidos não sejam versionados

**Mudanças:**
- Verificar `.gitignore` está completo (bin/, obj/, .vs/, logs/, etc.)
- Adicionar `docs/audits/` se necessário (não, pois será criado)
- Verificar `.editorconfig` está adequado

**Validações:**
```bash
# Verificar se há arquivos indevidos versionados
git ls-files | grep -E "(bin/|obj/|\.log$|\.cache$)"
```

**Impacto:** Nenhum (apenas prevenção)

---

### PR2: Consolidar Auditorias em `docs/audits/`

**Objetivo:** Reorganizar arquivos de auditoria em subpasta dedicada

**Mudanças:**
- Criar `docs/audits/`
- Mover e renomear:
  - `docs/AUDITORIA_DOCUMENTACAO.md` → `docs/audits/documentacao.md`
  - `docs/AUDITORIA_FINAL.md` → `docs/audits/final.md`
  - `docs/AUDITORIA_REPO_HIGIENE.md` → `docs/audits/repo-higiene.md`
- Atualizar referências em:
  - `docs/README.md` (se houver links)
  - `README.md` (raiz, se houver links)

**Mapa "de → para" (✅ EXECUTADO):**
```
docs/AUDITORIA_DOCUMENTACAO.md → docs/audits/documentacao.md ✅
docs/AUDITORIA_FINAL.md         → docs/audits/final.md ✅
docs/AUDITORIA_REPO_HIGIENE.md  → docs/audits/repo-higiene.md ✅
```

**Validações:**
```bash
# Verificar se arquivos foram movidos
ls -la docs/audits/

# Verificar se não há referências quebradas
grep -r "AUDITORIA_DOCUMENTACAO\|AUDITORIA_FINAL" docs/ README.md
```

**Impacto:** Baixo (apenas reorganização, links atualizados)

---

### PR3: Criar Scripts de Validação

**Objetivo:** Adicionar scripts para validar OpenAPI, build e links

**Mudanças:**
- Criar `scripts/validate.sh` (Linux/Mac)
- Criar `scripts/validate.ps1` (Windows)
- Criar `scripts/check-links.sh` (validação de links em docs)
- Adicionar target no `Makefile`: `make validate`

**Conteúdo de `scripts/validate.sh`:**
```bash
#!/bin/bash
set -e

echo "🔍 Validando OpenAPI..."
if command -v swagger-cli &> /dev/null; then
  swagger-cli validate docs/contracts/openapi.yaml
elif command -v redocly &> /dev/null; then
  redocly lint docs/contracts/openapi.yaml
else
  echo "⚠️  swagger-cli ou redocly não encontrado. Pulando validação OpenAPI."
fi

echo "🔍 Validando build .NET..."
dotnet build --no-restore

echo "🔍 Validando testes..."
dotnet test --no-build

echo "🔍 Validando links em docs..."
./scripts/check-links.sh

echo "✅ Validações concluídas!"
```

**Conteúdo de `scripts/check-links.sh`:**
```bash
#!/bin/bash
set -e

echo "🔍 Verificando links em docs/..."

broken_links=0

find docs -name "*.md" -type f | while read file; do
  grep -oP '\[.*?\]\([^)]+\)' "$file" | sed 's/.*(\(.*\))/\1/' | while read link; do
    # Remover âncoras (#)
    target=$(echo "$link" | cut -d'#' -f1)
    
    # Ignorar links externos
    if [[ "$target" =~ ^https?:// ]]; then
      continue
    fi
    
    # Resolver caminho relativo
    dir=$(dirname "$file")
    resolved="$dir/$target"
    
    # Verificar se arquivo existe
    if [ ! -f "$resolved" ] && [ ! -f "docs/$target" ] && [ ! -f "./$target" ]; then
      echo "❌ Link quebrado em $file: $link"
      broken_links=$((broken_links + 1))
    fi
  done
done

if [ $broken_links -gt 0 ]; then
  echo "❌ Encontrados $broken_links links quebrados."
  exit 1
else
  echo "✅ Nenhum link quebrado encontrado."
fi
```

**Validações:**
```bash
# Testar scripts
chmod +x scripts/*.sh
./scripts/validate.sh
```

**Impacto:** Nenhum (apenas adição de scripts)

---

### PR4: Melhorar `docs/README.md` com Índice Estruturado

**Objetivo:** Melhorar navegabilidade da documentação

**Mudanças:**
- Revisar `docs/README.md`
- Adicionar seção "Auditorias" apontando para `docs/audits/`
- Garantir que todos os documentos principais estejam listados
- Adicionar seção "Como Contribuir" com link para `CONTRIBUTING.md`

**Validações:**
```bash
# Verificar se todos os arquivos principais estão referenciados
for f in docs/*.md; do
  basename "$f" | grep -q "$(grep -o '[A-Z_]*\.md' docs/README.md)" || echo "⚠️  $f não referenciado"
done
```

**Impacto:** Nenhum (apenas melhoria de navegabilidade)

---

### PR5: Adicionar Validações ao CI

**Objetivo:** Executar validações automaticamente no CI

**Mudanças:**
- Atualizar `.github/workflows/ci.yml` para incluir:
  - Validação OpenAPI (se ferramenta disponível)
  - Validação de links (script `check-links.sh`)
  - Build e testes (já existem)

**Exemplo de atualização em `ci.yml`:**
```yaml
- name: Validar links em docs
  run: |
    chmod +x scripts/check-links.sh
    ./scripts/check-links.sh

- name: Validar OpenAPI (se disponível)
  run: |
    if command -v swagger-cli &> /dev/null; then
      swagger-cli validate docs/contracts/openapi.yaml
    else
      echo "⚠️  swagger-cli não disponível. Pulando validação OpenAPI."
    fi
```

**Validações:**
```bash
# Testar workflow localmente (se possível)
act -j ci
```

**Impacto:** Nenhum (apenas adição de validações)

---

## 6. Scripts/Automação Mínima

### 6.1 Scripts Propostos

**`scripts/validate.sh`** (Linux/Mac)
- Valida OpenAPI (swagger-cli ou redocly)
- Executa `dotnet build` e `dotnet test`
- Valida links em docs

**`scripts/validate.ps1`** (Windows)
- Versão PowerShell do `validate.sh`
- Mesmas funcionalidades

**`scripts/check-links.sh`**
- Valida links internos em arquivos `.md`
- Ignora links externos (https://)
- Reporta links quebrados

### 6.2 Integração com Makefile

Adicionar target:
```makefile
validate: ## Valida OpenAPI, build, testes e links
	@./scripts/validate.sh
```

### 6.3 Dependências Opcionais

**Ferramentas recomendadas (não obrigatórias):**
- `swagger-cli` ou `redocly` para validação OpenAPI
- Instalação: `npm install -g @apidevtools/swagger-cli` ou `npm install -g @redocly/cli`

**Se não disponíveis:**
- Scripts pulam validação OpenAPI com aviso
- Build e testes continuam sendo validados

---

## 7. Checklist Final (Go/No-Go)

### Antes da Migração

- [x] Inventário completo realizado
- [x] Problemas identificados e priorizados
- [x] Plano de migração definido
- [x] Scripts de validação propostos

### Durante a Migração (por PR)

**PR1:**
- [ ] `.gitignore` verificado e completo
- [ ] Nenhum arquivo indevido versionado

**PR2:**
- [ ] `docs/audits/` criado
- [ ] Arquivos de auditoria movidos e renomeados
- [ ] Referências atualizadas
- [ ] Nenhum link quebrado

**PR3:**
- [ ] `scripts/validate.sh` criado e testado
- [ ] `scripts/validate.ps1` criado (opcional)
- [ ] `scripts/check-links.sh` criado e testado
- [ ] `Makefile` atualizado com target `validate`
- [ ] Scripts executáveis (`chmod +x`)

**PR4:**
- [ ] `docs/README.md` melhorado
- [ ] Índice estruturado adicionado
- [ ] Seção "Auditorias" adicionada
- [ ] Todos os documentos principais referenciados

**PR5:**
- [ ] `.github/workflows/ci.yml` atualizado
- [ ] Validação de links adicionada ao CI
- [ ] Validação OpenAPI adicionada ao CI (se disponível)
- [ ] CI passa com sucesso

### Após a Migração

- [ ] OpenAPI válido (sintaxe YAML e referências $ref resolvem)
- [ ] Build .NET passa (`dotnet build`)
- [ ] Testes passam (`dotnet test`)
- [ ] Links internos validados (nenhum quebrado)
- [ ] Estrutura de pastas conforme estado desejado
- [ ] Nenhum arquivo indevido no git (bin/, obj/, logs/)
- [ ] Convenções de nomenclatura seguidas
- [ ] Documentação navegável e indexada

### Validação Final

```bash
# Executar validações completas
make validate

# Verificar estrutura
tree -L 3 docs/ scripts/

# Verificar links
./scripts/check-links.sh

# Verificar build
dotnet build
dotnet test
```

---

## 8. Observações Finais

### Riscos

- **Baixo:** Migração é incremental e reversível
- **Validações:** Scripts de validação previnem regressões
- **CI:** Validações automáticas garantem qualidade contínua

### Próximos Passos (Opcional)

1. **Documentar convenções:** Adicionar seção em `CONTRIBUTING.md` sobre nomenclatura
2. **Adicionar pre-commit hooks:** Validar antes de commit (opcional, não crítico)
3. **Melhorar validação Mermaid:** Adicionar validação de sintaxe Mermaid (se ferramenta leve disponível)

### Manutenção Contínua

- Executar `make validate` antes de PRs importantes
- Revisar `docs/README.md` periodicamente
- Atualizar scripts de validação conforme necessário

---

**Desenvolvido por [ness.](https://github.com/resper1965/nTransfer)**
