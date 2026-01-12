#!/bin/bash
# Script para gerar secrets aleatórios para produção

echo "🔐 Gerador de Secrets - Transferência de Materiais Entre Filiais"
echo ""

# Função para gerar string aleatória
generate_random() {
    local length=$1
    openssl rand -base64 $length | tr -d "=+/" | cut -c1-$length
}

# Função para gerar senha forte
generate_password() {
    openssl rand -base64 32 | tr -d "=+/" | cut -c1-32
}

echo "📝 Gerando secrets aleatórios..."
echo ""

# Database
DB_PASSWORD=$(generate_password)
echo "DATABASE_URL=Host=seu-host-db;Port=5432;Database=transferencia_materiais;Username=transferencia_prod;Password=${DB_PASSWORD}"

# Email SMTP Password
EMAIL_PASSWORD=$(generate_password)
echo "EMAIL_SMTP_PASSWORD=${EMAIL_PASSWORD}"

# API Keys
API_KEY_READONLY=$(generate_random 32)
API_KEY_ADMIN=$(generate_random 32)
echo "API_KEY_READONLY=${API_KEY_READONLY}"
echo "API_KEY_ADMIN=${API_KEY_ADMIN}"

# JWT Secret
JWT_SECRET=$(generate_random 64)
echo "JWT_SECRET=${JWT_SECRET}"

# Integration Adapter Keys
QIVE_API_KEY=$(generate_random 32)
RM_API_KEY=$(generate_random 32)
echo "INTEGRATION_ADAPTER_QIVE_API_KEY=${QIVE_API_KEY}"
echo "INTEGRATION_ADAPTER_RM_API_KEY=${RM_API_KEY}"

echo ""
echo "✅ Secrets gerados!"
echo ""
echo "⚠️  IMPORTANTE:"
echo "   1. Copie os valores acima"
echo "   2. Adicione ao seu .env.production ou secrets do GitHub"
echo "   3. NUNCA commite esses valores no Git"
echo "   4. Guarde em local seguro (password manager)"
