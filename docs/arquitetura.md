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

## Princípios de Clean Code e SOLID

### Clean Code

Os princípios de Clean Code devem ser seguidos em todo o desenvolvimento do MicroSaaS para garantir a manutenibilidade e legibilidade do código:

1. **Nomes Significativos**:
   - Use nomes que revelem a intenção
   - Evite abreviações ou nomes genéricos como `temp`, `data`, `manager`
   - Prefira nomes específicos: `userRepository` em vez de `repo`

2. **Funções Pequenas e Focadas**:
   - Funções devem fazer apenas uma coisa
   - Limite de 20-30 linhas por função
   - Funções devem ter um único nível de abstração

3. **Comentários Apropriados**:
   - O código deve ser autoexplicativo sempre que possível
   - Use comentários apenas para explicar o "porquê", não o "como"
   - Documente APIs públicas e comportamentos não óbvios

4. **Formatação Consistente**:
   - Siga o estilo de formatação estabelecido (.editorconfig)
   - Agrupe código relacionado
   - Mantenha espaçamento vertical e horizontal consistente

5. **Tratamento de Erros**:
   - Não use exceções para fluxo de controle normal
   - Use o padrão Result para representar sucesso/falha
   - Registre erros com contexto suficiente para diagnóstico

### Princípios SOLID

Todos os componentes do sistema devem aderir aos princípios SOLID:

1. **Princípio da Responsabilidade Única (SRP)**:
   - Uma classe deve ter apenas uma razão para mudar
   - Separe responsabilidades em classes distintas
   - Exemplo: `UserService` não deve lidar com autenticação E gerenciamento de perfil

2. **Princípio do Aberto/Fechado (OCP)**:
   - Classes devem estar abertas para extensão, fechadas para modificação
   - Use abstrações e herança/composição para estender comportamento
   - Exemplo: Adicione novos tipos de autenticação sem modificar o `AuthService`

3. **Princípio da Substituição de Liskov (LSP)**:
   - Subtipos devem ser substituíveis por seus tipos base
   - Mantenha contratos de interface consistentes
   - Não restrinja ou enfraqueça pré-condições em subclasses

4. **Princípio da Segregação de Interface (ISP)**:
   - Interfaces específicas são melhores que interfaces genéricas
   - Clientes não devem depender de métodos que não usam
   - Divida interfaces grandes em interfaces menores e mais específicas

5. **Princípio da Inversão de Dependência (DIP)**:
   - Dependa de abstrações, não de implementações concretas
   - Módulos de alto nível não devem depender de módulos de baixo nível
   - Use injeção de dependência para fornecer implementações

### Padrão Result para Tratamento de Erros

Em vez de usar exceções para controle de fluxo normal, adote o padrão Result:

```csharp
// Em vez disso:
public User GetUserById(Guid id)
{
    var user = _repository.GetById(id);
    if (user == null)
        throw new UserNotFoundException(id);
    return user;
}

// Faça isso:
public Result<User> GetUserById(Guid id)
{
    var user = _repository.GetById(id);
    if (user == null)
        return Result<User>.Failure("Usuário não encontrado");
    return Result<User>.Success(user);
}
```

Benefícios do padrão Result:
- Torna explícito que uma operação pode falhar
- Força o tratamento de erros no código cliente
- Separa o fluxo de erro do fluxo normal
- Reduz o uso de exceções para condições excepcionais reais

## Recomendações para o Frontend

Top React Best Practices In 2025

The user interface is one of the crucial things that users look at when using web applications. You have already won half of the battle when you have good user engagement. In today’s evolving and competitive world, staying always on top can be a challenging part. React is undoubtedly the popular javascript used for building dynamic web applications with enhanced user interfaces. Let’s admit that, a good user interface can only be achieved with clean code and you can only write neat and clean code by following the React JS best practices.

