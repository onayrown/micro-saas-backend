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

## Benefícios desta Estrutura

Esta estrutura garante:

1. **Separação clara de responsabilidades**: Cada camada tem um propósito bem definido
2. **Dependências unidirecionais**: As dependências fluem apenas em uma direção (de fora para dentro)
3. **Isolamento do domínio**: A lógica de negócio permanece isolada de detalhes técnicos
4. **Facilidade de manutenção e evolução**: Alterações em uma camada não afetam outras
5. **Testabilidade do código**: Facilita a escrita de testes unitários sem dependências externas

## Diretrizes para Desenvolvimento

Ao desenvolver ou corrigir código no MicroSaaS:

1. Sempre verifique se as referências de projeto estão de acordo com a estrutura definida
2. Se precisar acessar uma entidade de domínio na camada de infraestrutura, mapeie para uma entidade própria
3. Interfaces devem ser definidas na camada de aplicação, implementações na infraestrutura
4. Use DTOs para transferir dados entre as camadas, evitando expor entidades de domínio
5. Não crie dependências circulares entre projetos

## Anotações sobre Implementações Específicas

### Análise de Performance/Dashboard

Os modelos e serviços para análise de performance seguem a mesma estrutura:

- `PerformanceMetrics`, `DashboardInsights`, `ContentPerformance`: Definidos como entidades no Domain
- Interfaces de repositório: Definidas em Application
- Implementações: Armazenadas na camada de Infrastructure
- Controllers: Expostos na camada Backend

Sempre mantenha esta estrutura ao implementar novas funcionalidades. 