# Problemas Encontrados e Soluções

## 1. Problema: Interfaces não encontradas no DashboardService

### Descrição
O serviço `DashboardService.cs` estava utilizando várias interfaces de repositórios sem as devidas referências de namespace, causando erros de compilação:

- `CS0246: The type or namespace name 'IPerformanceMetricsRepository' could not be found`
- `CS0246: The type or namespace name 'IContentPostRepository' could not be found`
- `CS0246: The type or namespace name 'IContentPerformanceRepository' could not be found`

### Solução
1. Adicionada a diretiva using para o namespace que contém as interfaces de repositórios:
   ```csharp
   using MicroSaaS.Application.Interfaces.Repositories;
   ```

2. As interfaces estão definidas corretamente nos seguintes arquivos:
   - `MicroSaaS.Application/Interfaces/Repositories/IPerformanceMetricsRepository.cs`
   - `MicroSaaS.Application/Interfaces/Repositories/IContentPostRepository.cs`
   - `MicroSaaS.Application/Interfaces/Repositories/IContentPerformanceRepository.cs`

3. Estrutura de namespaces está de acordo com a arquitetura em camadas definida em `arquitetura.md`.

### Lições Aprendidas
1. Sempre verificar se os namespaces corretos estão sendo importados quando trabalhando com interfaces.
2. As interfaces dos repositórios devem estar no namespace `MicroSaaS.Application.Interfaces.Repositories` conforme a arquitetura definida.
3. Manter consistência na estrutura de namespaces facilita a resolução de problemas de referência.

## 2. Problema: Referências ambíguas para interfaces de repositório

### Descrição
Após adicionar as diretivas using necessárias, outro problema surgiu - referências ambíguas para as interfaces:

```
'IContentPerformanceRepository' is an ambiguous reference between 'MicroSaaS.Domain.Interfaces.IContentPerformanceRepository' and 'MicroSaaS.Application.Interfaces.Repositories.IContentPerformanceRepository'
'IContentPostRepository' is an ambiguous reference between 'MicroSaaS.Domain.Interfaces.IContentPostRepository' and 'MicroSaaS.Application.Interfaces.Repositories.IContentPostRepository'
```

Isso ocorreu porque as mesmas interfaces estão definidas em dois namespaces diferentes:
- `MicroSaaS.Domain.Interfaces`
- `MicroSaaS.Application.Interfaces.Repositories`

### Solução
Para resolver o problema de ambiguidade, utilizamos nomes de namespace completos (fully qualified names) para as interfaces:

```csharp
private readonly MicroSaaS.Application.Interfaces.Repositories.IPerformanceMetricsRepository _metricsRepository;
private readonly MicroSaaS.Application.Interfaces.Repositories.IContentPostRepository _contentRepository;
private readonly MicroSaaS.Application.Interfaces.Repositories.IContentPerformanceRepository _contentPerformanceRepository;
```

E no construtor:

```csharp
public DashboardService(
    IDashboardInsightsService insightsService,
    MicroSaaS.Application.Interfaces.Repositories.IPerformanceMetricsRepository metricsRepository,
    MicroSaaS.Application.Interfaces.Repositories.IContentPostRepository contentRepository,
    MicroSaaS.Application.Interfaces.Repositories.IContentPerformanceRepository contentPerformanceRepository,
    IRecommendationService recommendationService)
{
    // ...
}
```

### Lições Aprendidas
1. Interfaces com o mesmo nome não devem existir em namespaces diferentes, pois causam ambiguidade.
2. Quando necessário resolver ambiguidades, usar nomes de namespace completos (fully qualified names).
3. Considerar uma refatoração da arquitetura para evitar definições duplicadas de interfaces.

## 3. Arquitetura de Serviços e Repositórios

A arquitetura do projeto segue o padrão de camadas:

1. **Entidades de Domínio**: `MicroSaaS.Domain.Entities`
2. **Interfaces de Aplicação**: `MicroSaaS.Application.Interfaces`
   - Repositórios: `MicroSaaS.Application.Interfaces.Repositories`
   - Serviços: `MicroSaaS.Application.Interfaces.Services`
