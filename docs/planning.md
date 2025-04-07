# MicroSaaS - Documento de Contexto de Projeto

## 1. Visão Geral

**Produto:** Plataforma SaaS para gestão de conteúdo digital para criadores de conteúdo
**Público-alvo:** Criadores de conteúdo independentes, pequenas empresas, micro-influenciadores
**Status atual:** MVP em desenvolvimento (Backend: 100% concluído, Frontend: 58% concluído)
**Meta de lançamento:** 8 semanas

## 2. Arquitetura e Stack Tecnológico

### 2.1 Arquitetura
- **Padrão:** Clean Architecture
- **Estrutura Backend:**
  - MicroSaaS.Shared: Elementos compartilhados (enums, constantes, DTOs básicos)
  - MicroSaaS.Domain: Entidades de negócio, regras de domínio, interfaces de repositório
  - MicroSaaS.Application: Casos de uso, DTOs, validadores, interfaces de serviço
  - MicroSaaS.Infrastructure: Implementações concretas, integração com APIs externas, persistência
  - MicroSaaS.Backend: Controllers, configuração API, autenticação

### 2.2 Stack Tecnológico
- **Backend:**
  - Framework: .NET 8.0 com C#
  - API: ASP.NET Core Web API
  - Banco de Dados: MongoDB (NoSQL)
  - Autenticação: JWT
  - Cache: Redis
  - Testes: xUnit, Moq, FluentAssertions

- **Frontend:**
  - Framework: React com TypeScript
  - Estado: React Context API + Hooks
  - Cliente HTTP: Axios
  - Visualização de Dados: Recharts
  - Estrutura: Organizado por funcionalidades e componentes

- **DevOps:**
  - Containerização: Docker + Docker Compose
  - Backend hospedado em Google Cloud Free Tier
  - Frontend planejado para Vercel/Netlify/GitHub Pages

## 3. Modelo de Domínio

### 3.1 Entidades Principais
- **User:** Criadores de conteúdo (Id, Nome, Email, Plano...)
- **SocialAccount:** Contas em redes sociais (Id, UserId, Plataforma, Token...)
- **ScheduledPost:** Postagens agendadas (Id, Título, Conteúdo, DataAgendamento...)
- **MediaItem:** Mídias associadas às postagens (Id, PostId, Tipo, Url...)
- **PostPerformance:** Métricas de desempenho (Id, PostId, Curtidas, Comentários...)
- **Checklist:** Listas de tarefas para planejamento (Id, UserId, Título...)
- **ChecklistItem:** Itens dentro de checklists (Id, ChecklistId, Descrição, Status...)
- **MonetizationData:** Dados de monetização (Id, UserId, Fonte, Receita...)

### 3.2 Relacionamentos
- User 1→* SocialAccount
- User 1→* ScheduledPost
- User 1→* Checklist
- User 1→* MonetizationData
- ScheduledPost 1→* MediaItem
- ScheduledPost 1→1 PostPerformance
- Checklist 1→* ChecklistItem

## 4. Funcionalidades do MVP e Status

| Funcionalidade | Descrição | Status Backend | Status Frontend |
|----------------|-----------|----------------|-----------------|
| **Agendador de Conteúdo** | Conexão com 6 plataformas principais, programação de postagens | ✅ 100% | 🔄 45% |
| **Painel de Desempenho** | Métricas consolidadas de engajamento, relatórios | ✅ 100% | 🔄 40% |
| **Sugestão de Horários** | Análise de horários de pico por plataforma | ✅ 100% | 🔄 20% |
| **Checklists para Organização** | Criação de listas de tarefas para planejamento | ✅ 100% | 🔄 30% |
| **Monetização Simplificada** | Integração com Google AdSense | ✅ 100% | 🔄 25% |

## 5. Tarefas Prioritárias Atuais

1. **Finalizar autenticação no frontend** (Em progresso)
   - Implementar renovação automática de tokens expirados

2. **Implementar integração completa com APIs de gerenciamento de conteúdo** (Pendente)
   - Finalizar serviços para agendamento de posts
   - Implementar upload de mídia
   - Integrar seleção de redes sociais

3. **Finalizar dashboard principal** (Pendente)
   - Implementar filtros por data/período
   - Adicionar filtros por plataforma
   - Integrar visualizações detalhadas

4. **Completar interface de sugestão de horários** (Pendente)
   - Finalizar componente de horários recomendados
   - Integrar com formulário de agendamento

5. **Finalizar checklists e organização** (Pendente)
   - Implementar definição de prazos
   - Adicionar alertas de tarefas pendentes

6. **Implementar dashboard de monetização** (Pendente)
   - Finalizar visualização de métricas
   - Implementar conexão com AdSense

## 6. Padrões de Implementação

### 6.1 API RESTful
- Versionamento via URL: `/api/v1/resource`
- Autenticação via JWT
- Respostas padronizadas com formato consistente

### 6.2 Frontend
- Componentes organizados por features
- Hooks personalizados para lógica reutilizável
- Context API para estado global
- Serviços para chamadas à API

## 7. Diretrizes de Segurança e Qualidade de Código

### 7.1 Segurança

#### 7.1.1 Armazenamento de Dados
- **NUNCA armazenar dados sensíveis no localStorage ou sessionStorage**
  - Dados sensíveis incluem: IDs de usuário, informações pessoais, tokens de acesso não-JWT
  - Apenas tokens JWT podem ser armazenados no localStorage, e mesmo assim com tempo de expiração curto
- **Dados de usuário devem ser obtidos da API a cada sessão**
  - Utilizar o contexto de autenticação para gerenciar dados do usuário em memória
  - Nunca persistir dados de usuário no navegador

#### 7.1.2 Autenticação e Autorização
- Implementar renovação automática de tokens
- Validar permissões tanto no frontend quanto no backend
- Implementar logout automático após período de inatividade

#### 7.1.3 Proteção contra Vulnerabilidades
- Implementar proteção contra CSRF em todas as requisições
- Sanitizar todos os inputs de usuário
- Utilizar Content Security Policy (CSP) para prevenir XSS

### 7.2 Qualidade de Código

#### 7.2.1 Dados Reais vs. Placeholders
- **NUNCA utilizar dados fictícios ou placeholders hardcoded na interface**
  - Exibir mensagens claras quando dados não estiverem disponíveis
  - Implementar estados de carregamento (loading) e erro apropriados
- **Todos os textos exibidos devem vir de fontes confiáveis:**
  - API
  - Arquivos de tradução/localização
  - Constantes definidas em um único lugar

#### 7.2.2 Tratamento de Erros
- Implementar tratamento de erros consistente em todas as chamadas à API
- Fornecer feedback claro ao usuário em caso de falhas
- Registrar erros em serviço de logging para análise posterior

#### 7.2.3 Testes
- Implementar testes unitários para toda lógica de negócio
- Implementar testes de integração para fluxos críticos
- Manter cobertura de testes acima de 80%

### 7.3 Princípios Fundamentais

- **Nada de Gambiarras**: Soluções temporárias são estritamente proibidas
- **Código para Produção**: Todo código deve ser escrito como se fosse para produção imediata
- **Documentação Obrigatória**: Qualquer dívida técnica deve ser documentada em tasks.md
- **Segurança em Primeiro Lugar**: Nunca comprometer a segurança em favor da conveniência
- **Qualidade Consistente**: Manter o mesmo nível de qualidade em todo o código
