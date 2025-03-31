# Diretrizes de Arquitetura do MicroSaaS

## Estrutura de Camadas e Referências

A arquitetura do projeto MicroSaaS segue os princípios da Clean Architecture, com separação clara de responsabilidades entre as camadas. É **OBRIGATÓRIO** seguir a estrutura de referências abaixo em todo o desenvolvimento:

### MicroSaaS.Shared
- **Referências**: Nenhuma
- **Conteúdo**: Apenas enums, constantes e tipos compartilhados
- **Responsabilidade**: Fornecer tipos e valores comuns a todas as camadas

### MicroSaaS.Domain
- **Referências**: ✅ Apenas MicroSaaS.Shared
- **Conteúdo**: Entidades de domínio e interfaces de repositório
- **Responsabilidade**: Definir o modelo de domínio e as regras de negócio centrais

### MicroSaaS.Application
- **Referências**: 
  - ✅ MicroSaaS.Domain
  - ✅ MicroSaaS.Shared
- **Conteúdo**: Interfaces de serviço, DTOs e validadores
- **Responsabilidade**: Orquestrar casos de uso da aplicação

### MicroSaaS.Infrastructure
- **Referências**:
  - ✅ MicroSaaS.Application
  - ✅ MicroSaaS.Shared
  - 🚫 **NÃO DEVE** referenciar MicroSaaS.Domain
- **Conteúdo**: Implementações de repositórios e serviços
- **Responsabilidade**: Fornecer implementações concretas para interfaces definidas em Application

### MicroSaaS.Backend
- **Referências**:
  - ✅ MicroSaaS.Application
  - ✅ MicroSaaS.Infrastructure
  - 🚫 **NÃO DEVE** referenciar MicroSaaS.Domain
- **Conteúdo**: Controllers e configurações da API
- **Responsabilidade**: Lidar com requisições HTTP e coordenar a aplicação

### MicroSaaS.Tests
- **Referências**:
  - ✅ MicroSaaS.Application
  - ✅ MicroSaaS.Domain
  - ✅ MicroSaaS.Shared
  - 🚫 **NÃO DEVE** referenciar MicroSaaS.Infrastructure
- **Conteúdo**: Testes unitários e de integração
- **Responsabilidade**: Verificar o comportamento do código

## Arquitetura do Frontend

### MicroSaaS.Frontend
- **Tecnologia**: React (versão LTS mais recente)
- **Arquitetura**: Totalmente independente do backend
- **Comunicação**: Apenas via API RESTful
- **Estrutura**: Arquitetura modular baseada em componentes
- **Responsabilidade**: Interface de usuário e experiência do usuário

### Princípios do Frontend

1. **Independência Total**: O frontend deve funcionar de forma completamente independente do backend, comunicando-se exclusivamente via APIs RESTful.

2. **Substituibilidade**: A arquitetura deve permitir a substituição completa do frontend por outra implementação (seja outra tecnologia web ou aplicativos móveis) sem impactar o backend.

3. **Desacoplamento**: Não deve haver dependências diretas entre o código frontend e o código backend além dos contratos de API.

4. **Estado Gerenciado no Cliente**: O estado da aplicação deve ser gerenciado no cliente, com armazenamento local quando apropriado.

5. **Estrutura Modular**: Organização em módulos/componentes que reflitam os domínios funcionais da aplicação.

### Organização do Código Frontend

