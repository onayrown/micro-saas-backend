$localPort = 27017
$dockerNetworkIp = "172.19.0.2"
$dockerPort = 27017

# Verifique se WinNAT está instalado
if (-not (Get-NetNat -ErrorAction SilentlyContinue)) {
    Write-Host "Instalando WinNAT..."
    New-NetNat -Name DockerNAT -InternalIPInterfaceAddressPrefix "172.19.0.0/16"
}

# Adicione uma regra de redirecionamento de porta temporária
Write-Host "Adicionando redirecionamento de porta de $localPort para $dockerNetworkIp`:$dockerPort"
netsh interface portproxy add v4tov4 listenport=$localPort listenaddress=127.0.0.1 connectport=$dockerPort connectaddress=$dockerNetworkIp

Write-Host "Redirecionamento configurado. Use Ctrl+C para interromper e remover o redirecionamento."

try {
    while ($true) {
        Start-Sleep -Seconds 1
    }
} finally {
    # Removendo o redirecionamento quando o script for interrompido
    Write-Host "Removendo redirecionamento de porta..."
    netsh interface portproxy delete v4tov4 listenport=$localPort listenaddress=127.0.0.1
} 