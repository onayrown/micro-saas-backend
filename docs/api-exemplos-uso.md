# Exemplos de Uso da API do MicroSaaS

Este documento fornece exemplos detalhados de como utilizar a API do MicroSaaS. Cada seção inclui exemplos de requisições e respostas para os principais endpoints.

## Índice

1. [Autenticação](#autenticação)
2. [Gerenciamento de Posts](#gerenciamento-de-posts)
3. [Publicação de Conteúdo](#publicação-de-conteúdo)
4. [Analytics](#analytics)
5. [Gestão de Receitas](#gestão-de-receitas)
6. [Integração com Mídias Sociais](#integração-com-mídias-sociais)

## Autenticação

### Registro de Usuário

**Requisição:**
```http
POST /api/v1/Auth/register
Content-Type: application/json

{
  "email": "usuario@exemplo.com",
  "password": "Senha@123",
  "name": "Nome do Usuário",
  "username": "usuario_exemplo"
}
```

**Resposta:**
```json
{
  "success": true,
  "data": {
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "email": "usuario@exemplo.com",
    "name": "Nome do Usuário",
    "username": "usuario_exemplo"
  },
  "message": "Registro realizado com sucesso"
}
```

### Login

**Requisição:**
```http
POST /api/v1/Auth/login
Content-Type: application/json

{
  "email": "usuario@exemplo.com",
  "password": "Senha@123"
}
```

**Resposta:**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresIn": 3600,
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Nome do Usuário",
    "email": "usuario@exemplo.com"
  },
  "message": "Login realizado com sucesso"
}
```

## Gerenciamento de Posts

### Criar um Novo Post

**Requisição:**
```http
POST /api/v1/ContentPost
Content-Type: application/json
Authorization: Bearer {token}

{
  "creatorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "Meu novo post",
  "content": "Conteúdo do meu post #hashtag",
  "platform": "Instagram",
  "scheduledDate": "2023-06-15T14:30:00Z",
  "mediaUrls": [
    "https://exemplo.com/imagem1.jpg"
  ],
  "tags": [
    "lifestyle", "dicas"
  ]
}
```

**Resposta:**
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "title": "Meu novo post",
    "content": "Conteúdo do meu post #hashtag",
    "platform": "Instagram",
    "scheduledDate": "2023-06-15T14:30:00Z",
    "status": "Scheduled",
    "mediaUrls": [
      "https://exemplo.com/imagem1.jpg"
    ],
    "tags": [
      "lifestyle", "dicas"
    ],
    "creatorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "createdAt": "2023-06-10T08:15:00Z",
    "updatedAt": "2023-06-10T08:15:00Z"
  },
  "message": "Post criado com sucesso"
}
```

### Obter Posts Agendados para um Criador

**Requisição:**
```http
GET /api/v1/ContentPost/scheduled/3fa85f64-5717-4562-b3fc-2c963f66afa6
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "success": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "title": "Post Agendado para Instagram",
      "content": "Conteúdo do post #hashtag",
      "platform": "Instagram",
      "scheduledDate": "2023-06-15T14:30:00Z",
      "status": "Scheduled",
      "mediaUrls": [
        "https://exemplo.com/imagem1.jpg"
      ],
      "tags": [
        "lifestyle", "dicas"
      ],
      "creatorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "createdAt": "2023-06-10T08:15:00Z",
      "updatedAt": "2023-06-10T08:15:00Z"
    }
  ],
  "message": "Posts agendados recuperados com sucesso"
}
```

### Atualizar um Post

**Requisição:**
```http
PUT /api/v1/ContentPost/3fa85f64-5717-4562-b3fc-2c963f66afa6
Content-Type: application/json
Authorization: Bearer {token}

{
  "title": "Título atualizado",
  "content": "Conteúdo atualizado #hashtag",
  "platform": "Instagram",
  "scheduledDate": "2023-06-16T14:30:00Z",
  "mediaUrls": [
    "https://exemplo.com/imagem1.jpg",
    "https://exemplo.com/imagem2.jpg"
  ],
  "tags": [
    "lifestyle", "dicas", "novidade"
  ]
}
```

**Resposta:**
```
Status: 204 No Content
```

## Publicação de Conteúdo

### Publicar um Post Imediatamente

**Requisição:**
```http
POST /api/v1/Publishing/publish-now
Content-Type: application/json
Authorization: Bearer {token}

{
  "creatorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "Meu novo post",
  "content": "Conteúdo do meu post #microsaas",
  "platform": "Instagram",
  "mediaUrls": [
    "https://exemplo.com/imagem1.jpg"
  ],
  "tags": [
    "microsaas", "conteudo"
  ]
}
```

**Resposta:**
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "creatorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "title": "Meu novo post",
    "content": "Conteúdo do meu post #microsaas",
    "platform": "Instagram",
    "mediaUrls": [
      "https://exemplo.com/imagem1.jpg"
    ],
    "tags": [
      "microsaas", "conteudo"
    ],
    "status": "Published",
    "publishedAt": "2023-06-10T10:15:30Z",
    "createdAt": "2023-06-10T10:15:30Z"
  },
  "message": "Post publicado com sucesso"
}
```

### Obter Histórico de Posts Publicados

**Requisição:**
```http
GET /api/v1/Publishing/history
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "success": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "creatorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "title": "Meu post publicado",
      "content": "Conteúdo do meu post #microsaas",
      "platform": "Instagram",
      "mediaUrls": [
        "https://exemplo.com/imagem1.jpg"
      ],
      "tags": [
        "microsaas", "conteudo"
      ],
      "status": "Published",
      "publishedAt": "2023-06-10T10:15:30Z",
      "createdAt": "2023-06-10T10:15:30Z"
    }
  ],
  "message": "Histórico de posts recuperado com sucesso"
}
```

### Obter Estatísticas de Publicação

**Requisição:**
```http
GET /api/v1/Publishing/stats?creatorId=3fa85f64-5717-4562-b3fc-2c963f66afa6
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "success": true,
  "data": {
    "totalPublished": 42,
    "totalScheduled": 15,
    "publishedByPlatform": {
      "Instagram": 15,
      "Twitter": 20,
      "Facebook": 7
    },
    "mostEngagedPosts": [
      {
        "postId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "title": "Post mais engajado 1",
        "platform": "Instagram",
        "publishedAt": "2023-04-15T14:30:00Z",
        "totalEngagement": 450,
        "engagementRate": 12.5
      }
    ]
  },
  "message": "Estatísticas de publicação recuperadas com sucesso"
}
```

## Analytics

### Obter Métricas de Desempenho para um Post

**Requisição:**
```http
GET /api/v1/Analytics/post/3fa85f64-5717-4562-b3fc-2c963f66afa6
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "success": true,
  "data": [
    {
      "postId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "platform": "Instagram",
      "dateRecorded": "2023-06-14T10:00:00Z",
      "likes": 245,
      "comments": 31,
      "shares": 18,
      "views": 1250,
      "impressions": 2150,
      "engagementRate": 12.5,
      "reach": 1850,
      "saves": 42,
      "clickThroughs": 75
    }
  ],
  "message": "Métricas de desempenho recuperadas com sucesso"
}
```

### Obter Análises para um Criador em uma Plataforma

**Requisição:**
```http
GET /api/v1/Analytics/creator/3fa85f64-5717-4562-b3fc-2c963f66afa6?platform=Instagram&startDate=2023-06-01T00:00:00Z&endDate=2023-06-30T23:59:59Z
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "success": true,
  "data": [
    {
      "date": "2023-06-01T00:00:00Z",
      "platform": "Instagram",
      "followers": 5250,
      "followersGrowth": 125,
      "averageEngagement": 8.5,
      "totalPosts": 12,
      "totalLikes": 3450,
      "totalComments": 628,
      "totalShares": 245,
      "totalViews": 28500,
      "impressions": 42000,
      "reach": 35000
    },
    {
      "date": "2023-06-15T00:00:00Z",
      "platform": "Instagram",
      "followers": 5420,
      "followersGrowth": 170,
      "averageEngagement": 9.2,
      "totalPosts": 15,
      "totalLikes": 4125,
      "totalComments": 735,
      "totalShares": 312,
      "totalViews": 32400,
      "impressions": 48500,
      "reach": 39500
    }
  ],
  "message": "Métricas de desempenho recuperadas com sucesso"
}
```

### Obter Resumo de Métricas

**Requisição:**
```http
GET /api/v1/Analytics/summary/3fa85f64-5717-4562-b3fc-2c963f66afa6?period=Last30Days
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "success": true,
  "data": {
    "totalFollowers": 12500,
    "followersGrowth": 580,
    "followersGrowthPercentage": 4.86,
    "averageEngagementRate": 9.8,
    "totalPosts": 42,
    "totalEngagements": 8750,
    "platformBreakdown": {
      "Instagram": {
        "followers": 5420,
        "engagement": 10.2,
        "posts": 18
      },
      "YouTube": {
        "followers": 4850,
        "engagement": 7.5,
        "posts": 12
      },
      "TikTok": {
        "followers": 2230,
        "engagement": 12.8,
        "posts": 12
      }
    },
    "bestPerformingPlatform": "TikTok",
    "fastestGrowingPlatform": "Instagram"
  },
  "message": "Resumo de métricas recuperado com sucesso"
}
```

## Gestão de Receitas

### Obter Resumo de Receitas

**Requisição:**
```http
GET /api/v1/Revenue/revenue/3fa85f64-5717-4562-b3fc-2c963f66afa6?startDate=2023-06-01T00:00:00Z&endDate=2023-06-30T23:59:59Z
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "success": true,
  "data": {
    "totalRevenue": 1250.75,
    "currency": "BRL",
    "adSenseRevenue": 850.25,
    "sponsorshipsRevenue": 400.50,
    "affiliateRevenue": 0.00,
    "revenueByPlatform": {
      "Instagram": 450.25,
      "YouTube": 650.50,
      "Blog": 150.00
    },
    "previousPeriodRevenue": 980.50,
    "revenueGrowth": 27.56,
    "projectedRevenue": 1500.00
  },
  "message": "Resumo de receitas recuperado com sucesso"
}
```

### Obter Receitas por Plataforma

**Requisição:**
```http
GET /api/v1/Revenue/revenue/3fa85f64-5717-4562-b3fc-2c963f66afa6/by-platform?startDate=2023-06-01T00:00:00Z&endDate=2023-06-30T23:59:59Z
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "success": true,
  "data": [
    {
      "platform": "Instagram",
      "totalRevenue": 450.25,
      "adRevenue": 150.25,
      "sponsorshipRevenue": 300.00,
      "affiliateRevenue": 0.00,
      "revenuePerPost": 37.52,
      "revenuePerFollower": 0.085,
      "previousPeriodRevenue": 380.50,
      "revenueGrowth": 18.33
    },
    {
      "platform": "YouTube",
      "totalRevenue": 650.50,
      "adRevenue": 550.00,
      "sponsorshipRevenue": 100.50,
      "affiliateRevenue": 0.00,
      "revenuePerPost": 54.21,
      "revenuePerFollower": 0.134,
      "previousPeriodRevenue": 500.00,
      "revenueGrowth": 30.10
    }
  ],
  "message": "Receitas por plataforma recuperadas com sucesso"
}
```

### Obter Métricas de Monetização

**Requisição:**
```http
GET /api/v1/Revenue/monetization/3fa85f64-5717-4562-b3fc-2c963f66afa6?startDate=2023-06-01T00:00:00Z&endDate=2023-06-30T23:59:59Z
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "success": true,
  "data": {
    "rpm": 12.45,
    "cpm": 8.75,
    "ctr": 2.35,
    "revenuePerFollower": 0.125,
    "revenuePerPost": 42.50,
    "revenuePerEngagement": 0.85,
    "bestPerformingContentTypes": [
      {
        "contentType": "Tutorial",
        "averageRevenue": 68.50,
        "totalRevenue": 685.00
      },
      {
        "contentType": "Vlog",
        "averageRevenue": 42.25,
        "totalRevenue": 422.50
      }
    ],
    "bestTimeForPosting": "18:00-21:00",
    "bestDayForPosting": "Quinta-feira"
  },
  "message": "Métricas de monetização recuperadas com sucesso"
}
```

## Integração com Mídias Sociais

### Conectar com Google AdSense

**Requisição:**
```http
POST /api/v1/Revenue/connect-adsense/3fa85f64-5717-4562-b3fc-2c963f66afa6
Content-Type: application/json
Authorization: Bearer {token}

