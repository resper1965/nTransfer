# Diagramas — Transferência de Materiais Entre Filiais

> Diagramas em Mermaid. Referência aos fluxos do PDF.
> **Total:** 7 diagramas (3 fluxos alto nível, 3 máquinas de estado, 1 modelo de dados)
> 
> **IMPORTANTE:** Todos os estados usados aqui devem estar no catálogo canônico definido em [workflow-states.md](./workflow-states.md) e [data-model.md](../../data-models/data-model.md#22-workflowstatus-canônico).

## F1 — Compra Direta (alto nível)

```mermaid
flowchart LR
  A[Emitir OS] --> B[Fabricar Material]
  B --> C{Aprova Medição?}
  C -- Não --> X1[Reprovar com motivo] --> B
  C -- Sim --> D[Conferir Romaneio x Carregamento]
  D --> E[Emitir NFe de Venda]
  E --> F[Integração Qive x RM: obter XML] 
  F --> G{NFe correta?}
  G -- Não --> X2[Cancelar etapas anteriores e detalhar motivo] --> E
  G -- Sim --> H[Vincular OS com NFe<br/>OS contém filial destino 1:1 e quant<br/>não travar por divergência]
  H --> I[Receber NFe - Atualizar Estoque Origem]
  I --> J[Emitir NFe de Saída]
  J --> K[Receber NFe - Atualizar Estoque Destino<br/>+ anexo obrigatório]
  K --> L[Fim]
```

## F1 — Compra Direta (estados)

```mermaid
stateDiagram-v2
  [*] --> OS_CRIADA
  OS_CRIADA --> MATERIAL_FABRICADO
  MATERIAL_FABRICADO --> APROVACAO_MEDICAO_PENDENTE
  APROVACAO_MEDICAO_PENDENTE --> MEDICAO_APROVADA
  APROVACAO_MEDICAO_PENDENTE --> MEDICAO_REPROVADA
  MEDICAO_REPROVADA --> MATERIAL_FABRICADO
  MEDICAO_APROVADA --> ROMANEIO_CONFERIDO
  ROMANEIO_CONFERIDO --> NFE_EMITIDA
  NFE_EMITIDA --> XML_OBTIDO
  XML_OBTIDO --> NFE_VALIDADA_OK
  XML_OBTIDO --> NFE_VALIDADA_NOK
  NFE_VALIDADA_NOK --> CORRECAO_EMISSAO_OU_VINCULO
  CORRECAO_EMISSAO_OU_VINCULO --> NFE_EMITIDA
  NFE_VALIDADA_OK --> VINCULO_CRIADO
  VINCULO_CRIADO --> ESTOQUE_ORIGEM_ATUALIZADO
  ESTOQUE_ORIGEM_ATUALIZADO --> NFE_SAIDA_ORIGEM_EMITIDA
  NFE_SAIDA_ORIGEM_EMITIDA --> EM_TRANSITO
  EM_TRANSITO --> CHEGADA_MATERIAL_DESTINO
  CHEGADA_MATERIAL_DESTINO --> ENTRADA_DESTINO_PENDENTE_ANEXO
  ENTRADA_DESTINO_PENDENTE_ANEXO --> ENTRADA_DESTINO_CONCLUIDA
  ENTRADA_DESTINO_CONCLUIDA --> ESTOQUE_DESTINO_ATUALIZADO
  ESTOQUE_DESTINO_ATUALIZADO --> PROCESSO_CONCLUIDO
  PROCESSO_CONCLUIDO --> [*]
```

## F2 — Entrega Futura (mãe) — alto nível

```mermaid
flowchart LR
  A[Atualizar OS com Data Estimada] --> B[Emitir NFe para Entrega Futura]
  B --> C[Integração Qive x RM: obter XML]
  C --> D{NFe correta?}
  D -- Não --> X[Cancelar etapas anteriores] --> B
  D -- Sim --> E[Receber NFe - Sem Atualizar Estoque]
  E --> F[Criar OC para Receber Material no Futuro]
  F --> G[Fim - Aguardando Remessa]
```

## F2 — Entrega Futura (mãe) — estados

```mermaid
stateDiagram-v2
  [*] --> OS_CRIADA
  OS_CRIADA --> OS_ATUALIZADA_DATA_ESTIMADA
  OS_ATUALIZADA_DATA_ESTIMADA --> NFE_ENTREGA_FUTURA_EMITIDA
  NFE_ENTREGA_FUTURA_EMITIDA --> XML_OBTIDO
  XML_OBTIDO --> NFE_VALIDADA_OK
  XML_OBTIDO --> NFE_VALIDADA_NOK
  NFE_VALIDADA_NOK --> CORRECAO_EMISSAO_OU_VINCULO
  CORRECAO_EMISSAO_OU_VINCULO --> NFE_ENTREGA_FUTURA_EMITIDA
  NFE_VALIDADA_OK --> NFE_RECEBIDA_SEM_ESTOQUE
  NFE_RECEBIDA_SEM_ESTOQUE --> OC_CRIADA_PARA_REMESSA
  OC_CRIADA_PARA_REMESSA --> AGUARDANDO_REMESSA
  AGUARDANDO_REMESSA --> PROCESSO_CONCLUIDO
  PROCESSO_CONCLUIDO --> [*]
```

## F3 — Entrega Futura (filha) — alto nível

```mermaid
flowchart LR
  A[OC Pendente de Entrega Futura] --> B{Autorizar Entrega Efetiva?}
  B -- Não --> X[Reprovar com motivo] --> A
  B -- Sim --> C[Emitir NFe de Remessa]
  C --> D[Integração Qive x RM: obter XML]
  D --> E{NFe correta?}
  E -- Não --> Y[Cancelar etapas anteriores] --> C
  E -- Sim --> F[Vincular OS↔OC↔NFe]
  F --> G[Atualizar Estoque Origem - Baixa]
  G --> H[Atualizar Estoque Destino - Entrada]
  H --> I[Fim]
```

## F3 — Entrega Futura (filha) — estados

```mermaid
stateDiagram-v2
  [*] --> OC_PENDENTE_ENTREGA_FUTURA
  OC_PENDENTE_ENTREGA_FUTURA --> APROVACAO_ENTREGA_PENDENTE
  APROVACAO_ENTREGA_PENDENTE --> ENTREGA_APROVADA
  APROVACAO_ENTREGA_PENDENTE --> ENTREGA_REPROVADA
  ENTREGA_REPROVADA --> OC_PENDENTE_ENTREGA_FUTURA
  ENTREGA_APROVADA --> NFE_REMESSA_EMITIDA
  NFE_REMESSA_EMITIDA --> XML_OBTIDO
  XML_OBTIDO --> NFE_VALIDADA_OK
  XML_OBTIDO --> NFE_VALIDADA_NOK
  NFE_VALIDADA_NOK --> CORRECAO_EMISSAO_OU_VINCULO
  CORRECAO_EMISSAO_OU_VINCULO --> NFE_REMESSA_EMITIDA
  NFE_VALIDADA_OK --> VINCULADA_OS_OC_NFE
  VINCULADA_OS_OC_NFE --> ENTRADA_ORIGEM_CONCLUIDA
  ENTRADA_ORIGEM_CONCLUIDA --> SAIDA_ORIGEM_CONCLUIDA
  SAIDA_ORIGEM_CONCLUIDA --> EM_TRANSITO
  EM_TRANSITO --> CHEGADA_MATERIAL_DESTINO
  CHEGADA_MATERIAL_DESTINO --> ENTRADA_DESTINO_PENDENTE_ANEXO
  ENTRADA_DESTINO_PENDENTE_ANEXO --> ENTRADA_DESTINO_CONCLUIDA
  ENTRADA_DESTINO_CONCLUIDA --> ESTOQUE_DESTINO_ATUALIZADO
  ESTOQUE_DESTINO_ATUALIZADO --> PROCESSO_CONCLUIDO
  PROCESSO_CONCLUIDO --> [*]
  ENTREGA_REPROVADA --> PROCESSO_CANCELADO
  PROCESSO_CANCELADO --> [*]
```

## Modelo de dados (relacionamentos)

```mermaid
erDiagram
  OS ||--o{ VINCULO : possui
  OC ||--o{ VINCULO : agrega
  NFE ||--o{ VINCULO : referencia
  VINCULO ||--o{ ANEXO : possui
  OS ||--o{ AUDITORIA_EVENTO : gera
  NFE ||--o{ AUDITORIA_EVENTO : gera
  OC ||--o{ AUDITORIA_EVENTO : gera
```

## Arquitetura de Camadas (Clean Architecture)

```mermaid
graph TB
  subgraph "API Layer"
    API[ASP.NET Core API<br/>Controllers, Swagger]
  end
  
  subgraph "Application Layer"
    APP[Use Cases<br/>Orquestração de Fluxos]
  end
  
  subgraph "Domain Layer"
    DOM[Regras de Negócio<br/>State Machine<br/>Entidades]
  end
  
  subgraph "Infrastructure Layer"
    INF[EF Core<br/>Email<br/>Integration Adapter]
  end
  
  API --> APP
  APP --> DOM
  APP --> INF
  INF --> DOM
  
  style API fill:#e1f5ff
  style APP fill:#fff4e1
  style DOM fill:#ffe1f5
  style INF fill:#e1ffe1
```

## Fluxo de Integração Qive ↔ RM

```mermaid
sequenceDiagram
  participant Qive as Qive/RM
  participant API as API Endpoint
  participant Adapter as Integration Adapter
  participant Domain as Domain Layer
  participant DB as Database
  
  Qive->>API: POST /integrations/qive/nfe-recebida<br/>(XML/metadados)
  API->>Adapter: ReceiveNFeAsync(event)
  Adapter->>Adapter: Verificar idempotência<br/>(chave de acesso)
  alt Já processado
    Adapter-->>API: Resultado anterior
  else Novo evento
    Adapter->>Domain: Processar NFe
    Domain->>DB: Persistir evento
    Domain->>DB: Criar pendência (se necessário)
    Domain-->>Adapter: Sucesso
    Adapter-->>API: 202 Accepted
  end
```

## Fluxo de Validação Fiscal

```mermaid
sequenceDiagram
  participant Fiscal as Fiscal
  participant API as API
  participant Domain as Domain
  participant StateMachine as State Machine
  participant Audit as Auditoria
  
  Fiscal->>API: POST /fiscal/nfe/{chave}/validacao<br/>{decisao: CORRETA/INCORRETA}
  API->>Domain: Validar NFe
  Domain->>StateMachine: Verificar estado atual
  alt NFe CORRETA
    StateMachine->>StateMachine: Transição: NFE_VALIDADA_OK
    StateMachine->>Audit: Registrar evento
    Audit->>Audit: Salvar trilha
    Domain-->>API: 200 OK
  else NFe INCORRETA
    StateMachine->>StateMachine: Transição: NFE_VALIDADA_NOK
    StateMachine->>StateMachine: Cancelar etapas anteriores
    StateMachine->>Audit: Registrar evento + motivo
    Audit->>Audit: Salvar trilha
    Domain-->>API: 200 OK (com etapas canceladas)
  end
```

## Fluxo de Notificações

```mermaid
flowchart TD
  A[Evento no Sistema] --> B{Tipo de Evento?}
  B -->|Chegada Material| C[Notificar Adm. Destino]
  B -->|NFe Entrada| D[Notificar Adm. Destino]
  B -->|NFe Saída Pronta| E[Notificar Adm. Origem]
  B -->|Cancelamento| F[Notificar Responsáveis]
  B -->|Medição Concluída| G[Notificar Gestor]
  B -->|Alerta 7 dias| H[Notificar Gestor]
  B -->|Alerta 30 dias| I[Notificar Gestor + Adm. Destino]
  
  C --> J[EmailSender]
  D --> J
  E --> J
  F --> J
  G --> J
  H --> J
  I --> J
  
  J --> K[Registrar Auditoria]
  K --> L[Salvar Notificação]
```

## Matriz de Responsabilidades (RACI)

```mermaid
graph LR
  subgraph "Papéis"
    GESTOR[Gestor Contrato]
    ADM_ORIGEM[Adm. Origem]
    ADM_DESTINO[Adm. Destino]
    FISCAL[Fiscal]
    FABRICA[Fábrica/Inspetor]
  end
  
  subgraph "Ações"
    CRIAR_OS[Criar OS]
    VINCULAR[Vincular OS/NFe]
    VALIDAR[Validar NFe]
    ANEXAR[Anexar Evidência]
    APROVAR[Aprovar Medição]
  end
  
  GESTOR -.->|A| APROVAR
  ADM_ORIGEM -->|R| CRIAR_OS
  ADM_ORIGEM -->|R| VINCULAR
  ADM_DESTINO -->|R| ANEXAR
  FISCAL -->|R| VALIDAR
  FABRICA -.->|C| APROVAR
```

## Resumo de Diagramas

| Tipo | Quantidade | Localização |
|------|------------|-------------|
| Flowchart (Alto Nível) | 3 | F1, F2, F3 |
| State Diagram | 3 | F1, F2, F3 |
| ER Diagram | 1 | Modelo de Dados |
| Architecture Diagram | 1 | Clean Architecture |
| Sequence Diagram | 2 | Integração, Validação Fiscal |
| Flowchart (Notificações) | 1 | Fluxo de Notificações |
| RACI Diagram | 1 | Responsabilidades |
| **Total** | **12** | Este arquivo |
