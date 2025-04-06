# Script para iniciar os serviços necessários para debug

Write-Host "Iniciando serviços para debug..."

# Verificar se os containers já existem, se não, criá-los
$mongodbExists = docker ps -a | Select-String "micro-saas-backend-mongodb-1"
$redisExists = docker ps -a | Select-String "micro-saas-backend-redis-1"
$mongoExpressExists = docker ps -a | Select-String "micro-saas-backend-mongo-express-1"

if (-not $mongodbExists) {
    Write-Host "Criando container do MongoDB..."
    docker-compose up -d mongodb
} else {
    # Se existir, verificar se está rodando
    $mongodbRunning = docker ps | Select-String "micro-saas-backend-mongodb-1"
    if (-not $mongodbRunning) {
        Write-Host "Iniciando container existente do MongoDB..."
        docker start micro-saas-backend-mongodb-1
    } else {
        Write-Host "Container do MongoDB já está rodando."
    }
}

if (-not $redisExists) {
    Write-Host "Criando container do Redis..."
    docker-compose up -d redis
} else {
    # Se existir, verificar se está rodando
    $redisRunning = docker ps | Select-String "micro-saas-backend-redis-1"
    if (-not $redisRunning) {
        Write-Host "Iniciando container existente do Redis..."
        docker start micro-saas-backend-redis-1
    } else {
        Write-Host "Container do Redis já está rodando."
    }
}

# Verificar e iniciar o Mongo Express (interface web para MongoDB)
if (-not $mongoExpressExists) {
    Write-Host "Criando container do Mongo Express..."
    docker-compose up -d mongo-express
} else {
    # Se existir, verificar se está rodando
    $mongoExpressRunning = docker ps | Select-String "micro-saas-backend-mongo-express-1"
    if (-not $mongoExpressRunning) {
        Write-Host "Iniciando container existente do Mongo Express..."
        docker start micro-saas-backend-mongo-express-1
    } else {
        Write-Host "Container do Mongo Express já está rodando."
    }
}

# Adicionar entrada no arquivo hosts (requer privilégios de administrador)
Write-Host "Adicionando entrada no arquivo hosts para mongodb..."
$hostsPath = "$env:windir\System32\drivers\etc\hosts"
$hostEntry = "127.0.0.1 mongodb"

# Verifica se a entrada já existe
$content = Get-Content -Path $hostsPath -Raw
if ($content -notmatch [regex]::Escape($hostEntry)) {
    # Se o script não está rodando como administrador, avisa o usuário
    $isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
    if (-not $isAdmin) {
        Write-Host "AVISO: Este script precisa ser executado como administrador para adicionar entradas ao arquivo hosts." -ForegroundColor Yellow
        Write-Host "Por favor, execute este script novamente como administrador." -ForegroundColor Yellow
    } else {
        # Adiciona a entrada
        Add-Content -Path $hostsPath -Value "`n$hostEntry" -Force
        Write-Host "Entrada adicionada ao arquivo hosts."
    }
} else {
    Write-Host "Entrada já existe no arquivo hosts."
}

# Verificar se é possível conectar ao MongoDB
Write-Host "`nTestando conexão com MongoDB..."
$mongoPort = 27017
$tcpClient = New-Object System.Net.Sockets.TcpClient
$connectionResult = $tcpClient.BeginConnect("localhost", $mongoPort, $null, $null)
$waitResult = $connectionResult.AsyncWaitHandle.WaitOne(5000, $false)

if ($waitResult) {
    Write-Host "Conexão bem-sucedida com MongoDB na porta $mongoPort" -ForegroundColor Green
    $tcpClient.EndConnect($connectionResult)
} else {
    Write-Host "Não foi possível conectar ao MongoDB na porta $mongoPort. Verifique se o serviço está funcionando corretamente." -ForegroundColor Red
}
$tcpClient.Close()

# Verificar se é possível conectar ao Mongo Express
Write-Host "`nTestando conexão com Mongo Express..."
$mongoExpressPort = 8081
$tcpClient = New-Object System.Net.Sockets.TcpClient
$connectionResult = $tcpClient.BeginConnect("localhost", $mongoExpressPort, $null, $null)
$waitResult = $connectionResult.AsyncWaitHandle.WaitOne(5000, $false)

if ($waitResult) {
    Write-Host "Conexão bem-sucedida com Mongo Express na porta $mongoExpressPort" -ForegroundColor Green
    Write-Host "Acesse a interface web do MongoDB em: http://localhost:8081" -ForegroundColor Green
    Write-Host "Usuário: admin | Senha: senha123" -ForegroundColor Green
    $tcpClient.EndConnect($connectionResult)
} else {
    Write-Host "Não foi possível conectar ao Mongo Express na porta $mongoExpressPort. O serviço pode estar iniciando ou com problemas." -ForegroundColor Yellow
}
$tcpClient.Close()

Write-Host "`nConfiguração para debug concluída." -ForegroundColor Green
Write-Host "Agora você pode iniciar o debug no Visual Studio."

Write-Host "`nPressione qualquer tecla para continuar..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") 