#!/bin/bash
set -e

echo "🔍 Validando OpenAPI..."
if command -v swagger-cli &> /dev/null; then
  swagger-cli validate docs/contracts/openapi.yaml
elif command -v redocly &> /dev/null; then
  redocly lint docs/contracts/openapi.yaml
else
  echo "⚠️  swagger-cli ou redocly não encontrado. Pulando validação OpenAPI."
  echo "   Instale com: npm install -g @apidevtools/swagger-cli"
fi

echo ""
echo "🔍 Validando build .NET..."
dotnet build --no-restore

echo ""
echo "🔍 Validando testes..."
dotnet test --no-build

echo ""
echo "🔍 Validando links em docs..."
./scripts/check-links.sh

echo ""
echo "✅ Validações concluídas!"
