# Checklist de Validação — Documentação

> **Checklist de Validação** — Comandos e verificações para validar a documentação completa.
> Use este checklist antes de commit/PR.

## 1. Validação de Links

### 1.1 Links Internos

```bash
# Verificar links quebrados (requer ferramenta externa ou verificação manual)
# Todos os links em docs/ devem apontar para arquivos existentes
```

**Arquivos a verificar:**
- [ ] `docs/README.md` — todos os links funcionam
- [ ] `docs/PRD.md` — todas as referências a SPEC/PLAN/TASKS/TBD válidas
- [ ] `docs/ARCHITECTURE.md` — referências a SPEC/diagrams/examples válidas
- [ ] `docs/OPERATIONS.md` — referências a SPEC/openapi válidas
- [ ] `docs/RASTREABILIDADE.md` — referências a SPEC/diagrams/openapi válidas
- [ ] `docs/specs/transferencia-materiais/SPEC.md` — referências cruzadas válidas
- [ ] `docs/specs/transferencia-materiais/PLAN.md` — referências a SPEC válidas
- [ ] `docs/specs/transferencia-materiais/TASKS.md` — referências a PLAN/SPEC válidas

### 1.2 Referências a IDs

**Verificar que todos os IDs referenciados existem:**
- [ ] RB-01 a RB-10 existem em SPEC.md
- [ ] RF-01 a RF-11 existem em SPEC.md
- [ ] RNF-01 a RNF-05 existem em SPEC.md
- [ ] CA-01 a CA-04 existem em SPEC.md
- [ ] TBD-01 a TBD-07 existem em TBD.md
- [ ] T-01 a T-12 existem em TASKS.md
- [ ] F1, F2, F3 existem em diagrams.md

## 2. Validação de OpenAPI

### 2.1 Sintaxe YAML

```bash
# Validar sintaxe YAML (se tiver yamllint)
yamllint docs/contracts/openapi.yaml

# Ou usar validador online
# https://editor.swagger.io/
```

### 2.2 Schemas e Referências

```bash
# Validar schemas e referências $ref
# Usar Swagger Editor ou swagger-cli
npx @apidevtools/swagger-cli validate docs/contracts/openapi.yaml
```

**Verificações:**
- [ ] Todos os `$ref` apontam para schemas existentes
- [ ] Todos os schemas referenciados estão definidos em `components/schemas`
- [ ] Todos os endpoints têm requestBody/parameters definidos quando necessário
- [ ] Todos os responses têm schema definido
- [ ] Exemplos estão presentes nos endpoints principais

## 3. Validação de Diagramas Mermaid

### 3.1 Sintaxe Mermaid

**Verificar em:** [diagrams.md](./specs/transferencia-materiais/diagrams.md)

- [ ] F1 — Compra Direta (alto nível) — sintaxe válida
- [ ] F1 — Compra Direta (estados) — sintaxe válida
- [ ] F2 — Entrega Futura (mãe) — sintaxe válida
- [ ] F2 — Entrega Futura (mãe) estados — sintaxe válida
- [ ] F3 — Entrega Futura (filha) — sintaxe válida
- [ ] F3 — Entrega Futura (filha) estados — sintaxe válida
- [ ] Modelo de dados (ERD) — sintaxe válida

**Ferramenta:** Renderizar em https://mermaid.live/ ou VS Code com extensão Mermaid

## 4. Validação de Consistência

### 4.1 Nomenclatura

**Verificar consistência de nomes:**
- [ ] "OS" usado consistentemente (não "Ordem de Serviço" em alguns lugares)
- [ ] "OC" usado consistentemente
- [ ] "NFe" usado consistentemente
- [ ] Estados do workflow consistentes entre diagrams.md e SPEC.md
- [ ] Nomes de papéis consistentes (ex.: "Adm. Filial Origem" vs "Administrativo Filial Origem")

### 4.2 Referências Cruzadas

**Verificar que referências fazem sentido:**
- [ ] RF-XX referenciados em PLAN.md existem em SPEC.md
- [ ] RB-XX referenciados em TASKS.md existem em SPEC.md
- [ ] TBD-XX referenciados em SPEC/PLAN/TASKS existem em TBD.md
- [ ] Endpoints referenciados em PRD/ARCHITECTURE existem em openapi.yaml

## 5. Validação de Build/Test

### 5.1 Build do Projeto

