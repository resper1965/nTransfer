#!/bin/bash
set -e

echo "📦 Aplicando migrations..."

# Usar o container de build que já tem tudo configurado
docker run --rm \
    --network transferencia_transferencia-network \
    -v "$(pwd):/app" \
    -w /app \
    -e "ConnectionStrings__DefaultConnection=Host=transferencia-postgres;Port=5432;Database=transferencia_materiais;Username=transferencia;Password=transferencia" \
    mcr.microsoft.com/dotnet/sdk:8.0 \
    bash -c "
        cd /app && \
        dotnet restore && \
        dotnet build && \
        dotnet ef --version || dotnet tool install --global dotnet-ef --version 8.0.0 && \
        export PATH=\"\$PATH:/root/.dotnet/tools\" && \
        dotnet ef database update \
            --project src/TransferenciaMateriais.Infrastructure \
            --startup-project src/TransferenciaMateriais.Api \
            --connection 'Host=transferencia-postgres;Port=5432;Database=transferencia_materiais;Username=transferencia;Password=transferencia'
    "
