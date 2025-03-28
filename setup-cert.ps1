$ErrorActionPreference = 'Stop'

Write-Host "Criando diretório para certificados..."
New-Item -ItemType Directory -Force -Path .\https

$cert = New-SelfSignedCertificate -Subject "CN=microsaas" -CertStoreLocation "cert:\CurrentUser\My" -KeyExportPolicy Exportable -KeySpec Signature -KeyLength 2048 -KeyAlgorithm RSA -HashAlgorithm SHA256

$pwd = ConvertTo-SecureString -String "microsaas" -Force -AsPlainText

Write-Host "Exportando certificado para PFX..."
$certPath = ".\https\aspnetapp.pfx"
Export-PfxCertificate -Cert $cert -FilePath $certPath -Password $pwd

Write-Host "Certificado gerado com sucesso em: $certPath"
Write-Host "Senha do certificado: microsaas"
Write-Host ""
Write-Host "Agora você pode executar 'docker-compose up -d' para iniciar os containers" 