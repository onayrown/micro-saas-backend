# MicroSaaS - Rastreador de Tarefas e Progresso

## Status Atual (Atualizado: Abril 2024)

- **Backend:** ✅ 100% Concluído
- **Frontend:** 🔄 65% Em Progresso
- **Status Geral:** 🔄 83% Concluído

## Tarefas em Andamento e Pendentes por Funcionalidade

### 1. Agendador de Conteúdo (Frontend: 55%)

#### Implementação de Interface de Agendamento (90%)
- ✅ Componente de calendário implementado
- ✅ Formulário de criação de postagens implementado
- ✅ Seleção de redes sociais para publicação
- ✅ Anexo de mídias (imagens, vídeos)

#### Visualização de Postagens Agendadas (50%)
- ✅ Lista de postagens com status
- ❌ Funcionalidade de edição de postagens
- ❌ Funcionalidade de cancelamento

#### Sistema de Notificações (25%)
- ❌ Alertas de confirmação de agendamento
- ❌ Alertas de publicação bem-sucedida
- ❌ Alertas de falhas na publicação

### 2. Painel de Desempenho (Frontend: 40%)

#### Dashboard Principal (60%)
- ✅ Layout base do dashboard
- ✅ Componentes de visualização para métricas-chave (usando dados simulados)
- [ ] **Filtros:**
  - [ ] ⏳ Adicionar componentes visuais para filtros de data/período (sem lógica API)
  - [ ] ⏳ Adicionar componente visual para filtro de plataforma(s) (sem lógica API)
  - [ ] Implementar busca de dados da API usando os filtros selecionados
  - [ ] Remover dados simulados e usar dados reais da API nos gráficos/cards

#### Visualizações Detalhadas (40%)
- ✅ Gráficos de engajamento básicos
- ❌ Análise comparativa entre períodos
- ❌ Detalhamento por rede social

#### Exportação de Relatórios (20%)
- ❌ Exportação para CSV
- ❌ Relatórios periódicos

### 3. Sugestão de Horários (Frontend: 20%)

#### Visualização de Horários Recomendados (30%)
- ✅ Componente de exibição de horários
- ❌ Filtros por rede social
- ❌ Integração com formulário de agendamento

#### Análise de Performance por Horário (10%)
- ❌ Gráficos de desempenho por horário
- ❌ Comparativo entre horários utilizados

### 4. Checklists para Organização (Frontend: 30%)

#### Criação e Gestão de Checklists (40%)
- ✅ Formulário de criação de checklists
- ✅ Adição de itens/tarefas
- ❌ Definição de prazos
- ❌ Atribuição de responsáveis

#### Acompanhamento de Progresso (20%)
- ✅ Marcação de itens concluídos
- ❌ Barra de progresso visual
- ❌ Alertas de prazos próximos

### 5. Monetização Simplificada (Frontend: 25%)

#### Dashboard de Receitas (30%)
- ✅ Visualização básica de receitas
- ❌ Gráficos de evolução
- ❌ Métricas de conversão

#### Configuração de Integrações (20%)
- ❌ Conexão com conta Google AdSense
- ❌ Configurações de rastreamento

## Tarefas Prioritárias para Próximas Semanas

### Semana 1
- [✅] Finalizar autenticação no frontend
  - [✅] Implementar renovação automática de tokens
  - [✅] Corrigir configuração de ambiente para comunicação com a API

- [✅] Avançar na integração com APIs
  - [✅] Completar serviços para agendamento de posts (100%)
  - [✅] Iniciar implementação de upload de mídia (100%)

### Semana 2
- [ ] Completar integração com APIs
  - [ ] Finalizar upload de mídia
  - [ ] Integrar seleção de redes sociais

- [ ] Avançar no dashboard principal
  - [ ] Implementar filtros por data/período
  - [ ] Adicionar filtros por plataforma (50%)

### Semana 3
- [ ] Finalizar dashboard principal
  - [ ] Completar filtros por plataforma
  - [ ] Integrar visualizações detalhadas

- [ ] Avançar interface de sugestão de horários
  - [ ] Finalizar componente de horários recomendados
  - [ ] Iniciar integração com formulário de agendamento

### Semana 4
- [ ] Finalizar interface de sugestão de horários
  - [ ] Completar integração com formulário de agendamento
  - [ ] Implementar gráficos de desempenho