3. **Implementação de Infraestrutura**: 
   - Repositórios: `MicroSaaS.Infrastructure.Repositories`
   - Serviços: `MicroSaaS.Infrastructure.Services`

Esta arquitetura facilita:
- Testes unitários com mocks de repositórios
- Separação clara de responsabilidades
- Desacoplamento entre camadas
- Implementação de diferentes provedores de persistência

### Problema Identificado na Arquitetura
Existem interfaces de repositório duplicadas nos namespaces `MicroSaaS.Domain.Interfaces` e `MicroSaaS.Application.Interfaces.Repositories`. Isso causa ambiguidade e dificulta a manutenção do código.

### Recomendação
1. Remover a duplicação mantendo apenas uma definição de cada interface, preferencialmente em `MicroSaaS.Application.Interfaces.Repositories`.
2. Atualizar todas as referências para usar a interface no namespace escolhido.
3. Se necessário, manter adaptadores entre as interfaces do domínio e as interfaces da aplicação.

## 4. Problema: Propriedade incorreta na classe ContentPerformance

### Descrição
Erro de compilação no arquivo `DashboardService.cs` na linha 87:

```
CS1061: 'ContentPerformance' does not contain a definition for 'ContentId' and no accessible extension method 'ContentId' accepting a first argument of type 'ContentPerformance' could be found (are you missing a using directive or an assembly reference?)
```

O código estava tentando acessar uma propriedade `ContentId` na classe `ContentPerformance`, mas essa propriedade não existe.

### Solução
Após analisar a definição da classe `ContentPerformance`, identificamos que a propriedade correta é `PostId` em vez de `ContentId`. Alteramos a linha de código:

```csharp
// De:
var postPerfs = performances.Where(p => p.ContentId == post.Id).ToList();

// Para:
var postPerfs = performances.Where(p => p.PostId == post.Id).ToList();
```

### Lições Aprendidas
1. Verificar cuidadosamente os nomes das propriedades das entidades do domínio.
2. Manter consistência na nomenclatura entre diferentes entidades relacionadas.
3. Considerar usar constantes ou expressões lambda com membros ao usar propriedades para evitar erros de digitação.
   ```csharp
   // Exemplo com expressão lambda
   Expression<Func<ContentPerformance, bool>> hasMatchingPostId = p => p.PostId == post.Id;
   var postPerfs = performances.Where(hasMatchingPostId.Compile()).ToList();
   ``` 

## 5. Problema: Erro na validação de token no DashboardController

### Descrição
Erro de compilação no arquivo `DashboardController.cs` na linha 35:

```
CS1061: 'bool' does not contain a definition for 'IsValid' and no accessible extension method 'IsValid' accepting a first argument of type 'bool' could be found (are you missing a using directive or an assembly reference?)
```

O código estava tentando acessar uma propriedade `IsValid` do retorno do método `ValidateToken`, mas esse método retorna um `bool` e não um objeto com uma propriedade `IsValid`.

### Solução
Modificamos o código para usar diretamente o valor booleano retornado pelo método `ValidateToken`:

```csharp
// De:
var tokenData = _tokenService.ValidateToken(token);
if (!tokenData.IsValid)
{
    return Forbid();
}

// Para:
var isValidToken = _tokenService.ValidateToken(token);
if (!isValidToken)
{
    return Forbid();
}
```

### Lições Aprendidas
1. Verificar a assinatura dos métodos nas interfaces antes de usá-los.
2. Garantir que a implementação e o uso de um serviço sejam consistentes.
3. Em caso de dúvida sobre o retorno de um método, consultar a definição da interface.
4. Evitar suposições sobre a estrutura dos objetos retornados pelos métodos. 

## 6. Problema: Incompatibilidade entre classes de teste e definições de entidades no DashboardTests

### Descrição
Diversos erros de compilação nos testes de integração, principalmente em `MicroSaaS.IntegrationTests/DashboardTests.cs` e `MicroSaaS.IntegrationTests/Utils/TestDashboardController.cs`:

1. Referências a propriedades que não existem nas entidades:
   - `ContentPerformance` não contém propriedades como `ContentId` e `ImpressionCount`
   - `PerformanceMetrics` não contém propriedades como `Views`, `Likes`, `Comments`
   - `ContentPost` não contém propriedades como `Description` e `ContentUrl`

