# Guia de Autenticação

## Registro de Usuário

Para registrar um novo usuário, envie uma requisição POST para `/api/v1/Auth/register`:

```json
{
  "name": "seu_nome",
  "email": "seu_email@exemplo.com",
  "password": "Sua_Senha_Segura@123"
}
```

## Login

Para fazer login, envie uma requisição POST para `/api/v1/Auth/login`:

```json
{
  "email": "seu_email@exemplo.com",
  "password": "Sua_Senha_Segura@123"
}
```

## Usando o Token de Autenticação

Após o registro ou login bem-sucedido, você receberá um token JWT. Este token deve ser incluído em todas as requisições subsequentes a endpoints protegidos.

**IMPORTANTE:** O token deve ser enviado no cabeçalho `Authorization` como um token `Bearer`:

```
Authorization: Bearer seu_token_jwt_aqui
```

### Exemplo com Swagger

No Swagger, siga estas etapas:

1. Clique no botão "Authorize" no topo da página
2. Insira seu token no formato: `Bearer seu_token_jwt_aqui`
3. Clique em "Authorize"

### Exemplo com Curl

```bash
curl -X 'GET' \
  'https://localhost:7171/api/PerformanceMetrics/dashboard' \
  -H 'Authorization: Bearer seu_token_jwt_aqui'
```

### Exemplo com Postman

1. Selecione a requisição desejada
2. Vá para a aba "Headers"
3. Adicione um header com a chave "Authorization" e o valor "Bearer seu_token_jwt_aqui"

## Credenciais para MongoDB

Para acessar o MongoDB diretamente, use:

- **Host:** localhost
- **Porta:** 27017
- **Banco de dados:** microsaas

### Coleções padronizadas

Todas as coleções do MongoDB agora seguem o padrão PascalCase (primeira letra maiúscula):

- **Users** - Usuários do sistema
- **ContentCreators** - Criadores de conteúdo
- **ContentPosts** - Posts de conteúdo
- **SocialMediaAccounts** - Contas de redes sociais
- **ContentPerformances** - Métricas de desempenho de conteúdo
- **ContentRecommendations** - Recomendações de conteúdo
- **ContentChecklists** - Checklists de conteúdo
- **Analytics** - Dados analíticos
- **DashboardInsights** - Insights para dashboard
- **Schedules** - Agendamentos

### Acesso via mongo-express (interface web)

Para acessar o MongoDB através da interface web mongo-express:

- **URL:** http://localhost:8081
- **Usuário:** admin 
- **Senha:** senha123

**Nota:** Se não conseguir acessar o mongo-express, verifique se o contêiner está rodando com:
```
docker-compose up -d mongo-express
``` 