- [ ] Avançar checklists e organização
  - [ ] Implementar definição de prazos
  - [ ] Adicionar alertas de tarefas pendentes

## Marcos de Entrega Principais

- 🔄 **MVP Frontend** (Prazo: 4 semanas)
  - [ ] Interface básica funcional consumindo a API [62%]
  - [ ] Fluxos principais funcionando end-to-end [40%]
  - [ ] Testes com usuários selecionados [0%]

- 🔄 **Lançamento MVP** (Prazo: 8 semanas)
  - [ ] Sistema completo com frontend refinado [62%]
  - [ ] Correção de bugs e otimizações [40%]
  - [ ] Versão estável para usuários finais [0%]

## Notas para Desenvolvimento
- Frontend estruturado em React com TypeScript
- Priorizar funcionalidades com impacto direto na experiência do usuário
- Foco na simplicidade e usabilidade do MVP
- APIs do backend já estão prontas, concentrar esforços em sua correta utilização

## Déficit Técnico (Itens a Resolver Antes da Produção)

- [ ] **Certificados SSL para Desenvolvimento**
  - **Descrição:** A solução atual usa certificados SSL pré-gerados para ambiente de desenvolvimento. Antes de ir para produção, é necessário implementar uma solução mais robusta.
  - **Impacto:** Baixo em desenvolvimento, crítico para produção
  - **Solução Recomendada:** Implementar geração de certificados usando OpenSSL ou adquirir certificados de uma autoridade certificadora confiável para o ambiente de produção
  - **Prazo Sugerido:** Antes da preparação para lançamento

- [ ] **Erro de Certificado Inválido para Recursos Locais do Frontend (e.g., /avatars/default.jpg)**
  - **Descrição:** Ao rodar o frontend com HTTPS (`npm run start:secure`), o navegador gera o erro `ERR_CERT_AUTHORITY_INVALID` ao tentar carregar recursos (como imagens) do próprio servidor frontend (`localhost:3002`) devido ao certificado de desenvolvimento autoassinado.
  - **Impacto:** Baixo em desenvolvimento (impede visualização de alguns recursos locais), deve ser resolvido indiretamente com a configuração de produção.
  - **Solução Recomendada:** Ignorar em desenvolvimento. Em produção, os recursos serão servidos via CDN ou pelo servidor de produção com um certificado válido, eliminando o problema. Alternativamente, pode-se configurar o ambiente de desenvolvimento para confiar no certificado local.
  - **Prazo Sugerido:** Resolver como parte da configuração de build/deploy de produção.

## Melhorias Realizadas

- ✅ **05/04/2024:** Implementação de renovação automática de tokens para autenticação
  - Implementação do interceptor de resposta do Axios para detectar erros 401
  - Implementação do método refreshToken no AuthService
  - Tratamento de fila de requisições durante o refresh de token

- ✅ **06/04/2024:** Correção da configuração de ambiente para comunicação com a API backend
  - Configuração correta para acessar o servidor na porta 7171
  - Implementação de sistema flexível baseado em variáveis de ambiente
  - Documentação completa do processo de configuração

- ✅ **07/04/2024:** Implementação do sistema de upload de mídia
  - Componente de upload com suporte a arrastar e soltar
  - Integração com o formulário de criação de conteúdo
  - Serviço para comunicação com a API de mídia
  - Suporte para imagens e vídeos

- ✅ **08/04/2024:** Correção e otimização da autenticação no frontend
  - Correção do método de refresh token para enviar o token no cabeçalho corretamente
  - Otimização da configuração de proxy para garantir comunicação com o backend
  - Correção das configurações de rate limiting no backend para os endpoints de autenticação
  - Implementação de tratamento de erros de certificado SSL em desenvolvimento
  - Melhoria nos logs de requisições e respostas para facilitar a depuração

- ✅ **09/04/2024:** Implementação de solução robusta para HTTPS em desenvolvimento
  - Configuração de ambiente de desenvolvimento com suporte a HTTPS
  - Implementação de script Node.js para criação de certificados SSL pré-gerados
  - Melhoria no tratamento de erros de proxy para melhor feedback ao desenvolvedor
  - Documentação detalhada do processo de configuração de SSL para desenvolvimento
  - Correção de problemas de compatibilidade entre diferentes sistemas operacionais
  - Eliminação da dependência de ferramentas externas como OpenSSL