2. Referências a valores de enumeração que não existem:
   - `InsightType.Positive`, `InsightType.Weekly` e `InsightType.Custom` não existem
   - `RecommendationType.ContentStrategy` e `RecommendationType.PlatformSpecific` não existem
   - `ContentType.Image` não existe

3. Erros de conversão de tipos:
   - Conversão implícita de `int` para `RecommendationPriority`
   - Comparação entre `double` e `decimal`

### Solução
1. Criamos a classe `UserTokenData` em `MicroSaaS.IntegrationTests/Models/UserTokenData.cs` para simular o retorno do serviço de token:
   ```csharp
   public class UserTokenData
   {
       public Guid UserId { get; set; }
       public string Role { get; set; }
       public bool IsValid { get; set; }
   }
   ```

2. Corrigimos as propriedades nas entidades de teste para corresponder às definições reais:
   - Em `ContentPerformance`, trocamos `ContentId` por `PostId`
   - Em `PerformanceMetrics`, usamos `TotalViews`, `TotalLikes`, etc. em vez de `Views`, `Likes`, etc.
   - Em `ContentPost`, usamos `Content` e `MediaUrl` em vez de `Description` e `ContentUrl`

3. Atualizamos os valores de enumeração para corresponder às definições reais:
   - `InsightType.Positive` → `InsightType.Performance` e `InsightType.HighEngagement`
   - `InsightType.Weekly` → `InsightType.Normal`
   - `RecommendationType.ContentStrategy` → `RecommendationType.PostingFrequency`

4. Corrigimos os erros de conversão de tipos:
   - Atribuições de números inteiros a `RecommendationPriority` foram substituídas pelos valores de enumeração
   - Adicionamos o sufixo `m` para garantir comparações entre valores `decimal`

### Lições Aprendidas
1. Manter sincronizadas as definições de entidades entre o código principal e os testes
2. Verificar as propriedades e valores de enumeração nas definições originais antes de usá-los nos testes
3. Criar classes de suporte específicas para testes quando necessário
4. Especificar explicitamente os tipos em comparações e conversões para evitar erros de tipo 

## 7. Problema: Implementação incompleta da interface IDashboardService

### Descrição
Erros de compilação no arquivo `Mocks.cs` relacionados à classe `MockDashboardService`:

```
Error (active) CS0535: 'MockDashboardService' does not implement interface member 'IDashboardService.AddContentPerformanceAsync(ContentPerformance)'
Error (active) CS0535: 'MockDashboardService' does not implement interface member 'IDashboardService.AddMetricsAsync(PerformanceMetrics)'
```

Vários outros erros similares foram encontrados. A classe `MockDashboardService` estava implementando apenas um método da interface `IDashboardService` (`GetRecommendationsAsync`), mas a interface define vários outros métodos que precisam ser implementados:

1. `GetLatestInsightsAsync(Guid creatorId)`
2. `GenerateInsightsAsync(Guid creatorId, DateTime? startDate, DateTime? endDate)`
3. `GetMetricsAsync(Guid creatorId, DateTime? startDate, DateTime? endDate, SocialMediaPlatform? platform)`
4. `GetDailyMetricsAsync(Guid creatorId, DateTime? date, SocialMediaPlatform platform)`
5. `GetTopContentAsync(Guid creatorId, int limit)`
6. `GetBestTimeToPostAsync(Guid creatorId, SocialMediaPlatform platform)`
7. `GetAverageEngagementRateAsync(Guid creatorId, SocialMediaPlatform platform)`
8. `GetRevenueGrowthAsync(Guid creatorId, DateTime? startDate, DateTime? endDate)`
9. `GetFollowerGrowthAsync(Guid creatorId, SocialMediaPlatform platform, DateTime? startDate, DateTime? endDate)`
10. `AddMetricsAsync(PerformanceMetrics metrics)`
11. `AddContentPerformanceAsync(ContentPerformance performance)`

### Solução
1. Implementamos todos os métodos faltantes na classe `MockDashboardService`
2. Adicionamos dados de exemplo para cada tipo de entidade para uso nos testes:
   - Lista de `DashboardInsights`
   - Lista de `PerformanceMetrics`
   - Lista de `ContentPost`
   - Lista de `ContentPerformance`
   - Lista de `PostTimeRecommendation`

