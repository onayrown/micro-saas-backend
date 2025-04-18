using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.DTOs; // Assumindo que ContentInsightsDto está aqui ou em outro DTO namespace
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroSaaS.Application.Services.ContentAnalysis.Analyzers
{
    public interface IPerformanceMetricsAnalyzer
    {
        void AnalyzePerformanceMetrics(IEnumerable<ContentPerformance> performances, ContentInsightsDto insights);
    }

    public class PerformanceMetricsAnalyzer : IPerformanceMetricsAnalyzer
    {
        private readonly ILoggingService _loggingService; // Injetar se necessário

        // Construtor pode receber dependências como ILoggingService
        public PerformanceMetricsAnalyzer(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public void AnalyzePerformanceMetrics(IEnumerable<ContentPerformance> performances, ContentInsightsDto insights)
        {
            if (insights == null || performances == null || !performances.Any())
            {
                // Log ou tratamento para parâmetros inválidos
                _loggingService?.LogWarning("AnalyzePerformanceMetrics chamado com parâmetros inválidos ou lista de performance vazia.");
                return;
            }

            double totalViews = 0;
            double totalLikes = 0;
            double totalComments = 0;
            double totalShares = 0;
            double totalRevenue = 0;

            // Inicializar dicionários se não existirem
            insights.PlatformPerformance ??= new Dictionary<SocialMediaPlatform, double>();
            insights.PerformanceFactors ??= new Dictionary<string, double>();

            foreach (var perf in performances)
            {
                // Calcular taxa de engajamento para cada plataforma
                double engagementRate = CalculateEngagementRate(perf);
                insights.PlatformPerformance[perf.Platform] = engagementRate;

                // Acumular métricas
                totalViews += perf.Views;
                totalLikes += perf.Likes;
                totalComments += perf.Comments;
                totalShares += perf.Shares;
                totalRevenue += (double)perf.EstimatedRevenue;

                // Analisar fatores específicos de performance
                insights.PerformanceFactors[$"Likes ({perf.Platform})"] = perf.Views > 0 ? (double)perf.Likes / perf.Views * 100 : 0;
                insights.PerformanceFactors[$"Comentários ({perf.Platform})"] = perf.Views > 0 ? (double)perf.Comments / perf.Views * 100 : 0;
                insights.PerformanceFactors[$"Compartilhamentos ({perf.Platform})"] = perf.Views > 0 ? (double)perf.Shares / perf.Views * 100 : 0;
            }

            // Calcular scores globais
            int platformCount = performances.Select(p => p.Platform).Distinct().Count();
            if (platformCount > 0 && totalViews > 0)
            {
                insights.EngagementScore = (totalLikes + totalComments + totalShares) / totalViews * 100; // Ajuste para percentual?
                insights.ReachScore = totalViews / platformCount;
                insights.ConversionScore = totalRevenue > 0 ? totalRevenue / totalViews * 100 : 0;
                insights.RetentionScore = CalculateRetentionScore(performances);
                insights.SentimentScore = CalculateSentimentScore(performances);
                insights.OverallScore = CalculateOverallScore(insights); // Usar método auxiliar
            }
            else
            {
                 // Definir scores padrão se não houver dados suficientes
                 insights.EngagementScore = 0;
                 insights.ReachScore = 0;
                 insights.ConversionScore = 0;
                 insights.RetentionScore = 0;
                 insights.SentimentScore = 0;
                 insights.OverallScore = 0;
            }
        }

        // --- Métodos Auxiliares Movidos --- 

        private double CalculateEngagementRate(ContentPerformance performance)
        {
            if (performance.Views == 0)
                return 0;

            var totalEngagements = performance.Likes + performance.Comments + performance.Shares;
            return (double)totalEngagements / performance.Views * 100;
        }

        private double CalculateRetentionScore(IEnumerable<ContentPerformance> performances)
        {
            // Lógica simulada - uma implementação real analisaria taxas de retorno de visualizadores,
            // duração média de visualização, etc.
            double avgLikesPerView = performances.Any(p => p.Views > 0) ? performances.Average(p => (double)p.Likes / Math.Max(1, p.Views)) : 0;
            return avgLikesPerView * 50; // Fator de escala arbitrário
        }

        private double CalculateSentimentScore(IEnumerable<ContentPerformance> performances)
        {
            // Lógica simulada - uma implementação real usaria NLP nos comentários.
            // Aqui, simulamos baseado na proporção de likes vs. outras interações.
            double totalEngagements = performances.Sum(p => p.Likes + p.Comments + p.Shares);
            if (totalEngagements == 0) return 0.5; // Neutro se não houver engajamento

            double likesRatio = performances.Sum(p => p.Likes) / totalEngagements;
            // Mapeia a proporção de likes para um score de sentimento (0 a 1)
            return Math.Clamp(likesRatio * 1.2, 0, 1.0); 
        }

        private double CalculateOverallScore(ContentInsightsDto insights)
        {
            // Fórmula de ponderação para o score geral
            return (insights.EngagementScore * 0.3 +
                    insights.ReachScore * 0.00002 + // Ajustar peso de ReachScore (que pode ser grande)
                    insights.ConversionScore * 0.15 +
                    insights.RetentionScore * 0.15 +
                    insights.SentimentScore * 10); // Ajustar peso do Sentimento (que é 0-1)
                    // Normalizar ou ajustar pesos conforme necessário
        }

        // Nota: O método CalculateEngagementRate que recebe IEnumerable<ContentPerformance> 
        // não parece ser usado diretamente por AnalyzePerformanceMetrics, talvez seja de outra parte?
        // Se for, pode ser movido para outra classe ou mantido no serviço principal por enquanto.

        // private double CalculateEngagementRate(IEnumerable<ContentPerformance> performances)
        // {
        //     double totalViews = performances.Sum(p => p.Views);
        //     if (totalViews == 0) return 0;
        //     double totalEngagements = performances.Sum(p => p.Likes + p.Comments + p.Shares);
        //     return totalEngagements / totalViews * 100;
        // }
    }
} 