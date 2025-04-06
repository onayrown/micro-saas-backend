# Executar como administrador
$hostsPath = "$env:windir\System32\drivers\etc\hosts"
$hostEntry = "127.0.0.1 mongodb"

# Verifica se a entrada já existe
$content = Get-Content -Path $hostsPath -Raw
if ($content -notmatch [regex]::Escape($hostEntry)) {
    # Adiciona a entrada
    Add-Content -Path $hostsPath -Value "`n$hostEntry" -Force
    Write-Host "Entrada adicionada ao arquivo hosts."
} else {
    Write-Host "Entrada já existe no arquivo hosts."
}

Write-Host "Configuração concluída. Pressione qualquer tecla para sair."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") 