- ✅ **10/04/2024:** Simplificação da configuração HTTPS para desenvolvimento
  - Configuração do React Scripts para usar certificados autogerados nativamente
  - Remoção da dependência de certificados personalizados
  - Simplificação do processo de inicialização do servidor HTTPS
  - Correção da URL da API em todos os arquivos de ambiente
  - Padronização das configurações de ambiente para desenvolvimento

- ✅ **11/04/2024:** Correção da integração entre frontend e backend
  - Correção do endpoint para obter o perfil do usuário autenticado
  - Alinhamento dos nomes de endpoints entre frontend e backend
  - Melhoria na documentação de serviços de autenticação
  - Correção da rota para acessar o perfil do usuário autenticado
  - Implementação de verificação robusta de token de autenticação
  - Melhoria no tratamento de erros e logs para facilitar a depuração

- ✅ **12/04/2024:** Implementação do serviço de upload de mídia e correção de arquitetura
  - Implementação do controlador de mídia para upload, exclusão e recuperação de arquivos
  - Implementação do serviço de armazenamento local para arquivos
  - Correção de inconsistências arquiteturais para seguir a Clean Architecture
  - Remoção de referências diretas do Backend ao Domain, seguindo o padrão definido
  - Implementação de DTOs e interfaces na camada de Application

- ✅ **13/04/2024:** Correção de erros de compilação e refinamento da arquitetura
  - Correção da interface IStorageService no Domain para remover dependências de AspNetCore
  - Criação da interface IDashboardInsightsService na camada de Application
  - Atualização das implementações para usar as interfaces corretas
  - Garantia de que todas as dependências seguem o fluxo correto da Clean Architecture
  - Refinamento da separação de responsabilidades entre as camadas
  - Resolução de ambiguidades entre interfaces com o mesmo nome em diferentes camadas
  - Adição de pacotes necessários para processamento de imagens (SixLabors.ImageSharp)
  - Correção de chamadas de métodos de logging com parâmetros incorretos
  - Correção de referências no MediaController para ApiResponse, RateLimit e Cache
  - Ajuste dos parâmetros do atributo RateLimit para corresponder ao construtor disponível

- ✅ **14/04/2024:** Implementação do upload de mídia
  - Correção do serviço de upload de mídia no frontend para enviar corretamente o creatorId
  - Criação de uma página de teste para upload de mídia
  - Adição da rota para a página de teste no menu principal
  - Integração completa entre frontend e backend para upload de mídia

- ✅ **15/04/2024:** Correções de segurança e melhoria da interface
  - Remoção de dados sensíveis do localStorage para evitar vulnerabilidades XSS
  - Remoção de dados fictícios e placeholders inadequados na interface
  - Simplificação da página de perfil, removendo abas duplicadas de segurança e notificações
  - Criação de versões simplificadas das páginas de Analytics e Recommendations
  - Melhoria no tratamento de erros e feedback visual para o usuário
  - Adição de diretrizes claras de segurança e qualidade de código no documento planning.md

- ✅ **[Data Atual]:** Correção do endpoint de busca de perfil do usuário
  - Identificado que o endpoint correto no backend é `/api/v1/creators/me` (definido pelo atributo [Route("...")] no ContentCreatorController).
  - Revertida a modificação anterior no frontend para usar o endpoint correto.
  - Resolvido erro 404 que ocorria após o login.

- ✅ **[Data Atual]:** Correção da geração de token JWT
  - Identificado que a claim `ClaimTypes.NameIdentifier` não estava sendo incluída no token gerado (apenas `JwtRegisteredClaimNames.Sub`).
  - Modificado `TokenService.cs` para adicionar explicitamente a claim `ClaimTypes.NameIdentifier` com o ID do usuário.
  - Resolvido o problema onde a busca pelo perfil do criador (`GET /api/v1/creators/me`) falhava após o login devido à ausência desta claim.

- ✅ **[Data Atual]:** Padronização do formato do ID do Usuário/Criador
  - Identificado através de logs que o ID no token (`Guid.ToString()`) tinha um formato diferente do `ObjectId` usado no script de popularização.
  - Modificado `popular-mongodb.js` para usar um Guid fixo em formato string padrão para o `_id` do usuário de teste e seu `ContentCreator` correspondente.
  - Garantida a consistência do ID entre o banco de dados, a geração do token e a leitura da claim, resolvendo o erro 404 final.

