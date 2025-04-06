/**
 * Script para criar certificados SSL para desenvolvimento
 *
 * Este script cria certificados SSL para uso em ambiente de desenvolvimento.
 * Ele não depende do OpenSSL ou de outras ferramentas externas.
 */

const fs = require('fs');
const path = require('path');
const os = require('os');

// Diretório onde os certificados serão salvos
const CERT_DIR = path.join(__dirname, '..', 'certificates');

// Verifica se o diretório existe, se não, cria
if (!fs.existsSync(CERT_DIR)) {
  console.log(`Criando diretório ${CERT_DIR}...`);
  fs.mkdirSync(CERT_DIR, { recursive: true });
}

// Caminhos dos arquivos de certificado
const KEY_PATH = path.join(CERT_DIR, 'key.pem');
const CERT_PATH = path.join(CERT_DIR, 'cert.pem');

// Remover certificados existentes
if (fs.existsSync(KEY_PATH)) {
  fs.unlinkSync(KEY_PATH);
  console.log(`Certificado existente removido: ${KEY_PATH}`);
}

if (fs.existsSync(CERT_PATH)) {
  fs.unlinkSync(CERT_PATH);
  console.log(`Certificado existente removido: ${CERT_PATH}`);
}

console.log('Criando certificados SSL para desenvolvimento...');
console.log(`Sistema operacional: ${os.platform()}`);

