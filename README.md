# Micro SaaS Backend

Este é o backend de um projeto Micro SaaS para gerenciar o conteúdo de criadores de conteúdo. O sistema foi desenvolvido utilizando **.NET 8.0** com **C#**, **MongoDB** como banco de dados e a arquitetura **Clean Architecture** para garantir flexibilidade e escalabilidade.

## Estado Atual do Projeto

O projeto está em desenvolvimento ativo com as principais funcionalidades já implementadas. Para ver o estado detalhado do projeto e o checklist de funcionalidades, consulte o arquivo [docs/projeto-status.md](docs/projeto-status.md).

### Destaques
- ✅ Estrutura de arquitetura limpa implementada
- ✅ Autenticação e autorização com JWT
- ✅ Integração com mídias sociais (Instagram, YouTube, TikTok)
- ✅ Gestão de conteúdo com agendamento
- ✅ Análise de desempenho de conteúdo
- ✅ Testes unitários e de integração

## Funcionalidades

- **Gestão de Usuários**: Registro, login e gerenciamento de perfis.
- **Gerenciamento de Criadores de Conteúdo**: Associação de usuários como criadores.
- **Gestão de Posts**: CRUD completo para posts de conteúdo com agendamento.
- **Integração com Redes Sociais**: Conexão com Instagram, YouTube e TikTok.
- **Análise de Desempenho**: Métricas e insights sobre o conteúdo publicado.
- **Listas de Verificação**: Verificações pré-publicação para garantir qualidade.
- **API RESTful**: Exposição de dados via REST APIs para fácil integração.

## Tecnologias Utilizadas

- **Backend**: .NET 8.0 com C#
- **Banco de Dados**: MongoDB
- **Arquitetura**: Clean Architecture
- **API**: ASP.NET Core Web API
- **Validação**: FluentValidation
- **Documentação de API**: Swagger
- **Autenticação**: JWT (JSON Web Tokens)
- **Testes**: xUnit, Moq, FluentAssertions
- **Logging**: Serilog

## Estrutura do Projeto

O projeto segue a arquitetura limpa (Clean Architecture) com as seguintes camadas:

- **MicroSaaS.Shared**: Enums, constantes e tipos compartilhados
- **MicroSaaS.Domain**: Entidades e interfaces de repositório
- **MicroSaaS.Application**: Interfaces de serviço, DTOs e validadores
- **MicroSaaS.Infrastructure**: Implementações dos repositórios e serviços
- **MicroSaaS.Backend**: Controllers e configurações da API
- **MicroSaaS.Tests**: Testes unitários
- **MicroSaaS.IntegrationTests**: Testes de integração

### Diretrizes de Arquitetura

Para garantir a consistência e manutenibilidade do código, seguimos diretrizes específicas para as dependências entre camadas. Consulte o documento [docs/arquitetura.md](docs/arquitetura.md) para entender:

- Estrutura de referências entre projetos
- Responsabilidades de cada camada
- Padrões de implementação a serem seguidos

**Importante**: Todos os desenvolvedores devem seguir estas diretrizes ao contribuir para o projeto.

## Como Rodar o Projeto

### Requisitos

- **.NET 8.0** ou superior
- **MongoDB** (instalado localmente ou usando uma instância na nuvem)
- **Git** (para clonar o repositório)

### Configuração

1. Clone o repositório:
   ```
   git clone https://github.com/seu-usuario/MicroSaaS.git
   cd MicroSaaS
   ```

2. Configure o MongoDB:
   - Defina as configurações de conexão no arquivo `appsettings.json` no projeto `MicroSaaS.Backend`

3. Restaure os pacotes NuGet:
   ```
   dotnet restore
   ```

4. Execute a aplicação:
   ```
   cd MicroSaaS.Backend
   dotnet run
   ```

5. Acesse a documentação da API:
   - Navegue para `https://localhost:5001/swagger` no seu navegador

## Executando os Testes

Para executar os testes unitários:
```
dotnet test MicroSaaS.Tests
```

Para executar os testes de integração:
```
dotnet test MicroSaaS.IntegrationTests
```

## Próximos Passos

Confira o arquivo [docs/projeto-status.md](docs/projeto-status.md) para ver a lista completa de funcionalidades pendentes e as prioridades de desenvolvimento.

## Contribuição

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues ou enviar pull requests.

## Licença

Este projeto está licenciado sob a licença MIT. Veja o arquivo LICENSE para mais detalhes.