Key takeaways:
Leverage the use of functional components: functional components are easy to read as compared to class components. It helps to maintain code and allows to reuse it for better code performance.
No unnecessary renderings: prefer using React. memo for functional components and pure components for class components to prevent unnecessary renderings. You can also use “useMemo” and “useCallback” to optimize and handle complex calculations.
Make the most out of virtual DOM: the main role of virtual DOM is to facilitate quick rendering and enhance code for higher performance.
Use stateless components: Stateless components are efficient for data handling. Avoid passing unnecessary props and pass only what the components need.
Folder Structure Practices for a React JS Project
React js contains a lot of files and subfolders hence it is essential to create a well-organized folder structure to manage code, configurations, modules, and assets. High-performing web applications can only be made if you follow best practices for folder structure. Let’s dive in!

1)Design a proper Folder layout
Arrange the folders in the below sequence to allow code reusability and improve code maintenance. Keep test, CSS, JavaScript, assets, etc., under a single folder to allow sharing among multiple internal projects. You can group the same file types into the same folder and rearrange it as per your project needs.

Components Folder
Context Folder
Hooks Folder
Services Folder
Utils Folder
Assets Folder
Config Folder
Styles Folder
2) CSS in JS
Using CSS in Javascript can make styling and theme more flexible, and it allows for easy maintenance of large CSS files. You can use the desired libraries from EmotionJS, styled-components, and glamorous depending on the complexity of your theme. Hence, keep all CSS styles in single SCSS files to prevent name conflict issue.

3) Children’s Props
The children props function is useful when you want to render the method of a sub-component inside another component.

4) Higher-Order Components (HOC)
Higher-order components help to transform the advanced level component into higher-order components. it facilitates reusing the logic inside the render method.

React Component Best Practices
Components are the building blocks that will decide the final UI of your React project. Begin with this best react component best practices for efficient coding.

Divide large components into small components
Assigning one component to only one function is the best way to keep the code simple and small. It also helps the development team easily manage and test the code, making the rendering hassle-free for a beautiful design process.

Dont do over-nesting
React JS is known for its code reusability but always break into small components to prevent over nesting components. This way your code will be more organized and understandable hence making code running process easier.

Implement suitable lifecycle methods
Choose the right lifecycle method while calling various methods and perform necessary actions at a suitable time.

Use functional or class components according to the requirement
Never mix logic and data or else you can lose control over rendering. Functional components is useful to show user interface without any state or performing logic. Keep logic as simple as possible in React lifecycle methods such as componentDidMount() and componentDidUpdate(). Complexity of these methods can’t be found in functional components which include only class components.

Use of comments when required
If you add fewer comments for React JS components, the code will be clearer and self-explanatory. Excessive comments can clutter the code, so you should focus on writing clean, readable code instead.

Capitalize the component name
To ensure that JSX identifies and handles your component name, always start the new components name with capital letters. This react js practice is useful bccause it helps developers to differentiate between usernames and HTML tags.

Write testable code
You should write the clear code that is testable. The best practice is to give the test files and original files the same so there is no confusion and easy for a developer to make necessary changes without impacting the existing functionality.

React Code Structure Best Practices
Every developer has his own method of writing code in a unique format and style. Though there is no specific React code structure format, below, we have researched some of the best React Best Practices that will help you write clear and concise code.

Use linter
The whole React JS project runs by linking components. ESlint is a popular tool used by React JS developers to enhance the quality of code. It is also best from safety point of view as it also keeps check and prevents breaches on predefined standards. Overall, it promotes code stability and helps developers to fix spelling, unused variables and correct coding issues on real-time.

Avoid repeating the code(DRY code)
DRY refers to “don’t repeat yourself” which means you should not repeat or create a same name component as this could conflict with other file names and your code might not run properly.

Avoid unnecessary use of Divs
A lot of Divs can make the code look messy and poor. Avoid using div for a single component. If there a multiple components in the same file, you can use the div in shorthand form.

Implement destructuring
Destructuring is a useful feature in the Javascript function that extracts the form data and assigns variables from the object or array. You must include destructuring props to make your code cleaner and easier to read.

Use ES6 spread functions
ES6 function is used to pass an object property. Your web app can become complex with the growing code and bunch of files. Use {…props} between the open and close tag to automatically insert the props object.