3. Criamos um construtor que inicializa esses dados com valores realistas para testes
4. Cada método implementado segue o padrão de retornar dados de teste fixos ou filtrar os dados existentes

### Exemplo de implementação do método `GetLatestInsightsAsync`:
```csharp
public Task<DashboardInsights> GetLatestInsightsAsync(Guid creatorId)
{
    var insight = _insights.FirstOrDefault(i => i.CreatorId == creatorId);
    if (insight == null)
    {
        // Criar insights de exemplo se não existirem
        return GenerateInsightsAsync(creatorId);
    }
    
    return Task.FromResult(insight);
}
```

### Lições Aprendidas
1. Ao implementar uma interface, é essencial verificar todos os métodos definidos e implementá-los corretamente
2. Classes mock para testes devem fornecer dados de exemplo consistentes e realistas
3. Recomenda-se validar a implementação completa de interfaces periodicamente, especialmente após adicionar novos métodos
4. O uso de ferramentas de análise de código e compilação frequente ajuda a identificar implementações incompletas 

## 8. Problema: Referência ambígua a PostTimeRecommendation e incompatibilidade de tipos

### Descrição
Erros de compilação relacionados à classe `PostTimeRecommendation` na implementação de `MockDashboardService`:

```
Error (active) CS0104: 'PostTimeRecommendation' is an ambiguous reference between 'MicroSaaS.Domain.Entities.PostTimeRecommendation' and 'MicroSaaS.Shared.Models.PostTimeRecommendation'
Error (active) CS0738: 'MockDashboardService' does not implement interface member 'IDashboardService.GetBestTimeToPostAsync(Guid, SocialMediaPlatform)'. 'MockDashboardService.GetBestTimeToPostAsync(Guid, SocialMediaPlatform)' cannot implement 'IDashboardService.GetBestTimeToPostAsync(Guid, SocialMediaPlatform)' because it does not have the matching return type of 'Task<List<PostTimeRecommendation>>'.
```

O problema ocorreu porque existem duas classes com o mesmo nome (`PostTimeRecommendation`) em namespaces diferentes:
1. `MicroSaaS.Domain.Entities.PostTimeRecommendation` - Usado na interface `IDashboardService`
2. `MicroSaaS.Shared.Models.PostTimeRecommendation` - Outra definição similar mas com tipos diferentes

Além disso, os tipos tinham diferenças importantes:
- Em `MicroSaaS.Domain.Entities.PostTimeRecommendation`, a propriedade `EngagementScore` é do tipo `double`
- Em `MicroSaaS.Shared.Models.PostTimeRecommendation`, a propriedade `EngagementScore` é do tipo `decimal`
- A versão em `Shared.Models` também contém uma propriedade `Id` que não existe na versão do domínio

### Solução
1. Utilizamos o nome completo da classe (`MicroSaaS.Domain.Entities.PostTimeRecommendation`) em todas as referências para resolver a ambiguidade:

```csharp
private readonly List<MicroSaaS.Domain.Entities.PostTimeRecommendation> _postTimeRecommendations = 
    new List<MicroSaaS.Domain.Entities.PostTimeRecommendation>();
```

2. Modificamos a assinatura do método `GetBestTimeToPostAsync` para retornar explicitamente o tipo correto:

```csharp
public Task<List<MicroSaaS.Domain.Entities.PostTimeRecommendation>> GetBestTimeToPostAsync(Guid creatorId, SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
{
    // Retorna as recomendações pré-definidas
    return Task.FromResult(_postTimeRecommendations);
}
```

3. Atualizamos as instâncias de `PostTimeRecommendation` para usar a versão correta com o tipo correto para `EngagementScore` (double em vez de decimal):

```csharp
_postTimeRecommendations.Add(new MicroSaaS.Domain.Entities.PostTimeRecommendation
{
    DayOfWeek = DayOfWeek.Monday,
    TimeOfDay = new TimeSpan(18, 0, 0),
    EngagementScore = 8.5 // Sem o sufixo 'm' pois é double, não decimal
});
```

