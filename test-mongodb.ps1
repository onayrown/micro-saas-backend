# Testar conexão com MongoDB
$mongoHost = "mongodb"
$mongoPort = 27017

Write-Host "Testando todas as possíveis conexões com MongoDB..."

# Array de hosts para testar
$hostsToTest = @("mongodb", "localhost", "172.19.0.2", "127.0.0.1")

foreach ($hostName in $hostsToTest) {
    Write-Host "Testando conexão com $hostName na porta $mongoPort..."
    
    try {
        $tcpClient = New-Object System.Net.Sockets.TcpClient
        $connectionResult = $tcpClient.BeginConnect($hostName, $mongoPort, $null, $null)
        $waitResult = $connectionResult.AsyncWaitHandle.WaitOne(5000, $false)

        if ($waitResult) {
            Write-Host "✅ Conexão bem-sucedida com ${hostName}:${mongoPort}" -ForegroundColor Green
            $tcpClient.EndConnect($connectionResult)
        } else {
            Write-Host "❌ Tempo limite excedido ao conectar a ${hostName}:${mongoPort}" -ForegroundColor Red
        }
        $tcpClient.Close()
    } catch {
        Write-Host "❌ Erro ao testar conexão com ${hostName}: $_" -ForegroundColor Red
    }
}

# Teste usando o comando mongosh (se estiver instalado)
try {
    Write-Host "`nTentando conexão usando mongosh (se estiver disponível)..."
    $mongoCommand = Get-Command mongosh -ErrorAction SilentlyContinue
    
    if ($mongoCommand) {
        Write-Host "Mongosh está instalado, tentando conectar..."
        $output = mongosh --eval "db.runCommand({ping:1})" mongodb:27017 2>&1
        Write-Host $output -ForegroundColor Cyan
    } else {
        Write-Host "Mongosh não está instalado."
    }
} catch {
    Write-Host "Erro ao executar mongosh: $_" -ForegroundColor Red
}

Write-Host "`nRecomendações para seu arquivo appsettings.json:"
Write-Host "Experimente estas strings de conexão em ordem:"
Write-Host "1. mongodb://mongodb:27017" -ForegroundColor Yellow
Write-Host "2. mongodb://localhost:27017" -ForegroundColor Yellow
Write-Host "3. mongodb://172.19.0.2:27017" -ForegroundColor Yellow
Write-Host "4. mongodb://127.0.0.1:27017" -ForegroundColor Yellow

Write-Host "`nPressione qualquer tecla para continuar..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") 