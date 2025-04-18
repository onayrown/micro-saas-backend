# Tasks e Prioridades

## Prioridades Imediatas (Alta)

1. **[x] Resolver Erros de Compilação em MockRecommendationService.cs:**
    - [x] Resolver conflito de tipos RecommendationType:
        - [x] Analisar e alinhar tipos entre MicroSaaS.Shared.Enums.RecommendationType e MicroSaaS.Shared.DTOs.RecommendationType
        - [x] Implementar conversão adequada entre os tipos
    - [x] Corrigir propriedade Score ausente em ContentRecommendationDto:
        - [x] Verificar modelo correto do DTO
        - [x] Atualizar referências para usar propriedade correta
    - [x] Implementar RecommendationDto ausente:
        - [x] Criar DTO em MicroSaaS.Shared.DTOs
        - [x] Atualizar referências no MockRecommendationService

2. **[x] Corrigir Violações de Arquitetura:**
    - [x] ContentChecklistController: Remover uso direto de entidades de domínio
        - [x] Criar DTOs apropriados em MicroSaaS.Application/DTOs/Checklist
        - [x] Implementar mapeamento entre entidades e DTOs
        - [x] Atualizar controller para usar apenas DTOs
        - [x] Atualizar testes relacionados

3. **[ ] Planejar Refatoração do ContentAnalysisService:**
    - [ ] Criar documento de estratégia de refatoração
    - [ ] Identificar responsabilidades e definir novas classes
    - [ ] Estabelecer ordem de refatoração
    - [ ] Definir testes necessários

## Concluídas
- [x] Análise dos erros de compilação
- [x] Implementação das correções em MockRecommendationService.cs
- [x] Correção das violações de arquitetura no ContentChecklistController
- [x] Atualização dos testes relacionados ao ContentChecklistController
- [x] Resolução de ambiguidades de classes e DTOs:
  - [x] Resolução da ambiguidade entre ContentPerformanceDto e ContentPerformanceSummaryDto
  - [x] Resolução da ambiguidade entre os enums RecommendationType
  - [x] Resolução da ambiguidade entre PostTimeRecommendation e BestTimeSlotDto

## Em Andamento
- [x] Verificar e corrigir vulnerabilidades de pacotes NuGet
  - [x] Identificar pacotes com vulnerabilidades
  - [x] Atualizar para versões estáveis e LTS compatíveis com .NET 8
  - [x] Remover pacotes em versões preview
- [x] Identificar e resolver ambiguidades de arquivos/classes
  - [x] Verificar classes duplicadas em toda a solution
  - [x] Remover ou consolidar arquivos não utilizados
  - [x] Garantir conformidade com a arquitetura definida em planning.md
- [ ] Planejar Refatoração do ContentAnalysisService

## Observações
- Todas as alterações devem seguir estritamente a Clean Architecture definida em planning.md
- Manter este arquivo atualizado com progresso das tasks
- Documentar decisões importantes em /docs