### Lições Aprendidas
1. Quando existem tipos com o mesmo nome em namespaces diferentes, é crucial usar nomes qualificados (fully qualified names) para evitar ambiguidade
2. É importante verificar os tipos exatos das propriedades para garantir compatibilidade (double vs decimal)
3. A duplicação de tipos com o mesmo nome em namespaces diferentes deve ser evitada na arquitetura
4. Considerar a criação de mapeadores ou adaptadores entre versões de DTOs similares para manter a coesão 

## 9. Problema: Referências inválidas a valores de enumeração e ambiguidade em RecommendationType

### Descrição
Múltiplos erros de compilação relacionados a enumerações no projeto de testes de integração:

1. Referências inválidas a valores de `InsightType` que não existem:
```
Error (active) CS0117: 'InsightType' does not contain a definition for 'Custom'
Error (active) CS0117: 'InsightType' does not contain a definition for 'Opportunity'
Error (active) CS0117: 'InsightType' does not contain a definition for 'Positive'
```

2. Referências inválidas a valores de `RecommendationType` que não existem:
```
Error (active) CS0117: 'RecommendationType' does not contain a definition for 'ContentStrategy'
Error (active) CS0117: 'RecommendationType' does not contain a definition for 'PlatformSpecific'
```

3. Ambiguidade na referência a `RecommendationType`:
```
Error (active) CS0104: 'RecommendationType' is an ambiguous reference between 'MicroSaaS.Shared.DTOs.RecommendationType' and 'MicroSaaS.Shared.Enums.RecommendationType'
```

4. Problemas de propriedades inexistentes em `PerformanceMetrics`:
```
Error (active) CS0117: 'PerformanceMetrics' does not contain a definition for 'Comments'
Error (active) CS0117: 'PerformanceMetrics' does not contain a definition for 'Views'
```

5. Erros de conversão e comparação entre `double` e `decimal`:
```
Error (active) CS0266: Cannot implicitly convert type 'double' to 'decimal'
Error (active) CS0019: Operator '<=' cannot be applied to operands of type 'double' and 'decimal'
```

### Solução

1. **Para valores inexistentes de InsightType**:
   - Substituímos `InsightType.Positive` por `InsightType.HighEngagement` ou `InsightType.Performance` conforme adequado
   - Substituímos `InsightType.Custom` por `InsightType.Normal`
   - Substituímos `InsightType.Opportunity` por `InsightType.Trend`

2. **Para valores inexistentes de RecommendationType**:
   - Substituímos `RecommendationType.ContentStrategy` por `MicroSaaS.Shared.Enums.RecommendationType.PostingFrequency`
   - Substituímos `RecommendationType.PlatformSpecific` por `MicroSaaS.Shared.Enums.RecommendationType.Platform`

3. **Para resolver a ambiguidade em RecommendationType**:
   - Utilizamos o nome completamente qualificado `MicroSaaS.Shared.Enums.RecommendationType` em todas as referências

4. **Para propriedades inexistentes em PerformanceMetrics**:
   - Substituímos `Views` por `TotalViews`
   - Substituímos `Likes` por `TotalLikes`
   - Substituímos `Comments` por `TotalComments`
   - Substituímos `Shares` por `TotalShares`
   - Removemos propriedades inexistentes como `ImpressionCount`, `ProfileVisits` e `ReachCount`
   - Adicionamos propriedades existentes como `EstimatedRevenue` e `TopPerformingContentIds`

5. **Para incompatibilidade entre double e decimal**:
   - Em `GetBestTimeToPost_ReturnsSortedRecommendations`, mudamos o tipo da variável `lastScore` de `decimal?` para `double?`
   - Removemos a conversão usando `Convert.ToDecimal` e mantivemos o tipo original `double` para comparações

### Lições Aprendidas
1. Sempre verificar os valores reais das enumerações antes de usá-los em testes
2. Quando existem tipos com o mesmo nome em namespaces diferentes, usar nomes qualificados (fully qualified names) para evitar ambiguidade
3. Manter alinhamento entre os tipos de dados em classes de modelo e nos testes (double vs decimal)
4. Verificar a definição real das entidades ao criar objetos para testes
5. Evitar criar enumerações diferentes com o mesmo nome em namespaces distintos (como `RecommendationType`) 