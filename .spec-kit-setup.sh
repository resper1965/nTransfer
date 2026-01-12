#!/bin/bash
# Script de setup do GitHub Spec Kit
# Este script configura o spec-kit como ferramenta de desenvolvimento

set -e

echo "🌱 Configurando GitHub Spec Kit..."

# Verificar se uv está instalado
if ! command -v uv &> /dev/null; then
    echo "❌ Erro: uv não está instalado."
    echo "📦 Instale o uv: curl -LsSf https://astral.sh/uv/install.sh | sh"
    exit 1
fi

# Verificar se specify já está instalado
if command -v specify &> /dev/null; then
    echo "✅ Spec Kit já está instalado"
    specify check
else
    echo "📦 Instalando Spec Kit..."
    uv tool install specify-cli --from git+https://github.com/github/spec-kit.git
    echo "✅ Spec Kit instalado com sucesso!"
fi

# Verificar instalação
echo ""
echo "🔍 Verificando instalação..."
specify check

echo ""
echo "✨ Setup concluído!"
echo ""
echo "📚 Próximos passos:"
echo "   1. Inicialize o projeto: specify init . --ai <assistente>"
echo "   2. Use os comandos /speckit.* no chat do seu assistente de IA"
echo "   3. Consulte SPEC-KIT.md para mais informações"
