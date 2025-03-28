#!/bin/bash
set -e

echo "Criando diretório para certificados..."
mkdir -p ./https

echo "Gerando certificado auto-assinado..."
openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout ./https/aspnetapp.key -out ./https/aspnetapp.crt -subj "/CN=microsaas"

echo "Convertendo para formato PFX..."
openssl pkcs12 -export -out ./https/aspnetapp.pfx -inkey ./https/aspnetapp.key -in ./https/aspnetapp.crt -passout pass:microsaas

echo "Certificado gerado com sucesso em: ./https/aspnetapp.pfx"
echo "Senha do certificado: microsaas"
echo ""
echo "Agora você pode executar 'docker-compose up -d' para iniciar os containers" 