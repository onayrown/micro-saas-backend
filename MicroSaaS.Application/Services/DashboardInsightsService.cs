using MicroSaaS.Application.Interfaces.Repositories; // Usando interfaces da Application
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities; // Dependência do Domínio para Entidades
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Application.Services; // Namespace corrigido para Application

// Implementa APENAS a interface da Application
public class DashboardInsightsService : IDashboardInsightsService 
{
    // Dependências injetadas como interfaces da Application
    private readonly IDashboardInsightsRepository _repository;
    private readonly IContentPerformanceRepository _performanceRepository;
    private readonly IContentPostRepository _postRepository;

    public DashboardInsightsService(
        IDashboardInsightsRepository repository, 
        IContentPerformanceRepository performanceRepository, 
        IContentPostRepository postRepository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _performanceRepository = performanceRepository ?? throw new ArgumentNullException(nameof(performanceRepository));
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
    }

    // Métodos simples que delegam ao repositório
    public async Task<DashboardInsights> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<List<DashboardInsights>> GetByCreatorIdAsync(Guid creatorId)
    {
        var insights = await _repository.GetByCreatorIdAsync(creatorId);
        return insights.ToList();
    }

    public async Task<DashboardInsights> GetLatestByCreatorIdAsync(Guid creatorId)
    {
        return await _repository.GetLatestByCreatorIdAsync(creatorId);
    }

    // Método com lógica de aplicação para gerar insights
    public async Task<DashboardInsights> GenerateInsightsAsync(Guid creatorId)
    {
        var performances = await _performanceRepository.GetByCreatorIdAsync(creatorId);
        var posts = await _postRepository.GetByCreatorIdAsync(creatorId);

        var insights = new List<ContentInsight>();
        var recommendations = new List<ContentRecommendation>();

        // Gerar insights baseados no desempenho (Lógica de aplicação)
        foreach (var post in posts)
        {
            var performance = performances.FirstOrDefault(p => p.PostId == post.Id);
            if (performance != null)
            {
                // A lógica de cálculo e formatação do insight pertence aqui
                insights.Add(new ContentInsight
                {
                    Type = DetermineInsightType(performance), // Lógica de determinação do tipo
                    Title = $"Análise de Desempenho - {post.Title}",
                    Description = GenerateInsightDescription(performance), // Lógica de geração de texto
                    Metrics = ExtractRelevantMetrics(performance), // Lógica de extração
                    Trend = DetermineTrend(performance), // Lógica de tendência
                    ComparisonPeriod = "Últimos 7 dias", // Pode ser dinâmico
                    PercentageChange = (double)CalculatePercentageChange(performance) // Cast adicionado
                });
            }
        }

        // Gerar recomendações baseadas nos insights (Lógica de aplicação)
        recommendations = GenerateRecommendationsBasedOnInsights(insights, performances);

        var dashboardInsights = new DashboardInsights
        {
            Id = Guid.NewGuid(),
            CreatorId = creatorId,
            Date = DateTime.UtcNow,
            Insights = insights,
            Recommendations = recommendations,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Salva os insights gerados
        return await _repository.AddAsync(dashboardInsights);
    }

    // Métodos simples que delegam ao repositório
    public async Task<DashboardInsights> UpdateAsync(DashboardInsights insights)
    {
        // Adicionar validação ou lógica de negócio antes de atualizar, se necessário
        insights.UpdatedAt = DateTime.UtcNow;
        return await _repository.UpdateAsync(insights);
    }

    public async Task DeleteAsync(Guid id)
    {
        // Adicionar lógica de verificação de permissão ou regras de negócio, se necessário
        await _repository.DeleteAsync(id);
    }

    // --- Métodos Privados Auxiliares (Lógica de Aplicação) ---

    private InsightType DetermineInsightType(ContentPerformance performance)
    {
        // Exemplo: Lógica para determinar o tipo de insight
        var engagementRate = CalculateEngagementRate(performance);
        if (engagementRate > 10) return InsightType.HighEngagement;
        if (performance.Views > 10000) return InsightType.HighReach;
        return InsightType.Normal;
    }

    private string GenerateInsightDescription(ContentPerformance performance)
    {
        // Exemplo: Lógica para gerar descrição
        return $"O post teve {performance.Views} visualizações e {CalculateEngagementRate(performance):F1}% de engajamento.";
    }

    private Dictionary<string, double> ExtractRelevantMetrics(ContentPerformance performance)
    {
        // Exemplo: Lógica para extrair métricas
        return new Dictionary<string, double>
        {
            { "views", performance.Views },
            { "likes", performance.Likes },
            { "comments", performance.Comments },
            { "shares", performance.Shares },
            { "engagementRate", (double)CalculateEngagementRate(performance) }
        };
    }

     private TrendDirection DetermineTrend(ContentPerformance performance)
    {
        // Exemplo: Lógica placeholder para determinar tendência (comparar com período anterior?)
        return performance.EngagementRate > 5 ? TrendDirection.Up : TrendDirection.Stable;
    }

    private decimal CalculatePercentageChange(ContentPerformance performance)
    {
        // Exemplo: Lógica placeholder para calcular mudança percentual (comparar com período anterior?)
        return 0m; 
    }

    private decimal CalculateEngagementRate(ContentPerformance performance)
    {
        if (performance == null || performance.Views <= 0)
            return 0;
        var totalEngagements = performance.Likes + performance.Comments + performance.Shares;
        return ((decimal)totalEngagements / performance.Views) * 100;
    }

    private List<ContentRecommendation> GenerateRecommendationsBasedOnInsights(
        List<ContentInsight> insights, 
        IEnumerable<ContentPerformance> performances)
    {
        // Exemplo: Lógica placeholder mais elaborada para gerar recomendações
        var recommendations = new List<ContentRecommendation>();
        var avgEngagement = performances.Any() ? performances.Average(p => CalculateEngagementRate(p)) : 0;

        if (insights.Any(i => i.Type == InsightType.HighEngagement))
        {
            recommendations.Add(new ContentRecommendation { 
                RecommendationType = RecommendationType.ContentFormat, 
                Description = "Continue com o formato que gerou alto engajamento.",
                Priority = RecommendationPriority.High
            });
        }
        else if (avgEngagement < 3 && performances.Any())
        {
             recommendations.Add(new ContentRecommendation { 
                RecommendationType = RecommendationType.Strategy, 
                Description = "Experimente novos formatos ou tópicos para aumentar o engajamento médio.",
                Priority = RecommendationPriority.Medium
            });
        }

        // Adicionar recomendação genérica se nenhuma específica foi gerada
        if (!recommendations.Any())
        {
            recommendations.Add(new ContentRecommendation
            {
                RecommendationType = RecommendationType.Platform,
                Description = "Analise o desempenho por plataforma para otimizar sua estratégia.",
                Platform = SocialMediaPlatform.All,
                Priority = RecommendationPriority.Low
            });
        }

        return recommendations;
    }
} 