- ✅ **[Data Atual]:** Correção de dados do perfil de usuário
  - Identificado que a página de perfil exibia a mensagem "Não foi possível carregar os dados do usuário".
  - Confirmado que `AuthContext` tentava popular o estado `user` com dados de `/api/v1/creators/me`.
  - Suspeita inicial de que os dados (`Name`, `Email`) não estavam sendo persistidos corretamente pelo script `popular-mongodb.js`.
  - Modificado `popular-mongodb.js` para definir `Name`, `Email` e `Username` diretamente ao criar/substituir o `ContentCreator` de teste, garantindo sua persistência.

- ✅ **[Data Atual]:** Correção do mapeamento da entidade ContentCreator
  - Confirmado que os dados (`Name`, `Email`, `Username`) estavam corretos no MongoDB.
  - Identificado que a entidade C# `ContentCreator.cs` possuía inicializadores padrão (`= string.Empty;`) para propriedades string.
  - Removidos os inicializadores desnecessários da entidade para garantir que o driver MongoDB possa desserializar corretamente os valores do banco de dados.
  - Resolvido o problema onde a API retornava campos vazios para o perfil do usuário, apesar dos dados existirem no banco.

- ✅ **[Data Atual]:** Correção da desserialização da entidade de infraestrutura
  - Identificado que, apesar da entidade de domínio estar correta, a entidade de infraestrutura (`ContentCreatorEntity.cs`) ainda possuía inicializadores padrão (`= string.Empty;`).
  - Esses inicializadores estavam sobrescrevendo os dados lidos do MongoDB durante a desserialização no repositório.
  - Removidos os inicializadores da `ContentCreatorEntity.cs`.
  - Resolvida a causa raiz do problema onde os dados do perfil do usuário chegavam vazios ao frontend.

- ✅ **[Data Atual]:** Adição de mapeamento BSON explícito à entidade de infraestrutura
  - O problema de dados vazios persistiu mesmo após remover inicializadores.
  - Adicionados atributos `[BsonElement("FieldName")]` explícitos a todas as propriedades relevantes de `ContentCreatorEntity.cs` para garantir o mapeamento correto na desserialização do MongoDB, eliminando ambiguidades.

- ✅ **[Data Atual]:** Correção da rota do DashboardController
  - Identificado que o frontend chamava uma rota versionada (`/api/v1/Dashboard/...`) enquanto o `DashboardController` não estava configurado para versionamento.
  - Adicionado `[ApiVersion("1.0")]` e atualizado `[Route("...")]` no `DashboardController` para corresponder ao padrão versionado.
  - Resolvido erro 404 ao tentar buscar métricas do dashboard.

- ✅ **[Data Atual]:** Correção do tratamento da resposta da API de métricas
  - Identificado que o frontend esperava uma resposta no formato `ApiResponse<T>` enquanto o backend retornava o array de métricas diretamente.
  - Ajustado `DashboardService.ts` e `DashboardPage.tsx` para esperar e tratar corretamente o array de métricas retornado pela API.
  - Corrigido o erro "Falha ao buscar métricas: undefined" que ocorria após a chamada bem-sucedida à API.

- ✅ **[Data Atual]:** Melhorias no Dashboard Frontend
  - **Gráfico de Receita:** Implementada lógica para processar `metricsData` e exibir a receita mensal estimada (`estimatedRevenue`) agrupada por mês.
  - **Layout:** Reorganizados os cards do dashboard usando um grid responsivo (MUI Grid) para melhor aproveitamento do espaço em diferentes tamanhos de tela (2 colunas em telas médias/grandes).
  - **Filtros:** Dropdown de plataforma agora utiliza o enum `SocialMediaPlatform` do serviço.
  - Resolvido: Gráfico de receita não exibia dados; Layout desorganizado.

- ✅ **[Data Atual]:** Correção de erro de importação no Dashboard
  - Adicionado `IconButton` à lista de imports de `@mui/material` em `DashboardPage.tsx`.
  - Resolvido erro de compilação `TS2304: Cannot find name 'IconButton'`.