```bash
# Compilar solução .NET
make build
# ou
dotnet build
```

- [ ] Solução compila sem erros
- [ ] Todos os projetos referenciados existem
- [ ] Dependências resolvidas corretamente

### 5.2 Testes

```bash
# Rodar testes
make test
# ou
dotnet test
```

- [ ] Testes passam (mesmo que sejam placeholders)
- [ ] Estrutura de testes está correta

### 5.3 Lint/Format

```bash
# Verificar formatação
make lint
# ou
dotnet format --verify-no-changes
```

- [ ] Código formatado corretamente
- [ ] Sem warnings críticos

## 6. Validação de Conteúdo

### 6.1 Completude

**Verificar que todos os documentos obrigatórios existem:**
- [ ] PRD.md — completo
- [ ] ARCHITECTURE.md — completo
- [ ] OPERATIONS.md — completo
- [ ] DEVELOPMENT_GUIDE.md — completo
- [ ] RASTREABILIDADE.md — completo
- [ ] SPEC.md — todos os RF/RB/RNF/CA presentes
- [ ] diagrams.md — F1, F2, F3 completos
- [ ] examples.md — exemplos práticos presentes
- [ ] openapi.yaml — schemas completos

### 6.2 Sem Dúvidas Soltas

**Verificar que não há dúvidas fora do TBD.md:**
- [ ] Nenhum documento contém "[DÚVIDA]" ou "?" sem referência a TBD-XX
- [ ] Todas as pendências estão em TBD.md com ID

### 6.3 Exemplos Práticos

**Verificar exemplos:**
- [ ] Exemplos de API (request/response) presentes
- [ ] Exemplos de eventos de auditoria presentes
- [ ] Exemplos de estados do workflow presentes
- [ ] Exemplos alinhados com openapi.yaml

## 7. Validação de Estrutura

### 7.1 Estrutura de Diretórios

```bash
# Verificar estrutura
tree docs -L 3
```

- [ ] Estrutura de diretórios organizada
- [ ] Nenhum arquivo órfão
- [ ] README.md na raiz de docs/ com índice completo

### 7.2 Arquivos .NET

```bash
# Verificar estrutura .NET
find src tests -name "*.csproj" -o -name "*.cs" | head -20
```

- [ ] Estrutura Clean Architecture presente
- [ ] Todos os projetos referenciados na solution existem
- [ ] Directory.Build.props presente

## 8. Checklist Rápido (Pre-Commit)

**Antes de cada commit/PR, verificar:**

- [ ] `make build` passa
- [ ] `make test` passa (ou pelo menos compila)
- [ ] `make lint` passa
- [ ] Links em README.md funcionam
- [ ] OpenAPI válido (sintaxe YAML)
- [ ] Diagramas Mermaid renderizam corretamente
- [ ] Nenhuma referência a ID inexistente (RB-XX, RF-XX, etc.)
- [ ] TBD.md contém todas as pendências (sem duplicação)

## 9. Validação de Rastreabilidade

**Verificar matriz de rastreabilidade:**
- [ ] Todos os RF mapeados para fluxos/estados/endpoints
- [ ] Todos os RB mapeados para RF/estados
- [ ] Todos os fluxos (F1/F2/F3) têm estados mapeados
- [ ] Todos os endpoints têm RF/RB associados
- [ ] Todos os estados têm eventos de auditoria definidos

Ver [RASTREABILIDADE.md](./RASTREABILIDADE.md) para matriz completa.

## 10. Comandos de Validação Completa

```bash
# 1. Build
make build

# 2. Testes
make test

# 3. Lint
make lint

# 4. Verificar estrutura
find docs -type f -name "*.md" | wc -l  # Deve retornar número esperado

# 5. Verificar OpenAPI (se tiver swagger-cli)
npx @apidevtools/swagger-cli validate docs/contracts/openapi.yaml

# 6. Verificar links (manual ou ferramenta externa)
# Todos os links em docs/ devem funcionar
```

## 11. Validação Manual (Revisão)

**Revisar manualmente:**
- [ ] Documentação é "application-first" (não fala sobre Spec Kit)
- [ ] Todos os termos técnicos estão no glossário
- [ ] Exemplos são práticos e úteis
- [ ] Diagramas são claros e completos
- [ ] Matriz de rastreabilidade está completa
- [ ] Nenhuma informação contraditória entre documentos