try {
  // Certificados muito simples para desenvolvimento
  // Estes certificados são apenas para desenvolvimento e não devem ser usados em produção

  // Chave privada RSA no formato tradicional (simplificada)
  const privateKey = `-----BEGIN RSA PRIVATE KEY-----
MIIEpAIBAAKCAQEA3Tz2mr7SZiAMfQfaYp2zmwsTFnMSM9M8kVLtaFZOhjJKcQpZ
UiQFyLgH/vBVyOWHu/XsE3xz5GCjS9fgkSEx7k3Wnreb6cYaPBlICjtJkOY//JRC
JO0OTMCNZuZOXVLEZGsygmF6/4rEjWNOAUfxJqD4e4M6iST9iCVYFP53SDMGPG0N
O4U4dza+K63OX+hNm8lT0CKmCRWQWkVUkk9d+CuEHiuZvnCO9XIYXzWsOQ4ux58e
LEGwSZ4YhZo7QsLfBgyU+SdJJeEGh7vja1Y0Q5JOCvBcMYRFFpvJH1NdCH4YjqXf
GGO1XQ9pu9hVJmxA9UrGSP53YTuZZjSZNQIDAQABAoIBAQCK97qpWNWj1yLyJkMl
0jzDa5OBJnrIjYRttgujGPAUGLI1hDKZG/2iE1Bm8p089MI/4eK0Tj9GEtKGASN+
K4Vwx4V9UfBbWMITHBUKZR5/eSU7AZ8YwMbvuh5XgQ9BoaWOcOxCyKCQKwfBXJ8M
Sy9AV2iJNWxP1Z9CLH5lNnZ1Gg9Plf6B0/pq9S0T3HZBstF9Zderfc1zGjaf+7Ci
JgcgvrzZfTrNrF5QUEbTHdM4LWyMgDNSY7rAm9YCZXRJUtAEqJewXABVGqVxCh3L
sbC5Fl/2XqIBIAp5LNUg5/UIRzNbQw3U3tU2JnTbNbCY7ygj4KX8FNYC4Ey9xwdV
fG2FbtlBAoGBAO/ehgKKPVsKfEZpQ8BMVPqdPjrXOqcUbZZGIyoZO/xxI+Ks/a5K
KGQiQxKAq+rNb5V4S2aZ5T7yM1ueunKW1fTeLOoFVOxnXgJk1JLKPvhn3o/Nwpws
MgQkghRNqS5Tt/jmHUhQ2bLHS6EZ71sVRaRbD+Clruqb/Jb6S0C8I5VRAoGBAOvw
APpIv0HvtKR0LZF9pOr+LmIGVJ8qcFQljQBCvUvXdAcAJKJBVAqF7r8oUHqt+oJz
NuZJwNz+pPUDTKZLhQJKUzQQgWkFJvFYpUZZ7YetS793wWWUVc0RXzMtBXQ+xGBY
QtUnQXFAFD7Ruic9J9YE+i0hvL9e0FfAuKdZ9nS1AoGBAOVWOTkfAB8Ryi9KqbZk
Y+1MgJHCqgCXZqWQxEA8VsGNBIPJqf+8/jnQEJ2i5YzBIrjLFj5GkzKGvtD6+YIw
KCXAkEZyQPUkDXcpGJlpMXRKR2Jj1IHAZs8dSrXcQYHJlQAHLJpZRMmYZIFzZlGp
PFoy9yCUKyKEFoS5+YbJ/2cRAoGAZc8Dxtl6aGHZZIJEZ1zBYZKxGfsSry/XnOrh
EHecbS22ZDy/lsNsg1XWEZd0PnNx2mMWyK/kPMDvQJsJRxZKYQRbLuGUr7nX54XV
fbWYeHDYwLJEXCUVEXOa5piWu8gF6qdCXVTCSvQxA4JavhpDjJwc0RhYOkBQCwl7
B9a5cTUCgYAiNqAhr2XjFpsnPte5yN7M2kmg3mvdcvClngN5QSxkrKGc5uHa+1qR
rZMhuLZ5WwKsWk5yvRmzSAeF8SbQJ6pTWiPnOmRvTjf22qgxRoJqQn6bwIKx+/O3
PxvFj+kVPJHEbvUfS85o/ZCpWDdECnkYbCBXUWrTw9I+exh6v4QkDg==
-----END RSA PRIVATE KEY-----`;

  // Certificado X509 no formato tradicional (simplificado)
  const certificate = `-----BEGIN CERTIFICATE-----
MIIDXTCCAkWgAwIBAgIJAJC1HiIAZAiIMA0GCSqGSIb3DQEBCwUAMEUxCzAJBgNV
BAYTAkFVMRMwEQYDVQQIDApTb21lLVN0YXRlMSEwHwYDVQQKDBhJbnRlcm5ldCBX
aWRnaXRzIFB0eSBMdGQwHhcNMTkwNjA3MDI0MjI5WhcNMTkwNzA3MDI0MjI5WjBF
MQswCQYDVQQGEwJBVTETMBEGA1UECAwKU29tZS1TdGF0ZTEhMB8GA1UECgwYSW50
ZXJuZXQgV2lkZ2l0cyBQdHkgTHRkMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIB
CgKCAQEA3Tz2mr7SZiAMfQfaYp2zmwsTFnMSM9M8kVLtaFZOhjJKcQpZUiQFyLgH
/vBVyOWHu/XsE3xz5GCjS9fgkSEx7k3Wnreb6cYaPBlICjtJkOY//JRCJO0OTMCN
ZuZOXVLEZGsygmF6/4rEjWNOAUfxJqD4e4M6iST9iCVYFP53SDMGPG0NO4U4dza+
K63OX+hNm8lT0CKmCRWQWkVUkk9d+CuEHiuZvnCO9XIYXzWsOQ4ux58eLEGwSZ4Y
hZo7QsLfBgyU+SdJJeEGh7vja1Y0Q5JOCvBcMYRFFpvJH1NdCH4YjqXfGGO1XQ9p
u9hVJmxA9UrGSP53YTuZZjSZNQIDAQABo1AwTjAdBgNVHQ4EFgQUxbnU+5H/Rjj4
A2PpnpSSvtRpKIUwHwYDVR0jBBgwFoAUxbnU+5H/Rjj4A2PpnpSSvtRpKIUwDAYD
VR0TBAUwAwEB/zANBgkqhkiG9w0BAQsFAAOCAQEANhbm8WgIqBDlJPrVQgzZ/vkA
8/sBA7DpjJrFw8Zn0thXhptILyuIKqPRCKg9gBiHX8eikPNrYGdpQ5R6Ub3XhnsJ
rZWIV+Yp0Vx7J6LY4+oCXk1/uPKPVnMV7/WZB7aAEHZ2+UKEJBfYTcZ2BwI7wvvk
k9gdWOvZagI5nJKX+9h1vGzWPtJo/lWKpGLHKKaoYEYPYcJvpPkVCxNjZvwy97Vj
Wx/AMgv9Z8GULGnEIxQcWZ+aY2ilbUsBUglJtSLgF7eHvxCsHYRsKRxVt5JK4+/s
OAEq9nRWd5i6Tz6SY95H+5eu0EXpgMKkXPMc62i8m2YrIjUhiPYnlAQZl7rAmQ==
-----END CERTIFICATE-----`;

  // Salvar arquivos
  fs.writeFileSync(KEY_PATH, privateKey);
  fs.writeFileSync(CERT_PATH, certificate);

  console.log('Certificados criados com sucesso!');
  console.log(`Chave privada: ${KEY_PATH}`);
  console.log(`Certificado: ${CERT_PATH}`);
  console.log('\nPara iniciar o servidor com HTTPS:');
  console.log('npm run start:secure');
} catch (error) {
  console.error('Erro ao criar certificados:', error.message);
  console.error('\nDetalhes do erro:', error);
  process.exit(1);
}