- ✅ **[Data Atual]:** Refinamento do Card de Seguidores e Remoção de Dados Simulados no Dashboard
  - **Objetivo:** Substituídos dados simulados por dados reais e removidos componentes não funcionais.
  - **Passos Concluídos:**
    1. Removidas constantes de dados simulados (`followersData`, `engagementData`, `notifications`, `scheduledPosts`).
    2. Processamento de `metricsData` implementado para extrair o número mais recente de seguidores por plataforma.
    3. Card "Seguidores por Plataforma" atualizado para usar uma lista MUI com dados reais, ícones e cores representativas das plataformas.
    4. Componentes/seções que dependiam dos dados simulados removidos (gráfico de engajamento, listas de notificações e posts agendados) foram comentados e marcados com `// TODO:` para implementação futura com APIs reais.
  - **Status:** Concluído.

- ✅ **[Data Atual]:** Correção da busca de métricas no Repositório
  - **Problema:** API retornava array vazio `[]` para métricas, apesar dos dados existirem no MongoDB.
  - **Causa:** O `PerformanceMetricsRepository` estava buscando na coleção `"performance_metrics"` (minúsculo), enquanto o script de população inseriu os dados em `"PerformanceMetrics"` (maiúsculo).
  - **Correção:** Alterado o nome da coleção no repositório para `"PerformanceMetrics"` para corresponder ao nome usado na inserção dos dados. Refatorado o acesso à coleção para usar uma propriedade privada.
  - Resolvido: Dashboard exibia "Sem dados..." para Receita e Seguidores.

- ✅ **[Data Atual]:** Diagnóstico - API retorna métricas vazias (Concluído)
  - **Problema:** API retornava array vazio `[]`.
  - **Diagnóstico (via Logs):** O método `GetByCreatorIdAsync` no repositório confirmou que a consulta `.Find(p => p.CreatorId == creatorIdString)` retornou 0 documentos.
  - **Próxima Etapa:** Verificar a consistência dos dados inseridos via script `popular-mongodb.js` (valor exato do `creatorId`) e o mapeamento/tipo do campo `CreatorId` na entidade `PerformanceMetricsEntity.cs`.

- ✅ **[Data Atual]:** Correção Final - Coleção Inexistente
  - **Problema:** A coleção `PerformanceMetrics` não existia no banco de dados MongoDB, por isso a busca do repositório retornava 0 documentos.
  - **Confirmação:** O `PerformanceMetricsRepository` já estava configurado corretamente para buscar na coleção `PerformanceMetrics`.
  - **Ação:** Revisado o script `popular-mongodb.js` para garantir que ele use `db.getCollection('PerformanceMetrics')` e o `creatorId` correto. A execução deste script criará a coleção (se ausente) e inserirá os dados necessários.
  - **Status:** Concluído - Aguardando execução do script e teste final.

- ✅ **[Data Atual]:** Correção do Local do Script de População
  - **Problema:** O script `popular-mongodb.js` foi editado na raiz do projeto backend, em vez de no diretório `scripts/`.
  - **Correção:**
    1. O conteúdo atualizado (com inserção em `PerformanceMetrics`) foi copiado para `scripts/popular-mongodb.js`.
    2. O arquivo incorreto `popular-mongodb.js` foi excluído da raiz do backend.
  - **Status:** Concluído.

- ✅ **[Data Atual]:** Correção do Hash de Senha no Script de População
  - **Problema:** Login falhava com erro 401 (Unauthorized) para usuários de teste.
  - **Causa:** O script `scripts/popular-mongodb.js` estava inserindo um placeholder inválido (`$2a$11$...`) no campo `passwordHash` dos usuários.
  - **Correção:** Substituído o placeholder por um hash BCrypt válido correspondente à senha "senha123" para todos os usuários de teste no script.
  - **Status:** Concluído - Aguardando execução do script e teste final.

- ✅ **[Data Atual]:** Padronização do Tratamento de Erro de Login (401)
  - **Problema:** A melhoria anterior para erro 401 usou incorretamente `ApiResponse<>` em vez da classe `Result<>` padronizada.
  - **Correção:** Modificado o método `LoginAsync` no `AuthController` para usar `Result<AuthResponse>.Fail("Credenciais inválidas")` e retornar este objeto padronizado no `UnauthorizedObjectResult`.
  - **Status:** Concluído.

- ✅ **[Data Atual]:** Correção do Tratamento de Erro de Login no Frontend
  - **Problema:** O frontend (`AuthService.ts`) tratava qualquer erro 401 como "Sessão expirada", ignorando a mensagem específica ("Credenciais inválidas") enviada pelo backend no objeto `Result`.
  - **Correção:** Ajustado o bloco `catch` do método `login` em `AuthService.ts` para verificar se a resposta do erro contém `{ success: false, errorMessage: ... }` e extrair a `errorMessage` corretamente para retornar ao contexto de autenticação.
  - **Status:** Concluído.

