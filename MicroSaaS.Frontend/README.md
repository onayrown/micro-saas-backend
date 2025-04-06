# MicroSaaS Frontend

## Visão Geral

Este é o frontend do projeto MicroSaaS, uma plataforma para criadores de conteúdo gerenciarem suas redes sociais, agendamento de posts e análise de métricas de desempenho e receita.

O frontend é construído com React, TypeScript, e Material UI, oferecendo uma interface moderna e responsiva.

## Estrutura do Projeto

```
src/
├── components/        # Componentes reutilizáveis
│   ├── common/        # Componentes genéricos (botões, cards, etc.)
│   ├── layouts/       # Layouts da aplicação (MainLayout, AuthLayout)
│   └── ui/            # Componentes específicos da UI
├── contexts/          # Contextos React (autenticação, tema, etc.)
├── hooks/             # Hooks personalizados
├── pages/             # Componentes de página
│   ├── analytics/     # Páginas de análise/relatórios
│   ├── auth/          # Páginas de autenticação (login, registro)
│   ├── content/       # Páginas de gerenciamento de conteúdo
│   ├── dashboard/     # Página principal do dashboard
│   ├── profile/       # Páginas de perfil e configurações
│   ├── schedule/      # Páginas de agendamento de conteúdo
│   └── social/        # Páginas de gerenciamento de contas sociais
├── services/          # Serviços para API, autenticação, etc.
├── styles/            # Estilos globais e tema da aplicação
├── types/             # Definições de tipos TypeScript
└── utils/             # Utilitários e funções auxiliares
```

## Principais Funcionalidades

- **Dashboard**: Visão geral de métricas importantes, receita e engajamento
- **Contas Sociais**: Gerenciamento de contas em diferentes plataformas
- **Conteúdo**: Criação, edição e gerenciamento de publicações
- **Agendamento**: Agendamento de publicações em várias plataformas
- **Análises**: Relatórios detalhados de desempenho e receita
- **Perfil**: Gerenciamento de informações do usuário e configurações

## Tecnologias Utilizadas

- **React**: Biblioteca para construção de interfaces
- **TypeScript**: Superset tipado de JavaScript
- **Material UI**: Biblioteca de componentes para React
- **React Router**: Roteamento para aplicações React
- **Axios**: Cliente HTTP para comunicação com a API
- **Recharts**: Biblioteca para criação de gráficos

## Autenticação

O sistema de autenticação utiliza JWT (JSON Web Tokens), com gerenciamento de tokens de acesso e refresh, e rotas protegidas. A autenticação é integrada com a API backend através do serviço `AuthService`.

## Comunicação com API

A comunicação com o backend é feita através de serviços utilizando Axios, com interceptors para gerenciar tokens de autenticação e tratamento de erros. Cada área funcional da aplicação possui um serviço dedicado:

- **AuthService**: Gerencia autenticação e autorização
- **RecommendationService**: Fornece recomendações personalizadas
- **AnalyticsService**: Acesso a métricas e análise de desempenho
- **ContentService**: Gerenciamento de posts e conteúdo

## Arquitetura Resiliente

O frontend implementa uma arquitetura resiliente que permite:

1. **Desenvolvimento Paralelo**: Frontend e backend podem ser desenvolvidos independentemente
2. **Fallback para Dados Simulados**: Quando o backend está indisponível, exibe dados simulados
3. **Feedback Visual**: Indica claramente ao usuário quando dados simulados estão sendo usados
4. **Degradação Elegante**: Mantém a aplicação funcional mesmo em cenários de erro

Para mais detalhes sobre a implementação desta abordagem, consulte o arquivo `docs/arquitetura.md`.

## Temas e Estilos

A aplicação utiliza o sistema de temas do Material UI, permitindo personalização e suporte para modo claro/escuro.

## Configuração do Ambiente

### Configuração da API

O frontend se comunica com o backend através da API REST. A URL base da API é configurada através de variáveis de ambiente:

1. **Variáveis de ambiente principais**:
   - `REACT_APP_API_URL`: Define a URL base da API (ex: `https://localhost:7170/api`)
   
2. **Arquivos de configuração**:
   - `.env`: Configurações padrão válidas para todos os ambientes
   - `.env.local`: Configurações locais que substituem as padrão (não versionado)
   - `.env.development`: Configurações específicas para ambiente de desenvolvimento
   - `.env.production`: Configurações específicas para ambiente de produção

3. **Configuração do Proxy de Desenvolvimento**:
   O arquivo `setupProxy.js` configura um proxy para redirecionar chamadas da API durante o desenvolvimento,
   evitando problemas de CORS. O proxy é configurado automaticamente usando as mesmas variáveis de ambiente.

### Exemplo de Configuração

Para configurar o frontend para acessar um backend em execução na porta 7171:

1. Crie um arquivo `.env.local` na raiz do projeto frontend:
   ```
   REACT_APP_API_URL=https://localhost:7171/api
   PORT=3000
   ```

2. Reinicie o servidor de desenvolvimento:
   ```
   npm start
   ```

**Importante**: Ao alterar arquivos de variáveis de ambiente, é necessário reiniciar o servidor de desenvolvimento para que as mudanças tenham efeito.

## Integração com Backend

O frontend está configurado para se conectar ao backend em:
- API: https://localhost:7171/api
- Swagger (documentação da API): https://localhost:7171/swagger/index.html
- MongoDB Admin: http://localhost:8081
- Redis Admin: http://localhost:8082

## Instalação e Execução

Pré-requisitos:
- Node.js 14.x ou superior
- npm 7.x ou superior

Para instalar e executar:

1. Instale as dependências:
   ```
   npm install
   ```

2. Execute o projeto em modo de desenvolvimento:
   ```
   npm start
   ```

3. Para build de produção:
   ```
   npm run build
   ```

## Estrutura de Componentes

### Layouts
- **MainLayout**: Layout principal da aplicação com sidebar e header
- **AuthLayout**: Layout para páginas de autenticação

### Páginas
- **Dashboard**: Visão geral das métricas e desempenho
- **Conteúdo**: Gerenciamento de posts (drafts, agendados, publicados)
- **Agendamento**: Visualização e gerenciamento de calendário de posts
- **Contas Sociais**: Gerenciamento de contas em diferentes plataformas
- **Análises**: Relatórios de desempenho e receita
- **Perfil**: Gerenciamento de informações do usuário
- **Autenticação**: Login, registro, recuperação de senha 