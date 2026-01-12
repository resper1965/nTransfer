.PHONY: help up down dev test lint build check clean

help: ## Mostra esta mensagem de ajuda
	@echo "Comandos disponíveis:"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "  \033[36m%-15s\033[0m %s\n", $$1, $$2}'

up: ## Sobe serviços auxiliares (Docker Compose)
	docker compose -f infra/docker-compose.yml up -d

down: ## Para serviços auxiliares (Docker Compose)
	docker compose -f infra/docker-compose.yml down

dev: ## Roda a API em modo desenvolvimento
	dotnet run --project src/TransferenciaMateriais.Api

test: ## Roda todos os testes
	dotnet test

lint: ## Roda formatação e análise de código
	dotnet format --verify-no-changes
	dotnet build --no-restore

build: ## Compila a solução
	dotnet build

check: lint test ## Roda lint + test (gate de PR)

clean: ## Limpa artefatos de build
	dotnet clean
