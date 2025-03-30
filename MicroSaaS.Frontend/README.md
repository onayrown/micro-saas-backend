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

O sistema de autenticação utiliza JWT (JSON Web Tokens), com gerenciamento de tokens de acesso e refresh, e rotas protegidas.

## Comunicação com API

A comunicação com o backend é feita através de serviços utilizando Axios, com interceptors para gerenciar tokens de autenticação e tratamento de erros.

## Temas e Estilos

A aplicação utiliza o sistema de temas do Material UI, permitindo personalização e suporte para modo claro/escuro.

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