- **components/**: Componentes React reutilizáveis
- **pages/**: Componentes de página completos
- **hooks/**: Hooks personalizados do React
- **services/**: Serviços de comunicação com o backend
- **contexts/**: Contextos React para gerenciamento de estado
- **utils/**: Utilitários e funções auxiliares
- **types/**: Definições de tipos TypeScript
- **assets/**: Recursos estáticos (imagens, ícones, etc.)
- **styles/**: Estilos globais e temas

## Benefícios desta Estrutura

Esta estrutura garante:

1. **Separação clara de responsabilidades**: Cada camada tem um propósito bem definido
2. **Dependências unidirecionais**: As dependências fluem apenas em uma direção (de fora para dentro)
3. **Isolamento do domínio**: A lógica de negócio permanece isolada de detalhes técnicos
4. **Facilidade de manutenção e evolução**: Alterações em uma camada não afetam outras
5. **Testabilidade do código**: Facilita a escrita de testes unitários sem dependências externas
6. **Flexibilidade de UI**: Permite substituir completamente a interface sem modificar o backend

## Diretrizes para Desenvolvimento

Ao desenvolver ou corrigir código no MicroSaaS:

1. Sempre verifique se as referências de projeto estão de acordo com a estrutura definida
2. Se precisar acessar uma entidade de domínio na camada de infraestrutura, mapeie para uma entidade própria
3. Interfaces devem ser definidas na camada de aplicação, implementações na infraestrutura
4. Use DTOs para transferir dados entre as camadas, evitando expor entidades de domínio
5. Não crie dependências circulares entre projetos
6. Mantenha o frontend e o backend estritamente separados, comunicando-se apenas via API

## Diretrizes para o Frontend

1. **Responsividade**: Todas as interfaces devem ser responsivas, funcionando bem em dispositivos desktop e móveis
2. **Acessibilidade**: Seguir padrões WCAG para garantir acessibilidade
3. **Performance**: Otimizar carregamento e renderização, utilizando lazy loading e renderização condicional
4. **Testes**: Implementar testes unitários para componentes e testes end-to-end para fluxos críticos
5. **Estado**: Gerenciar estado de forma eficiente usando Context API e/ou Redux
6. **API**: Isolar chamadas de API em serviços dedicados para facilitar manutenção e testes
7. **Segurança**: Implementar autenticação JWT segura e proteção contra vulnerabilidades comuns
8. **Dados Simulados (Mock)**: Implementar dados simulados para todas as APIs para permitir desenvolvimento independente do backend

## Abordagem de Resiliência Frontend

Para garantir uma experiência de usuário consistente e facilitar o desenvolvimento paralelo do frontend e backend, estabelecemos uma abordagem padronizada para todas as páginas:

### 1. Estrutura de Serviços

Todos os serviços de API devem:

- Ser organizados por domínio funcional (ex: RecommendationService, AnalyticsService, etc.)
- Utilizar o serviço `api.ts` centralizado para chamadas HTTP
- Incluir tipagem completa para requisições e respostas
- Implementar tratamento de erros abrangente

### 2. Dados Simulados (Mock)

Cada serviço deve incluir:

- Um objeto `mockData` com dados simulados realistas
- Implementação de fallback para dados simulados quando a API não estiver disponível
- Estrutura de dados simulados correspondente à estrutura esperada da API

Exemplo de estrutura para serviços:

```typescript
// NomeServiço.ts
import api from './api';
import { TiposNecessários } from '../types/common';

// Dados simulados para quando a API não estiver disponível
const mockData = {
  // Estrutura de dados simulados...
};

class NomeServiço {
  async métodoServiço(): Promise<TipoRetorno> {
    try {
      const response = await api.get('/endpoint');
      return response.data;
    } catch (error) {
      console.warn('Erro ao chamar API, usando dados simulados:', error);
      return mockData.dadosSimulados;
    }
  }
}
```

### 3. Componentes de Interface

Todas as páginas devem:

- Indicar visualmente quando dados simulados estão sendo usados (ex: badge "Dados de Demonstração")
- Implementar estados de loading, erro e dados vazios
- Usar degradação elegante quando serviços estão indisponíveis
- Priorizar experiência do usuário, evitando telas de erro

### 4. Tratamento de Estado

Para gerenciar o estado dos dados da API:

- Utilizar estados React para armazenar dados carregados da API
- Implementar lógica para detectar quando dados simulados estão sendo usados
- Fornecer feedback visual ao usuário sobre o estado da conexão com backend
- Manter mensagens de erro técnicas no console, evitando mostrar detalhes técnicos ao usuário final

### 5. Estratégia de Atualização de Dados

Para garantir consistência:

- Implementar mecanismo de refresh manual para o usuário atualizar dados
- Considerar uso de polling ou websockets para atualizações automáticas quando apropriado
- Armazenar em cache dados que não mudam frequentemente
- Utilizar estratégias optimistic UI para ações do usuário

### Benefícios Desta Abordagem

Esta estratégia proporciona:

1. **Desenvolvimento Paralelo**: Frontend e backend podem ser desenvolvidos simultaneamente
2. **Capacidade de Demonstração**: O aplicativo pode ser demonstrado mesmo sem um backend funcional
3. **Resiliência**: A aplicação permanece utilizável mesmo quando o backend apresenta problemas
4. **Melhor UX**: Usuários recebem feedback claro sobre o estado do sistema
5. **Testes Facilitados**: Componentes podem ser testados sem dependências externas

Todas as novas páginas e funcionalidades devem seguir esta abordagem para garantir consistência e qualidade em toda a aplicação.

## Recomendações para o Frontend

Após experiências anteriores, as seguintes diretrizes são OBRIGATÓRIAS para o desenvolvimento do frontend:

Antes de tudo ler esse artigo sobre boas praticas com React https://medium.com/front-end-weekly/top-react-best-practices-in-2025-a06cb92def81#id_token=eyJhbGciOiJSUzI1NiIsImtpZCI6IjgyMWYzYmM2NmYwNzUxZjc4NDA2MDY3OTliMWFkZjllOWZiNjBkZmIiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiIyMTYyOTYwMzU4MzQtazFrNnFlMDYwczJ0cDJhMmphbTRsamRjbXMwMHN0dGcuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhdWQiOiIyMTYyOTYwMzU4MzQtazFrNnFlMDYwczJ0cDJhMmphbTRsamRjbXMwMHN0dGcuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJzdWIiOiIxMTMwMzEwNDcyOTA3MjY3MzM0ODMiLCJlbWFpbCI6ImZlbGlwZXNnbWFjaGFkb0BnbWFpbC5jb20iLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwibmJmIjoxNzQzNDU0NzE5LCJuYW1lIjoiRmVsaXBlIE1hY2hhZG8iLCJwaWN0dXJlIjoiaHR0cHM6Ly9saDMuZ29vZ2xldXNlcmNvbnRlbnQuY29tL2EvQUNnOG9jS1BnZnB3VHpRRGFBT19PQjMxVUtDbnBndDFORE81S2ppNXFobG5fMzdHOVU1bjZ4OXQ9czk2LWMiLCJnaXZlbl9uYW1lIjoiRmVsaXBlIiwiZmFtaWx5X25hbWUiOiJNYWNoYWRvIiwiaWF0IjoxNzQzNDU1MDE5LCJleHAiOjE3NDM0NTg2MTksImp0aSI6ImNjNjUzZDcwNmRmYjVjZDQxNjk1M2FiMWZjNTQwOTU0ZjUzZTIzY2MifQ.HPMHObnLlZjrMecH_SqZuwVWD5NT5VUzZ5PZi_p-PFBIgQzqlVOTGVl2-ln76rwgboLnhGI7G94T6HYKIIdu7-bSXghmh-7RBBC3Rg4SMTIOGeEhhw4OYyF6dfWQ5uGFkbUQYfLcOPETy-J7aG_jTA5uZ7yLWow4LjagdhDRwNtY2Au7GZGbTegPGeHXr8YonN6ziB86miQ-DjTcjeXqTeCTWtaYPpRGBvKCmJ50bXCGXhvWhHbe-Med27rxcV-nx8h3-ixL3Z_irK9kfOci-RxjEXKdKGv4go53nKQChh7nWq0_sgvcmjd_0zpp19xkE6TLehAXhCy-bM5jHK36HQ
 e aplicar sempre essas boas praticas no nosso desenvolvimento.

1. **Planejamento cuidadoso da arquitetura**:
   - Definir claramente a estrutura, padrões de código e convenções antes de começar
   - Documentar decisões arquiteturais importantes
   - Revisar a arquitetura em equipe antes de implementar
   - Criar templates e exemplos para os principais padrões de componentes

2. **Gerenciamento rigoroso de dependências**:
   - Utilizar versões fixas (não ranges) para todas as bibliotecas principais
   - Verificar compatibilidade entre bibliotecas antes de adicioná-las
   - Documentar propósito e escopo de cada dependência
   - Limitar número de dependências, preferindo funcionalidades nativas quando possível
   - Usar yarn.lock ou package-lock.json para garantir instalações consistentes

3. **Implementação de testes desde o início**:
   - Implementar testes unitários para componentes compartilhados
   - Criar testes de integração para fluxos principais
   - Usar testes como documentação viva do comportamento esperado
   - Implementar pipeline de CI que execute os testes a cada commit

4. **Documentação do código**:
   - Documentar todos os componentes públicos com props e exemplos
   - Criar storybook para visualizar e testar componentes isoladamente
   - Documentar padrões de design e convenções de código
   - Registrar decisões arquiteturais importantes e suas justificativas
   - Sempre que precisar criar um novo documento salvar em micro-saas-backend\docs

5. **Abordagem incremental**:
   - Desenvolver e testar funcionalidades completas de forma incremental
   - Validar cada componente antes de integrá-lo ao sistema
   - Implementar primeiro o "caminho feliz", depois tratar casos de erro
   - Obter feedback frequente sobre componentes e fluxos
   - Fazer revisões de código regulares para garantir consistência

6. **Isolamento de lógica de negócio**:
   - Separar lógica de negócio da lógica de apresentação (hooks vs componentes)
   - Centralizar lógica de chamadas de API em serviços dedicados
   - Usar Context API ou bibliotecas como Redux para gerenciar estado compartilhado
   - Evitar lógica de negócio complexa em componentes de UI

7. **Práticas para evitar problemas comuns**:
   - Evitar aninhamento profundo de componentes e rotas
   - Padronizar tratamento de erros e loading states em toda a aplicação
   - Nunca ter mais de um Router na mesma árvore de componentes
   - Definir claramente a estrutura de rotas em um único lugar
   - Consistência estrita na nomeação de props, funções e componentes
   - Evitar efeitos colaterais em componentes de renderização

Estas recomendações não são opcionais e devem ser seguidas rigorosamente para garantir a estabilidade, manutenibilidade e qualidade do código frontend.