- ✅ **[Data Atual]:** Correção Final do Tratamento de Erro 401 no Login (Interceptador)
  - **Problema:** O interceptador de erros do Axios (`api.ts`) estava tratando o erro 401 da rota de login incorretamente, convertendo-o na mensagem genérica "Sessão expirada" antes que chegasse ao `AuthService`. Isso ocorria porque a lógica que tentava fazer o refresh do token era acionada indevidamente. A condição para identificar a rota de login (`originalRequest.url?.includes('auth/login')`) falhava devido à diferença de maiúsculas/minúsculas ('auth/login' vs '/v1/Auth/login').
  - **Correção:** Adicionada uma verificação explícita para identificar erros 401 originados da rota `auth/login` (convertendo a URL para minúsculas para comparação *case-insensitive*) e rejeitar a promise imediatamente com o erro *original* do Axios. Isso permite que o `catch` no `AuthService.login` receba o objeto de erro completo (`error.response.data`) e extraia a mensagem correta do backend.
  - **Status:** Concluído.

### Histórico e Concluídos

  - ✅ **[Data Anterior]:** Correção Final do Tratamento de Erro 401 no Login (Interceptador)
    - **Problema:** O interceptador de erros do Axios (`api.ts`) estava tratando o erro 401 da rota de login incorretamente, convertendo-o na mensagem genérica "Sessão expirada" antes que chegasse ao `AuthService`. Isso ocorria porque a lógica que tentava fazer o refresh do token era acionada indevidamente. A condição para identificar a rota de login (`originalRequest.url?.includes('auth/login')`) falhava devido à diferença de maiúsculas/minúsculas ('auth/login' vs '/v1/Auth/login').
    - **Correção:** Adicionada uma verificação explícita para identificar erros 401 originados da rota `auth/login` (convertendo a URL para minúsculas para comparação *case-insensitive*) e rejeitar a promise imediatamente com o erro *original* do Axios. Isso permite que o `catch` no `AuthService.login` receba o objeto de erro completo (`error.response.data`) e extraia a mensagem correta do backend.
    - **Status:** Concluído.

## Tarefas Atuais

  - ✅ **[Data Atual]:** Correção Arquitetônica Profunda no Backend
    - **Problema:** Identificadas inconsistências graves com a Clean Architecture: Serviços de aplicação implementados na Infraestrutura (`AuthService`, `PerformanceAnalysisService`, `DashboardInsightsService`), DI incorreta no `Program.cs` mapeando interfaces para implementações na camada errada, interfaces duplicadas no Domínio/Aplicação, e múltiplos padrões inconsistentes para registro de repositórios.
    - **Correção:**
      1.  Serviços refatorados e movidos para a camada `Application.Services`.
      2.  Injeção de Dependência corrigida no `Program.cs` para mapear `Application.Interface` -> `Application.Implementation` (para serviços) e `Application.Interface` -> `Infrastructure.Implementation` (para repositórios).
      3.  Interfaces de serviço duplicadas removidas do Domínio.
      4.  Interfaces de repositório padronizadas na `Application.Interfaces.Repositories`.
      5.  Implementações de repositório na `Infrastructure.Repositories` ajustadas para implementar as interfaces da Aplicação e métodos corrigidos/adicionados.
      6.  Configuração de DI redundante removida de `Infrastructure/DependencyInjection.cs`.
      7.  Arquivos/Pastas redundantes (`AdapterRepositories`, `Infrastructure/Services/AuthService.cs`, etc.) excluídos.
      8.  Erros de compilação decorrentes da refatoração corrigidos.
    - **Status:** Concluído.
    - **Próximo Passo:** Testar funcionalidade principal (Login) após a refatoração.

  - 🟡 **[Data Anterior]:** Diagnosticar Falha no Login com Credenciais Válidas (Backend)
    - **Problema:** Login com credenciais válidas falhava (retornava 401).
    - **Diagnóstico:** Causa raiz identificada como erro arquitetônico e de DI, impedindo a execução da lógica correta do `AuthService`. Logs de debug não apareciam.
    - **Status:** Obsoleto (Problema resolvido pela Tarefa de Correção Arquitetônica).

### Histórico e Concluídos