{
  "email": "criador@exemplo.com"
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

### Gerenciar Contas de Mídia Social

**Requisição (Listar contas):**
```http
GET /api/v1/SocialMediaAccount/creator/3fa85f64-5717-4562-b3fc-2c963f66afa6
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "success": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "creatorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "platform": "Instagram",
      "username": "usuario_instagram",
      "profileUrl": "https://instagram.com/usuario_instagram",
      "followers": 5420,
      "isVerified": true,
      "isConnected": true,
      "lastSynced": "2023-06-10T08:15:00Z"
    },
    {
      "id": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
      "creatorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "platform": "YouTube",
      "username": "Canal do Usuário",
      "profileUrl": "https://youtube.com/channel/UC1234567890",
      "followers": 4850,
      "isVerified": false,
      "isConnected": true,
      "lastSynced": "2023-06-10T08:15:00Z"
    }
  ],
  "message": "Contas de mídia social recuperadas com sucesso"
}
```

## Tratamento de Erros

Todos os endpoints da API seguem o mesmo padrão de resposta para erros:

### Exemplo de erro 400 (Bad Request)

```json
{
  "success": false,
  "message": "Dados inválidos na requisição",
  "errors": {
    "email": "O campo Email é obrigatório",
    "password": "A senha deve ter pelo menos 8 caracteres"
  }
}
```

### Exemplo de erro 404 (Not Found)

```json
{
  "success": false,
  "message": "Post não encontrado"
}
```

### Exemplo de erro 500 (Internal Server Error)

```json
{
  "success": false,
  "message": "Erro interno do servidor"
}
``` 

### Lista contendo todos os Endpoints da API (https://localhost:7170/swagger/index.html#/v1)

GET
/api/v1/Analytics/post/{postId}
Obtém métricas de desempenho para um post específico


GET
/api/v1/Analytics/creator/{creatorId}
Obtém análises de desempenho para um criador de conteúdo em uma plataforma específica


GET
/api/v1/Analytics/summary/{creatorId}
Obtém um resumo das métricas de desempenho consolidadas do criador em todas as plataformas


POST
/api/v1/Auth/register
Registra um novo usuário no sistema


POST
/api/v1/Auth/login
Autentica um usuário existente no sistema


POST
/api/v1/Auth/refresh-token
Renova um token de autenticação existente


POST
/api/v1/Auth/revoke-token
Revoga um token de autenticação existente


GET
/api/ContentChecklist/{id}


DELETE
/api/ContentChecklist/{id}


GET
/api/ContentChecklist/creator/{creatorId}


POST
/api/ContentChecklist


POST
/api/ContentChecklist/{checklistId}/items


PUT
/api/ContentChecklist/{checklistId}/items/{itemId}


PUT
/api/ContentChecklist/items/{itemId}/duedate


PUT
/api/ContentChecklist/items/{itemId}/reminder


PUT
/api/ContentChecklist/items/{itemId}/priority


GET
/api/ContentChecklist/creator/{creatorId}/upcoming


GET
/api/ContentChecklist/creator/{creatorId}/due


GET
/api/ContentChecklist/creator/{creatorId}/overdue


GET
/api/v1/creators/{id}


PUT
/api/v1/creators/{id}


DELETE
/api/v1/creators/{id}


POST
/api/v1/creators


GET
/api/v1/creators/me


GET
/api/v1/ContentPost/scheduled/{creatorId}
Obtém todos os posts agendados para um criador específico


POST
/api/v1/ContentPost
Cria um novo post de conteúdo


GET
/api/v1/ContentPost/{id}
Obtém um post específico pelo ID


PUT
/api/v1/ContentPost/{id}
Atualiza um post existente


GET
/api/Dashboard/insights/{creatorId}
Obtém as insights mais recentes do dashboard para um criador específico


GET
/api/Dashboard/insights/{creatorId}/generate
Gera novos insights para o dashboard de um criador específico


GET
/api/Dashboard/metrics/{creatorId}


GET
/api/Dashboard/metrics/{creatorId}/daily


GET
/api/Dashboard/content/{creatorId}/top


GET
/api/Dashboard/recommendations/{creatorId}/posting-times


GET
/api/Dashboard/analytics/{creatorId}/engagement


GET
/api/Dashboard/analytics/{creatorId}/revenue-growth


GET
/api/Dashboard/analytics/{creatorId}/follower-growth


POST
/api/Dashboard/metrics


POST
/api/Dashboard/content-performance


GET
/api/Performance/insights/{creatorId}/{platform}


GET
/api/Performance/summary/{creatorId}


GET
/api/PerformanceMetrics/dashboard


GET
/api/PerformanceMetrics/content/{contentId}


GET
/api/PerformanceMetrics/creator/{creatorId}


POST
/api/PerformanceMetrics/refresh


POST
/api/v1/Publishing/publish-now
Publica um post imediatamente em uma plataforma de mídia social


GET
/api/v1/Publishing/history
Obtém o histórico de posts publicados pelo usuário atual


GET
/api/v1/Publishing/stats
Obtém estatísticas de publicação para um criador específico


GET
/api/v1/Publishing/engagement/{postId}
Obtém dados de engajamento para um post específico


POST
/api/v1/Publishing/republish
Republica um post existente em uma plataforma de mídia social


GET
/api/v1/Recommendation/best-times/{creatorId}
Obtém recomendações de melhores horários para postagem em uma plataforma específica


GET
/api/v1/Recommendation/best-times/{creatorId}/all-platforms
Obtém recomendações de melhores horários para postagem em todas as plataformas


GET
/api/v1/Recommendation/content/{creatorId}
Obtém recomendações de conteúdo personalizadas para o criador


GET
/api/v1/Recommendation/topics/{creatorId}


GET
/api/v1/Recommendation/formats/{creatorId}


GET
/api/v1/Recommendation/hashtags/{creatorId}


GET
/api/v1/Recommendation/trends


GET
/api/v1/Recommendation/trends/{creatorId}/niche


GET
/api/v1/Recommendation/monetization/{creatorId}


GET
/api/v1/Recommendation/audience-growth/{creatorId}


GET
/api/v1/Recommendation/engagement/{creatorId}


GET
/api/v1/Recommendation/analyze/{contentId}


POST
/api/v1/Recommendation/refresh/{creatorId}


GET
/api/v1/Recommendation/content-insights/{contentId}


GET
/api/v1/Recommendation/high-performance-patterns/{creatorId}


GET
/api/v1/Recommendation/content-recommendations/{creatorId}


GET
/api/v1/Recommendation/topic-suggestions/{creatorId}


GET
/api/v1/Recommendation/format-suggestions/{creatorId}


POST
/api/v1/Revenue/connect-adsense/{creatorId}
Inicia o processo de conexão com o Google AdSense para um criador


GET
/api/v1/Revenue/adsense-callback
Callback que recebe o código de autorização do Google AdSense após autenticação


GET
/api/v1/Revenue/revenue/{creatorId}
Obtém um resumo das receitas de um criador de conteúdo


GET
/api/v1/Revenue/revenue/{creatorId}/by-platform
Obtém receitas detalhadas por plataforma para um criador


GET
/api/v1/Revenue/revenue/{creatorId}/by-day
Obtém as receitas diárias de um criador no período especificado


GET
/api/v1/Revenue/monetization/{creatorId}
Obtém métricas avançadas de monetização para um criador


POST
/api/v1/Revenue/adsense/refresh/{creatorId}
Atualiza os dados de receita do AdSense para um criador


GET
/api/v1/Scheduling/pending
Obtém a lista de posts agendados pendentes de publicação


POST
/api/v1/Scheduling/schedule
Agenda um novo post para publicação futura


PUT
/api/v1/Scheduling/{id}
Atualiza um post agendado existente


DELETE
/api/v1/Scheduling/{id}
Cancela um post agendado


GET
/api/v1/Scheduling/range
Obtém posts agendados dentro de um intervalo de datas


GET
/api/SocialMediaAccount/{creatorId}


POST
/api/SocialMediaAccount/{creatorId}


GET
/api/SocialMediaAccount/callback


DELETE
/api/SocialMediaAccount/{creatorId}/{accountId}


GET
/api/SwaggerDebug/test


GET
/api/SwaggerDebug/manual