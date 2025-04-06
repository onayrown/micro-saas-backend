@echo off
echo Iniciando containers de suporte para debug do backend...

echo Verificando se containers MongoDB e Redis estão rodando...
docker ps | findstr "mongodb"
if %errorlevel% neq 0 (
    echo MongoDB não está rodando. Iniciando...
    docker-compose up -d mongodb
) else (
    echo MongoDB já está rodando.
)

docker ps | findstr "redis"
if %errorlevel% neq 0 (
    echo Redis não está rodando. Iniciando...
    docker-compose up -d redis
) else (
    echo Redis já está rodando.
)

echo Verificando status dos containers necessários...
docker ps -a | findstr "mongodb redis"

echo.
echo ===== MICROSAAS AMBIENTE DE DEBUG =====
echo MongoDB: mongodb://localhost:27017
echo Redis: localhost:6379
echo =========================================
echo.
echo Agora você pode iniciar o debug do backend pelo Visual Studio 2022.
echo O Visual Studio irá iniciar o container do backend automaticamente.
echo.
pause 