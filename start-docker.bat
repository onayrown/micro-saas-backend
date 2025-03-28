@echo off
echo Gerando certificado SSL para HTTPS...
powershell -ExecutionPolicy Bypass -File .\setup-cert.ps1

echo Parando containers existentes...
docker-compose down

REM Limpar dados antigos (opcional)
REM echo Limpando volumes de dados anteriores...
REM docker volume rm microsaas_mongodb_data microsaas_redis_data

echo Construindo e iniciando containers...
docker-compose up --build -d

echo Verificando status dos containers...
docker-compose ps

echo.
echo ===== MICROSAAS AMBIENTE DE DESENVOLVIMENTO =====
echo API: https://localhost:7170/swagger/index.html
echo MongoDB Admin: http://localhost:8081
echo Redis Admin: http://localhost:8082
echo ==============================================
pause 