#!/bin/bash
set -e

echo "🔍 Verificando links em docs/..."

broken_links=0
temp_file=$(mktemp)

find docs -name "*.md" -type f | while read file; do
  grep -oP '\[.*?\]\([^)]+\)' "$file" 2>/dev/null | sed 's/.*(\(.*\))/\1/' | while read link; do
    # Remover âncoras (#)
    target=$(echo "$link" | cut -d'#' -f1)
    
    # Ignorar links externos
    if [[ "$target" =~ ^https?:// ]]; then
      continue
    fi
    
    # Ignorar links vazios
    if [ -z "$target" ]; then
      continue
    fi
    
    # Resolver caminho relativo
    dir=$(dirname "$file")
    resolved="$dir/$target"
    
    # Verificar se arquivo existe
    if [ ! -f "$resolved" ] && [ ! -f "docs/$target" ] && [ ! -f "./$target" ]; then
      echo "❌ Link quebrado em $file: $link" >> "$temp_file"
    fi
  done
done

if [ -s "$temp_file" ]; then
  cat "$temp_file"
  broken_links=$(wc -l < "$temp_file")
  rm "$temp_file"
  echo ""
  echo "❌ Encontrados $broken_links links quebrados."
  exit 1
else
  rm -f "$temp_file"
  echo "✅ Nenhum link quebrado encontrado."
fi
