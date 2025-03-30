# Documentação de Integração com Sistemas Externos

Este documento descreve como integrar sistemas externos com a API do MicroSaaS. A documentação abrange autenticação, requisitos e exemplos de implementação.

## Índice

1. [Visão Geral](#visão-geral)
2. [Autenticação](#autenticação)
3. [Integração com Redes Sociais](#integração-com-redes-sociais)
   - [Instagram](#instagram)
   - [YouTube](#youtube)
   - [TikTok](#tiktok)
   - [Twitter](#twitter)
4. [Integração com Google AdSense](#integração-com-google-adsense)
5. [Webhooks](#webhooks)
6. [Tratamento de Erros](#tratamento-de-erros)
7. [Limitações de Taxa](#limitações-de-taxa)

## Visão Geral

A API do MicroSaaS permite que sistemas externos interajam com a plataforma através de endpoints RESTful. A integração possibilita:

- Gerenciamento de conteúdo em múltiplas redes sociais
- Acesso a métricas de desempenho e análises
- Monitoramento de receitas e monetização
- Recebimento de notificações em tempo real via webhooks

## Autenticação

### Autenticação de API

Para acessar a API do MicroSaaS, sistemas externos devem usar autenticação JWT.

#### Obtenção de Token API

**Requisição:**
```http
POST /api/v1/Auth/api-token
Content-Type: application/json

{
  "apiKey": "sua-api-key",
  "apiSecret": "seu-api-secret"
}
```

**Resposta:**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresIn": 86400
  },
  "message": "Token gerado com sucesso"
}
```

#### Uso do Token

O token deve ser incluído em todas as requisições subsequentes no cabeçalho de autorização:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Integração com Redes Sociais

### Instagram

#### Requisitos

- Conta de desenvolvedor Instagram/Facebook
- Aplicativo registrado na plataforma Meta for Developers
- Permissões: `instagram_basic`, `instagram_content_publish`, `instagram_manage_insights`

#### Fluxo de Autorização

1. **Iniciar Fluxo OAuth**

**Requisição:**
```http
GET /api/v1/SocialMedia/connect/instagram?creatorId=3fa85f64-5717-4562-b3fc-2c963f66afa6
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "success": true,
  "data": {
    "authUrl": "https://api.instagram.com/oauth/authorize?client_id=..."
  },
  "message": "URL de autorização gerada com sucesso"
}
```

2. O usuário autoriza o acesso no Instagram
3. O Instagram redireciona para a URL de callback configurada com um código de autorização
4. O sistema do MicroSaaS troca o código por tokens de acesso e refresh

#### Publicação de Conteúdo

**Requisição:**
```http
POST /api/v1/Publishing/publish-now
Content-Type: application/json
Authorization: Bearer {token}

{
  "creatorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "platform": "Instagram",
  "content": "Conteúdo do post #hashtag",
  "mediaUrls": ["https://exemplo.com/imagem.jpg"],
  "tags": ["exemplo", "hashtag"]
}
```

### YouTube

#### Requisitos

- Conta Google Cloud com YouTube Data API v3 habilitada
- Aplicativo OAuth configurado no Google Cloud Console
- Permissões: `https://www.googleapis.com/auth/youtube`, `https://www.googleapis.com/auth/youtube.upload`

#### Fluxo de Autorização

Similar ao do Instagram, mas usando os endpoints do Google OAuth.

### TikTok

#### Requisitos

- Conta de desenvolvedor TikTok
- Aplicativo registrado na plataforma TikTok for Developers
- Permissões: `user.info.basic`, `video.publish`, `video.list`

#### Limitações

O TikTok tem limites rigorosos de publicação. Verifique a documentação mais recente da API TikTok.

### Twitter

#### Requisitos

- Conta de desenvolvedor Twitter
- Aplicativo registrado no Twitter Developer Portal
- Permissões: `tweet.read`, `tweet.write`, `users.read`

## Integração com Google AdSense

### Requisitos

- Conta de AdSense ativa
- Aplicativo registrado no Google Cloud Console
- API AdSense Management ativada

### Fluxo de Integração

1. **Iniciar conexão com AdSense**

**Requisição:**
```http
POST /api/v1/Revenue/connect-adsense/3fa85f64-5717-4562-b3fc-2c963f66afa6
Content-Type: application/json
Authorization: Bearer {token}

{
  "email": "email-da-conta-adsense@example.com"
}
```

**Resposta:**
```json
{
  "success": true,
  "data": {
    "authorizationUrl": "https://accounts.google.com/o/oauth2/auth?client_id=..."
  },
  "message": "URL de autorização gerada com sucesso"
}
```

2. O usuário autentica na conta Google e concede permissões
3. O Google redireciona para a URL de callback com um código de autorização
4. O MicroSaaS troca o código por tokens de acesso e refresh

### Consulta de Dados de Receita

**Requisição:**
```http
GET /api/v1/Revenue/adsense/3fa85f64-5717-4562-b3fc-2c963f66afa6?startDate=2023-06-01&endDate=2023-06-30
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "success": true,
  "data": {
    "totalRevenue": 1280.45,
    "revenueByDay": [
      {
        "date": "2023-06-01",
        "amount": 42.15
      },
      // ... outros dias
    ],
    "revenueByWebsite": {
      "exemplo.com": 980.25,
      "blog.exemplo.com": 300.20
    }
  },
  "message": "Dados de receita recuperados com sucesso"
}
```

## Webhooks

### Configuração de Webhooks

**Requisição:**
```http
POST /api/v1/Webhook/register
Content-Type: application/json
Authorization: Bearer {token}

{
  "url": "https://seu-sistema.com/microsaas-webhook",
  "secret": "seu-segredo-webhook",
  "events": [
    "post.published",
    "post.scheduled",
    "analytics.updated",
    "revenue.updated"
  ]
}
```

**Resposta:**
```json
{
  "success": true,
  "data": {
    "webhookId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "url": "https://seu-sistema.com/microsaas-webhook",
    "events": [
      "post.published",
      "post.scheduled",
      "analytics.updated",
      "revenue.updated"
    ]
  },
  "message": "Webhook registrado com sucesso"
}
```

### Formato de Payload de Webhook

```json
{
  "event": "post.published",
  "timestamp": "2023-06-15T14:30:00Z",
  "data": {
    "postId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "creatorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "platform": "Instagram",
    "status": "Published"
  }
}
```

### Verificação de Assinatura

Para verificar a autenticidade das chamadas de webhook, o MicroSaaS inclui um cabeçalho de assinatura:

```
X-MicroSaaS-Signature: sha256=5743894a0e93e123fd15a79fa7e8c3af35d3e195
```

A assinatura é calculada como:
```
HMAC-SHA256(secreto_webhook, payload_completo)
```

## Tratamento de Erros

Todas as integrações externas devem implementar um tratamento de erros adequado. A API do MicroSaaS retorna erros padronizados:

```json
{
  "success": false,
  "message": "Descrição do erro",
  "errorCode": "ERROR_CODE",
  "details": {
    "campo": "Descrição específica do erro"
  }
}
```

### Códigos de Erro Comuns

| Código | Descrição |
|--------|-----------|
| `AUTH_FAILED` | Falha de autenticação |
| `PERMISSION_DENIED` | Permissão insuficiente |
| `SOCIAL_INTEGRATION_FAILED` | Falha na integração com rede social |
| `INVALID_TOKEN` | Token inválido ou expirado |
| `RATE_LIMIT_EXCEEDED` | Limite de taxa excedido |

## Limitações de Taxa

Para garantir a estabilidade do sistema, a API do MicroSaaS implementa limites de taxa:

| Tipo de Endpoint | Limite |
|------------------|--------|
| Leitura | 1000 requisições por hora |
| Escrita | 100 requisições por hora |
| Autenticação | 20 requisições por hora |
| Publicação | 50 requisições por hora |

Quando um limite é excedido, a API retorna:

```json
{
  "success": false,
  "message": "Limite de requisições excedido. Tente novamente mais tarde.",
  "errorCode": "RATE_LIMIT_EXCEEDED",
  "details": {
    "limitType": "hourly",
    "resetAt": "2023-06-15T15:00:00Z"
  }
}
```

A resposta inclui o cabeçalho `X-RateLimit-Reset` com um timestamp Unix indicando quando o limite será resetado. 