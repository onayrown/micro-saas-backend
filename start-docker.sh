#!/bin/bash

# Gerar certificado SSL para HTTPS
echo "Gerando certificado SSL para HTTPS..."
chmod +x ./setup-cert.sh
./setup-cert.sh

# Parar qualquer container em execução
echo "Parando containers existentes..."
docker-compose down

# Limpar dados antigos (opcional)
# echo "Limpando volumes de dados anteriores..."
# docker volume rm microsaas_mongodb_data microsaas_redis_data

# Construir e iniciar containers
echo "Construindo e iniciando containers..."
docker-compose up --build -d

# Verificar status
echo "Verificando status dos containers..."
docker-compose ps

echo ""
echo "===== MICROSAAS AMBIENTE DE DESENVOLVIMENTO ====="
echo "API: https://localhost:7170/swagger/index.html"
echo "MongoDB Admin: http://localhost:8081"
echo "Redis Admin: http://localhost:8082"
echo "==============================================" 