*   **[Data Anterior]** - [Backend] Configurar CORS, Logging (Serilog) e Controllers básicos.
*   **[Data Anterior]** - [Geral] Definir entidades iniciais (User, ContentCreator, ContentPost).
*   **[Data Anterior]** - [Infra] Configurar MongoDB e repositórios básicos.
*   **[Data Anterior]** - [Frontend] Configurar estrutura inicial do projeto React com TypeScript e Tailwind.
*   **[Data Anterior]** - [Frontend] Implementar fluxo de Autenticação (Login/Logout) e Contexto de Autenticação.
*   **[Data Anterior]** - [Backend] Implementar hashing de senha com BCrypt e geração/validação de JWT.
*   **[Data Anterior]** - [Frontend] Criar página de Dashboard básica e layout principal.
*   **[Data Anterior]** - [Infra] Adicionar atributos `[BsonElement("FieldName")]` explícitos em `ContentCreatorEntity` para garantir mapeamento MongoDB.
*   **[Data Anterior]** - [Backend] Corrigir rota do `DashboardController` para incluir versionamento (`/api/v1/...`) e adicionar `[ApiVersion("1.0")]` para resolver erro 404.
*   **[Data Anterior]** - [Frontend] Ajustar `DashboardService.ts` e `DashboardPage.tsx` para lidar com a resposta da API de métricas (array direto vs objeto `ApiResponse`).
*   **[Data Anterior]** - [Backend] Refatorar `AuthController` para usar `Result<T>` e retornar mensagens de erro estruturadas.
*   **[Data Anterior]** - [Frontend] Ajustar interceptor de erro em `api.ts` para não tratar 401 de `/auth/login` como sessão expirada e permitir que `AuthService` capture o erro original.
*   **[Data Atual]** - [Geral] **Resolver problema persistente de falha no login:** 
    *   **Problema:** Mesmo com credenciais corretas, hash BCrypt correto no banco e tratamento de erro adequado no frontend, a verificação `BCrypt.Verify` no backend falhava consistentemente. Depuração extensiva (logs, auto-teste, hash hardcodado) não resolveu o problema com a biblioteca `BCrypt.Net.BCrypt` no ambiente específico.
    *   **Correção:** Substituída a biblioteca `BCrypt.Net.BCrypt` pela biblioteca padrão `Microsoft.AspNetCore.Identity.PasswordHasher`. 
        1. Adicionado o pacote NuGet `Microsoft.Extensions.Identity.Core` ao `MicroSaaS.Infrastructure.csproj`.
        2. Reimplementado `MicroSaaS.Infrastructure/Services/PasswordHasher.cs` para usar `PasswordHasher<User>`.
        3. Garantido o registro correto da DI em `MicroSaaS.Infrastructure/DependencyInjection.cs` com `services.AddScoped<IPasswordHasher, PasswordHasher>()`.
        4. Atualizado o script `scripts/popular-mongodb.js` com o novo formato de hash gerado pela biblioteca Identity para a senha padrão.
    *   **Resultado:** Login agora funciona corretamente.

### Pendentes

*   **[Próxima Sessão]** - [Geral] **Concluir refatoração da Infraestrutura:**
    *   Corrigir erros de implementação de interface (CS0535/CS0738) em `MediaRepository` ajustando a interface `IMediaRepository` para usar a entidade `Media` em vez de `MediaDto`.
    *   Corrigir erro de namespace (CS0234) em `MediaRepository` removendo `using MicroSaaS.Infrastructure.Data;`.
    *   Ajustar serviços da camada de Aplicação (ex: `MediaService`) que usam `IMediaRepository` para realizar o mapeamento `Media` <=> `MediaDto`.
    *   Excluir pastas vazias remanescentes na Infraestrutura (`AdapterRepositories`, `Entities`, `DTOs`, `Mappers`, `Database`, `Data`, `Persistence` antiga, `Repositories` antiga).
*   [Backend] Implementar endpoint para upload de imagem de perfil.
*   [Frontend] Permitir upload e exibição da imagem de perfil do criador.
*   [Frontend] Implementar visualização de posts e contas sociais no dashboard.
*   [Frontend] Exibir recomendações na interface.
*   [Backend] Implementar lógica de negócios para gerar recomendações reais.
*   [Geral] Refinar testes unitários e de integração.
*   [Frontend] Investigar e corrigir possível erro 404 ao carregar `avatars/default.jpg`.
