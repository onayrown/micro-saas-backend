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

3. **[x] Planejar Refatoração do ContentAnalysisService:**
    - [x] Criar documento de estratégia de refatoração
    - [x] Identificar responsabilidades e definir novas classes
    - [x] Estabelecer ordem de refatoração
    - [x] Definir testes necessários

4. **[ ] Implementar Refatoração do ContentAnalysisService (Abordagem Incremental):**
    - [ ] Fase 1: Preparação e Infraestrutura
      - [ ] Criar interfaces base para novos serviços e analisadores
      - [ ] Extrair classes utilitárias comuns (EngagementCalculator, etc.)
      - [ ] Configurar estrutura de pastas e namespaces
      - [ ] Garantir que o projeto compila sem erros

    - [ ] Fase 2: Refatoração do ContentInsightsService (Primeiro Serviço)
      - [ ] Criar interfaces para analisadores relacionados (ViralPotentialAnalyzer, etc.)
      - [ ] Implementar analisadores básicos com testes unitários
      - [ ] Implementar ContentInsightsService com testes unitários
      - [ ] Integrar com ContentAnalysisService (mantendo interface pública)
      - [ ] Verificar compilação e executar todos os testes

    - [ ] Fase 3: Refatoração do AudienceAnalysisService (Segundo Serviço)
      - [ ] Criar interfaces para analisadores relacionados à audiência
      - [ ] Implementar analisadores com testes unitários
      - [ ] Implementar AudienceAnalysisService com testes unitários
      - [ ] Integrar com ContentAnalysisService
      - [ ] Verificar compilação e executar todos os testes

    - [ ] Fase 4: Refatoração do ContentPredictionService (Terceiro Serviço)
      - [ ] Implementar analisadores e serviço com testes
      - [ ] Integrar com ContentAnalysisService
      - [ ] Verificar compilação e executar todos os testes

    - [ ] Fase 5: Refatoração do PerformancePatternAnalysisService (Quarto Serviço)
      - [ ] Implementar analisadores e serviço com testes
      - [ ] Integrar com ContentAnalysisService
      - [ ] Verificar compilação e executar todos os testes

    - [ ] Fase 6: Finalização e Limpeza
      - [ ] Transformar ContentAnalysisService em fachada completa
      - [ ] Remover código legado e duplicado
      - [ ] Atualizar documentação
      - [ ] Verificar cobertura de código
      - [ ] Executar testes de integração completos

## Concluídas
- [x] Análise dos erros de compilação
- [x] Implementação das correções em MockRecommendationService.cs
- [x] Correção das violações de arquitetura no ContentChecklistController
- [x] Atualização dos testes relacionados ao ContentChecklistController
- [x] Resolução de ambiguidades de classes e DTOs:
  - [x] Resolução da ambiguidade entre ContentPerformanceDto e ContentPerformanceSummaryDto
  - [x] Resolução da ambiguidade entre os enums RecommendationType
  - [x] Resolução da ambiguidade entre PostTimeRecommendation e BestTimeSlotDto
- [x] Planejamento da refatoração do ContentAnalysisService:
  - [x] Análise do estado atual do serviço
  - [x] Identificação de problemas e oportunidades de melhoria
  - [x] Definição da estratégia de refatoração
  - [x] Criação do plano de implementação em fases

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