Try the map function for dynamic rendering
You can utilize props in React to generate dynamic HTML without repeating code. The map() function allows you to show arrays quickly. To keep things structured, use map() with one item from the array as a key.

React Security Best Practices
React is obviously the safe framework for web application development but there are still many concerns to keep in mind to ensure full security during the entire development. Below are some of the proven react security best practices you must follow .

Monitor for URL-based injection or fishy URL
Use native URL parsing functionality to make sure your URL is genuine and valid. Always use http or https against your javascript: URL-based script injection.

Sanitize the dynamic values with DOM purify
You should always include sanitization libraries like DOM purify if you are using dangerouslySetInnerHTML. Hence this way, developer can directly enter the HTML conten only in HTML element of the app.

Dont use user-generated properties with create element API
Don’t ever do the mistake of using user-generated properties, always use linter configuration or check code manually to prevent malicious or other unsafe activities.

Secure your server-side rendering
Remember to secure your server-side rendering. Data binding will lead to automatic data escape when using server-side functions. For added security, you can use server-side functions like ReactDOMServer.renderToString() and ReactDOMServer.renderToStaticMarkup().

Also, before sending the output to the client, you should avoid adding strings with renderToStaticMarkup().

Avoid using dangerouslySetInnerHTML
This code is used to perform dangerous operations that might expose your sensitive data hence it is always a good idea to avoid unvalidated URLs and dangerouslySetInnerHTML. Install and configure a linter on your system to automatically detect unsafe activities.

Secure against DDOS attacks
DDoS attacks makes your application inaccessible to the potential traffic. Use DDoS protection service or tool and invest in robust network protocols for effective monitoring of network activities.

Keep the react version updated
Older versions are more likely to be attacked due to a lack of security functionality. Always keep your React version up to date. Use npm to update your React.


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

## Diretrizes para Ambiente de Produção

Toda solução implementada deve ser projetada desde o início considerando o ambiente de produção, não apenas o desenvolvimento local ou testes. Estas diretrizes são **OBRIGATÓRIAS**:

1. **Priorize soluções permanentes sobre correções temporárias**:
   - Evite implementações que funcionam apenas em ambiente de desenvolvimento
   - Identifique e corrija a causa raiz dos problemas, não apenas seus sintomas
   - Documente decisões técnicas que possam afetar o comportamento em produção

2. **Compatibilidade entre ambientes**:
   - Scripts e utilitários devem usar as mesmas bibliotecas e versões do código principal
   - Garanta que qualquer script de inicialização ou configuração produza dados no mesmo formato que a aplicação
   - Utilize as mesmas convenções e formatos em todos os ambientes (dev, teste, produção)

3. **Consistência e integridade dos dados**:
   - Todos os scripts de população ou migração de dados devem gerar dados válidos e consistentes
   - Implemente validações rigorosas para garantir a integridade dos dados
   - Utilize o mesmo formato de hash, criptografia e codificação em todos os ambientes

4. **Segurança em produção**:
   - Nunca codifique senhas ou chaves de API diretamente no código
   - Implemente rotação de segredos e gerenciamento de chaves
   - Configure corretamente firewalls, CORS e proteções contra ataques comuns
   - Implemente logging e monitoramento adequados para detecção de problemas

5. **Resiliência e recuperação**:
   - Projete para falhas, implementando retry policies, circuit breakers e fallbacks
   - Garanta que a aplicação possa ser reiniciada sem perda de dados
   - Implemente sistema de backup e restauração de dados

6. **Performance e escalabilidade**:
   - Otimize consultas de banco de dados para volumes de produção
   - Implemente caching apropriado para recursos frequentemente acessados
   - Considere a escalabilidade horizontal ao projetar novos componentes

7. **Observabilidade**:
   - Implemente logs estruturados com níveis apropriados
   - Configure alertas para condições críticas
   - Implemente métricas para monitorar performance e uso de recursos

Lembre-se: O que funciona em ambiente de desenvolvimento pode falhar em produção. Sempre projete, implemente e teste considerando o ambiente final de produção.