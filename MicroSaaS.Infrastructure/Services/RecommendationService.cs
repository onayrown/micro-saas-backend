using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.Services;

public class RecommendationService : IRecommendationService
{
    private readonly IContentPostRepository _contentRepository;
    private readonly IContentCreatorRepository _creatorRepository;
    private readonly ISocialMediaAccountRepository _socialMediaRepository;
    private readonly IPerformanceMetricsRepository _metricsRepository;
    private readonly IContentPerformanceRepository _contentPerformanceRepository;
    private readonly ISocialMediaIntegrationService _socialMediaService;

    public RecommendationService(
        IContentPostRepository contentRepository,
        IContentCreatorRepository creatorRepository,
        ISocialMediaAccountRepository socialMediaRepository,
        IPerformanceMetricsRepository metricsRepository,
        IContentPerformanceRepository contentPerformanceRepository,
        ISocialMediaIntegrationService socialMediaService)
    {
        _contentRepository = contentRepository;
        _creatorRepository = creatorRepository;
        _socialMediaRepository = socialMediaRepository;
        _metricsRepository = metricsRepository;
        _contentPerformanceRepository = contentPerformanceRepository;
        _socialMediaService = socialMediaService;
    }

    public virtual async Task<List<PostTimeRecommendation>> GetBestTimeToPostAsync(Guid creatorId, SocialMediaPlatform platform)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(creatorId));

        // Obter conta da plataforma específica
        var account = (await _socialMediaRepository.GetByCreatorIdAsync(creatorId))
            .FirstOrDefault(a => a.Platform == platform);

        if (account == null)
            throw new ArgumentException($"Conta da plataforma {platform} não encontrada", nameof(platform));

        // Buscar dados históricos de performance
        var performances = await _contentPerformanceRepository.GetByCreatorIdAsync(creatorId);
        // Filtrar por plataforma
        performances = performances.Where(p => p.Platform == platform).ToList();
        
        if (!performances.Any())
        {
            // Se não houver dados históricos suficientes, buscar recomendações da plataforma
            var platformRecommendations = await _socialMediaService.GetBestPostingTimesAsync(account.Id);
            
            // Converter para o tipo Domain
            return platformRecommendations.Select(r => new PostTimeRecommendation
            {
                DayOfWeek = r.DayOfWeek,
                TimeOfDay = r.TimeOfDay,
                EngagementScore = (double)r.EngagementScore
            }).ToList();
        }
        
        // Analisar quando os posts de melhor performance foram publicados
        var bestPerformances = performances
            .OrderByDescending(p => CalculateEngagementScore(p))
            .Take(10)
            .ToList();
            
        // Agrupar posts por dia da semana e hora do dia
        var timeSlots = new Dictionary<DayOfWeek, Dictionary<int, List<double>>>();
        
        foreach (var perf in bestPerformances)
        {
            var post = await _contentRepository.GetByIdAsync(perf.PostId);
            if (post == null) continue;
            
            var dayOfWeek = post.PublishedAt.HasValue ? post.PublishedAt.Value.DayOfWeek : post.CreatedAt.DayOfWeek;
            var hourOfDay = post.PublishedAt.HasValue ? post.PublishedAt.Value.Hour : post.CreatedAt.Hour;
            
            if (!timeSlots.ContainsKey(dayOfWeek))
            {
                timeSlots[dayOfWeek] = new Dictionary<int, List<double>>();
            }
            
            if (!timeSlots[dayOfWeek].ContainsKey(hourOfDay))
            {
                timeSlots[dayOfWeek][hourOfDay] = new List<double>();
            }
            
            timeSlots[dayOfWeek][hourOfDay].Add(CalculateEngagementScore(perf));
        }
        
        // Calcular média de engajamento para cada slot de tempo
        var recommendations = new List<PostTimeRecommendation>();
        foreach (var dayEntry in timeSlots)
        {
            var dayOfWeek = dayEntry.Key;
            var hourEntries = dayEntry.Value;
            
            foreach (var hourEntry in hourEntries)
            {
                var hourOfDay = hourEntry.Key;
                var scores = hourEntry.Value;
                
                var avgEngagement = scores.Count > 0 ? scores.Average() : 0;
                
                recommendations.Add(new PostTimeRecommendation
                {
                    DayOfWeek = dayOfWeek,
                    TimeOfDay = new TimeSpan(hourOfDay, 0, 0),
                    EngagementScore = avgEngagement
                });
            }
        }
        
        // Se não houver recomendações com base nos dados, gerar algumas padrão
        if (recommendations.Count == 0)
        {
            recommendations = GenerateDefaultRecommendations(platform);
        }
        
        return recommendations.OrderByDescending(r => r.EngagementScore).ToList();
    }

    public async Task<Dictionary<SocialMediaPlatform, List<PostTimeRecommendation>>> GetBestTimeToPostAllPlatformsAsync(Guid creatorId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(creatorId));
            
        var accounts = await _socialMediaRepository.GetByCreatorIdAsync(creatorId);
        var result = new Dictionary<SocialMediaPlatform, List<PostTimeRecommendation>>();
        
        foreach (var account in accounts)
        {
            var recommendations = await GetBestTimeToPostAsync(creatorId, account.Platform);
            result[account.Platform] = recommendations;
        }
        
        return result;
    }

    public async Task<List<ContentRecommendation>> GetContentRecommendationsAsync(Guid creatorId)
    {
        // Implementação básica para a primeira versão
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(creatorId));
            
        var recommendations = new List<ContentRecommendation>();
        
        // Adicionar recomendações de tópicos
        var topicRecommendations = await GetTopicRecommendationsAsync(creatorId);
        recommendations.AddRange(topicRecommendations);
        
        // Adicionar recomendações de formato
        var formatRecommendations = await GetFormatRecommendationsAsync(creatorId);
        recommendations.AddRange(formatRecommendations);
        
        return recommendations;
    }

    public async Task<List<ContentRecommendation>> GetTopicRecommendationsAsync(Guid creatorId)
    {
        // Implementação simplificada para a primeira versão
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(creatorId));
            
        // Na versão completa, esta função analisaria o conteúdo histórico do criador,
        // tendências atuais e feedback da audiência para recomendar tópicos
        
        return new List<ContentRecommendation>
        {
            new ContentRecommendation
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                Title = "Diversifique seus tópicos",
                Description = "Baseado na análise do seu conteúdo, recomendamos expandir para tópicos relacionados para aumentar o alcance.",
                Type = MicroSaaS.Shared.Enums.RecommendationType.ContentTopic,
                SuggestedTopics = new List<string> { "Dicas práticas", "Tutoriais rápidos", "Histórias de sucesso" },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Priority = RecommendationPriority.Low,
                PotentialImpact = "Aumento potencial de 20% no alcance"
            }
        };
    }

    public async Task<List<ContentRecommendation>> GetFormatRecommendationsAsync(Guid creatorId)
    {
        // Implementação simplificada para a primeira versão
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(creatorId));
            
        // Na versão completa, esta função analisaria quais formatos de conteúdo 
        // têm melhor performance para este criador específico
        
        return new List<ContentRecommendation>
        {
            new ContentRecommendation
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                Title = "Experimente vídeos curtos",
                Description = "Vídeos de 60-90 segundos estão gerando maior engajamento em sua niche. Considere adaptar parte do seu conteúdo para este formato.",
                Type = MicroSaaS.Shared.Enums.RecommendationType.ContentFormat,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Priority = RecommendationPriority.Medium,
                PotentialImpact = "Aumento potencial de 35% no engajamento"
            }
        };
    }

    // Implementação simplificada inicial
    public async Task<List<string>> GetHashtagRecommendationsAsync(Guid creatorId, string contentDescription, SocialMediaPlatform platform)
    {
        // Na versão completa, usaríamos NLP e dados de tendências para gerar hashtags relevantes
        switch (platform)
        {
            case SocialMediaPlatform.Instagram:
                return new List<string> { "#conteudodigital", "#marketingdigital", "#dicas", "#estrategiadigital" };
            case SocialMediaPlatform.YouTube:
                return new List<string> { "#tutorial", "#howto", "#dicas", "#aprenda" };
            case SocialMediaPlatform.TikTok:
                return new List<string> { "#fyp", "#paratipagina", "#viral", "#aprenda" };
            default:
                return new List<string> { "#conteudo", "#digital", "#dicas" };
        }
    }

    // Métodos auxiliares
    private double CalculateEngagementScore(ContentPerformance performance)
    {
        if (performance.Views == 0)
            return 0;
            
        var totalEngagements = performance.Likes + performance.Comments + performance.Shares;
        return (double)totalEngagements / performance.Views * 100;
    }

    private List<PostTimeRecommendation> GenerateDefaultRecommendations(SocialMediaPlatform platform)
    {
        var recommendations = new List<PostTimeRecommendation>();
        
        // Recomendações padrão baseadas em estudos gerais para cada plataforma
        switch (platform)
        {
            case SocialMediaPlatform.Instagram:
                recommendations.Add(new PostTimeRecommendation { DayOfWeek = DayOfWeek.Monday, TimeOfDay = new TimeSpan(12, 0, 0), EngagementScore = 7.5 });
                recommendations.Add(new PostTimeRecommendation { DayOfWeek = DayOfWeek.Wednesday, TimeOfDay = new TimeSpan(18, 0, 0), EngagementScore = 8.2 });
                recommendations.Add(new PostTimeRecommendation { DayOfWeek = DayOfWeek.Friday, TimeOfDay = new TimeSpan(17, 0, 0), EngagementScore = 8.7 });
                recommendations.Add(new PostTimeRecommendation { DayOfWeek = DayOfWeek.Saturday, TimeOfDay = new TimeSpan(11, 0, 0), EngagementScore = 7.8 });
                break;
                
            case SocialMediaPlatform.YouTube:
                recommendations.Add(new PostTimeRecommendation { DayOfWeek = DayOfWeek.Tuesday, TimeOfDay = new TimeSpan(16, 0, 0), EngagementScore = 8.0 });
                recommendations.Add(new PostTimeRecommendation { DayOfWeek = DayOfWeek.Thursday, TimeOfDay = new TimeSpan(17, 0, 0), EngagementScore = 8.3 });
                recommendations.Add(new PostTimeRecommendation { DayOfWeek = DayOfWeek.Saturday, TimeOfDay = new TimeSpan(10, 0, 0), EngagementScore = 8.9 });
                recommendations.Add(new PostTimeRecommendation { DayOfWeek = DayOfWeek.Sunday, TimeOfDay = new TimeSpan(15, 0, 0), EngagementScore = 8.5 });
                break;
                
            case SocialMediaPlatform.TikTok:
                recommendations.Add(new PostTimeRecommendation { DayOfWeek = DayOfWeek.Monday, TimeOfDay = new TimeSpan(20, 0, 0), EngagementScore = 8.1 });
                recommendations.Add(new PostTimeRecommendation { DayOfWeek = DayOfWeek.Wednesday, TimeOfDay = new TimeSpan(19, 0, 0), EngagementScore = 8.6 });
                recommendations.Add(new PostTimeRecommendation { DayOfWeek = DayOfWeek.Friday, TimeOfDay = new TimeSpan(22, 0, 0), EngagementScore = 9.2 });
                recommendations.Add(new PostTimeRecommendation { DayOfWeek = DayOfWeek.Sunday, TimeOfDay = new TimeSpan(18, 0, 0), EngagementScore = 8.4 });
                break;
                
            default:
                recommendations.Add(new PostTimeRecommendation { DayOfWeek = DayOfWeek.Monday, TimeOfDay = new TimeSpan(18, 0, 0), EngagementScore = 7.5 });
                recommendations.Add(new PostTimeRecommendation { DayOfWeek = DayOfWeek.Wednesday, TimeOfDay = new TimeSpan(12, 0, 0), EngagementScore = 8.0 });
                recommendations.Add(new PostTimeRecommendation { DayOfWeek = DayOfWeek.Friday, TimeOfDay = new TimeSpan(17, 0, 0), EngagementScore = 8.5 });
                break;
        }
        
        return recommendations;
    }

    // Restante da implementação será concluído na próxima etapa
    public async Task<List<TrendTopic>> GetTrendingTopicsAsync(SocialMediaPlatform platform)
    {
        // Em uma implementação real, isso consultaria APIs externas para obter tendências em tempo real
        // Para o MVP, usaremos uma implementação simplificada com dados simulados
        var trends = new List<TrendTopic>();
        
        switch (platform)
        {
            case SocialMediaPlatform.Instagram:
                trends.Add(CreateTrendTopic("Conteúdo Sustentável", "Criadores compartilhando ideias e práticas sustentáveis", 0.92, platform, 
                    new List<string> { "#sustentabilidade", "#ecofriendly", "#zerowaste" }));
                trends.Add(CreateTrendTopic("Reels Educativos", "Vídeos curtos com dicas educacionais e tutoriais", 0.89, platform, 
                    new List<string> { "#aprenda", "#dicasinstagram", "#tutorial" }));
                trends.Add(CreateTrendTopic("Behind the Scenes", "Conteúdo mostrando os bastidores da criação", 0.85, platform, 
                    new List<string> { "#behindthescenes", "#bastidores", "#bts" }));
                break;
                
            case SocialMediaPlatform.YouTube:
                trends.Add(CreateTrendTopic("Explicações em 3 minutos", "Vídeos curtos explicando conceitos complexos", 0.91, platform, 
                    new List<string> { "#explicação", "#resumo", "#3minutos" }));
                trends.Add(CreateTrendTopic("Day in the Life", "Um dia na vida de profissionais de diversas áreas", 0.88, platform, 
                    new List<string> { "#dayinthelife", "#rotina", "#lifestyle" }));
                trends.Add(CreateTrendTopic("Reviews Honestos", "Análises sinceras e detalhadas de produtos", 0.83, platform, 
                    new List<string> { "#review", "#análise", "#opinião" }));
                break;
                
            case SocialMediaPlatform.TikTok:
                trends.Add(CreateTrendTopic("Transitions Criativos", "Transições únicas e criativas entre vídeos", 0.94, platform, 
                    new List<string> { "#transition", "#fyp", "#criativo" }));
                trends.Add(CreateTrendTopic("Dicas Rápidas", "Conselhos rápidos e práticos em 15 segundos", 0.92, platform, 
                    new List<string> { "#dica", "#hack", "#aprenda" }));
                trends.Add(CreateTrendTopic("Storytelling", "Narrativas pessoais e histórias interessantes", 0.87, platform, 
                    new List<string> { "#storytelling", "#história", "#storytime" }));
                break;
                
            default:
                trends.Add(CreateTrendTopic("Conteúdo Autêntico", "Conteúdo mais pessoal e menos produzido", 0.88, platform, 
                    new List<string> { "#autentico", "#pessoal", "#real" }));
                break;
        }
        
        return trends;
    }

    public async Task<List<TrendTopic>> GetNicheTrendingTopicsAsync(Guid creatorId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(creatorId));
            
        // Em uma implementação real, analisaríamos o nicho do criador e cruzaríamos com dados
        // de tendências específicas desse segmento
        
        // Para o MVP, usaremos informações básicas do criador para sugerir tendências relevantes
        var trendingTopics = new List<TrendTopic>();
        var niche = !string.IsNullOrEmpty(creator.Niche) ? creator.Niche : "Digital";
        
        // Adicionar algumas tendências relacionadas ao nicho do criador
        switch (niche.ToLower())
        {
            case "tecnologia":
            case "tech":
                trendingTopics.Add(CreateTrendTopic("Novidades em IA", "Avanços e aplicações práticas de inteligência artificial", 0.93, SocialMediaPlatform.YouTube, 
                    new List<string> { "#IA", "#tech", "#futuro" }));
                trendingTopics.Add(CreateTrendTopic("Dicas de Produtividade", "Ferramentas e métodos para aumentar a produtividade", 0.89, SocialMediaPlatform.Instagram, 
                    new List<string> { "#produtividade", "#ferramentas", "#workflow" }));
                break;
                
            case "saúde":
            case "fitness":
            case "bem-estar":
                trendingTopics.Add(CreateTrendTopic("Rotinas Matinais", "Hábitos saudáveis para começar o dia", 0.92, SocialMediaPlatform.Instagram, 
                    new List<string> { "#rotina", "#saúde", "#manhã" }));
                trendingTopics.Add(CreateTrendTopic("Receitas Saudáveis", "Preparos rápidos e nutritivos", 0.87, SocialMediaPlatform.TikTok, 
                    new List<string> { "#receita", "#saudável", "#nutrição" }));
                break;
                
            case "marketing":
            case "negócios":
            case "empreendedorismo":
                trendingTopics.Add(CreateTrendTopic("Case Studies", "Estudos de caso detalhados de empresas de sucesso", 0.91, SocialMediaPlatform.LinkedIn, 
                    new List<string> { "#casestudy", "#business", "#estratégia" }));
                trendingTopics.Add(CreateTrendTopic("Marketing Ético", "Estratégias de marketing com responsabilidade social", 0.88, SocialMediaPlatform.Instagram, 
                    new List<string> { "#marketingético", "#negócios", "#impactosocial" }));
                break;
                
            default:
                trendingTopics.Add(CreateTrendTopic("Conteúdo Personalizado", "Como criar conteúdo mais autêntico e pessoal", 0.90, SocialMediaPlatform.Instagram, 
                    new List<string> { "#autenticidade", "#conteúdo", "#personalização" }));
                trendingTopics.Add(CreateTrendTopic("Storytelling Digital", "Como contar histórias cativantes online", 0.85, SocialMediaPlatform.TikTok, 
                    new List<string> { "#storytelling", "#narrativa", "#conteúdo" }));
                break;
        }
        
        // Adicionar tendências gerais relevantes para todos os criadores
        var generalTrends = await GetTrendingTopicsAsync(GetPrimaryPlatform(creatorId));
        trendingTopics.AddRange(generalTrends.Take(1)); // Adicionar apenas a principal tendência geral
        
        return trendingTopics;
    }

    public async Task<List<ContentRecommendation>> GetMonetizationRecommendationsAsync(Guid creatorId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(creatorId));
            
        var recommendations = new List<ContentRecommendation>();
        
        // Verificar se o criador já tem integrações monetizáveis
        var hasAdSense = creator.AdSenseSettings != null && creator.AdSenseSettings.IsConnected;
        var accounts = await _socialMediaRepository.GetByCreatorIdAsync(creatorId);
        var platformsCount = accounts.Count();
        
        // Recomendar diversificação de fontes de receita
        if (!hasAdSense)
        {
            recommendations.Add(new ContentRecommendation
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                Title = "Conecte Google AdSense",
                Description = "Integre o Google AdSense para monetizar seu conteúdo com anúncios relevantes.",
                Type = MicroSaaS.Shared.Enums.RecommendationType.MonetizationStrategy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Priority = RecommendationPriority.Low,
                PotentialImpact = "Nova fonte de receita com potencial de crescimento"
            });
        }
        
        // Recomendar integração com mais plataformas
        if (platformsCount < 3)
        {
            recommendations.Add(new ContentRecommendation
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                Title = "Diversifique suas plataformas",
                Description = "Estar presente em mais plataformas aumenta seu alcance e oportunidades de monetização.",
                Type = MicroSaaS.Shared.Enums.RecommendationType.CrossPlatformPromotion,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Priority = RecommendationPriority.Medium,
                PotentialImpact = "Aumento potencial de 40% na receita com múltiplas plataformas"
            });
        }
        
        // Recomendar formatos específicos para monetização
        recommendations.Add(new ContentRecommendation
        {
            Id = Guid.NewGuid(),
            CreatorId = creatorId,
            Title = "Conteúdo premium exclusivo",
            Description = "Considere criar conteúdo exclusivo para uma audiência disposta a pagar por acesso privilegiado.",
            Type = MicroSaaS.Shared.Enums.RecommendationType.MonetizationStrategy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Priority = RecommendationPriority.High,
            PotentialImpact = "Nova fonte de receita recorrente e previsível"
        });
        
        return recommendations;
    }

    public async Task<List<ContentRecommendation>> GetAudienceGrowthRecommendationsAsync(Guid creatorId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(creatorId));
            
        var recommendations = new List<ContentRecommendation>();
        
        // Analisar o crescimento atual para fazer recomendações
        var accounts = await _socialMediaRepository.GetByCreatorIdAsync(creatorId);
        var platformMetrics = new Dictionary<SocialMediaPlatform, (int Followers, decimal Growth)>();
        
        foreach (var account in accounts)
        {
            var followers = account.FollowersCount;
            var growth = 0M; // Crescimento mensal em %
            
            try
            {
                // Calcular crescimento dos últimos 30 dias, se houver dados
                var endDate = DateTime.UtcNow;
                var startDate = endDate.AddDays(-30);
                growth = await CalculateFollowerGrowthPercent(creatorId, account.Platform, startDate, endDate);
            }
            catch
            {
                // Sem dados suficientes para calcular crescimento
                growth = 0;
            }
            
            platformMetrics[account.Platform] = (followers, growth);
        }
        
        // Recomendar estratégias baseadas nas métricas
        foreach (var platform in platformMetrics.Keys)
        {
            var (followers, growth) = platformMetrics[platform];
            
            if (growth < 5) // Crescimento mensal menor que 5%
            {
                recommendations.Add(new ContentRecommendation
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = $"Acelere o crescimento no {platform}",
                    Description = $"Sua taxa de crescimento no {platform} está abaixo da média. Considere aumentar a frequência de postagens e interações.",
                    Type = MicroSaaS.Shared.Enums.RecommendationType.AudienceTargeting,
                    Platform = platform,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Priority = RecommendationPriority.Low,
                    PotentialImpact = "Potencial para dobrar a taxa de crescimento de seguidores"
                });
            }
            
            if (followers < 1000) // Audiência pequena
            {
                recommendations.Add(new ContentRecommendation
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = "Construa sua base inicial",
                    Description = "Foque em nichos específicos e construa uma comunidade engajada antes de diversificar muito seu conteúdo.",
                    Type = MicroSaaS.Shared.Enums.RecommendationType.AudienceTargeting,
                    Platform = platform,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Priority = RecommendationPriority.Medium,
                    PotentialImpact = "Crescimento sustentável da base de seguidores"
                });
            }
        }
        
        // Recomendações gerais para crescimento de audiência
        recommendations.Add(new ContentRecommendation
        {
            Id = Guid.NewGuid(),
            CreatorId = creatorId,
            Title = "Colaborações estratégicas",
            Description = "Identifique criadores com audiência semelhante mas não concorrente e proponha colaborações.",
            Type = MicroSaaS.Shared.Enums.RecommendationType.CollaborationOpportunity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Priority = RecommendationPriority.High,
            PotentialImpact = "Exposição a novas audiências e crescimento acelerado"
        });
        
        return recommendations;
    }

    public async Task<List<ContentRecommendation>> GetEngagementImprovementRecommendationsAsync(Guid creatorId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(creatorId));
            
        var recommendations = new List<ContentRecommendation>();
        
        // Analisar os posts recentes e seu engajamento
        var posts = await _contentRepository.GetByCreatorIdAsync(creatorId);
        var recentPosts = posts.OrderByDescending(p => p.CreatedAt).Take(10).ToList();
        
        var totalEngagement = 0.0;
        var engagementRates = new List<double>();
        
        foreach (var post in recentPosts)
        {
            var performances = await _contentPerformanceRepository.GetByPostIdAsync(post.Id.ToString());
            
            foreach (var perf in performances)
            {
                var rate = CalculateEngagementScore(perf);
                engagementRates.Add(rate);
                totalEngagement += rate;
            }
        }
        
        var avgEngagement = engagementRates.Count > 0 ? totalEngagement / engagementRates.Count : 0;
        
        // Fazer recomendações baseadas no engajamento médio
        if (avgEngagement < 3.0) // Engajamento baixo (< 3%)
        {
            recommendations.Add(new ContentRecommendation
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                Title = "Aumente a interatividade",
                Description = "Seu engajamento está abaixo da média. Experimente fazer perguntas, enquetes, e conteúdo que solicite diretamente a participação da audiência.",
                Type = MicroSaaS.Shared.Enums.RecommendationType.EngagementTactic,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Priority = RecommendationPriority.Low,
                PotentialImpact = "Aumento potencial de 200% na taxa de engajamento"
            });
            
            recommendations.Add(new ContentRecommendation
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                Title = "Responda a comentários",
                Description = "Dedique tempo para responder comentários e interagir com sua audiência. Isso estimula mais interações e fideliza seguidores.",
                Type = MicroSaaS.Shared.Enums.RecommendationType.EngagementTactic,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Priority = RecommendationPriority.Medium,
                PotentialImpact = "Maior fidelização e aumento gradual do engajamento"
            });
        }
        else if (avgEngagement < 6.0) // Engajamento médio (3-6%)
        {
            recommendations.Add(new ContentRecommendation
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                Title = "Crie conteúdo seriado",
                Description = "Seu engajamento é razoável. Experimente criar séries de conteúdo que mantenham sua audiência voltando para ver a continuação.",
                Type = MicroSaaS.Shared.Enums.RecommendationType.ContentFormat,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Priority = RecommendationPriority.Medium,
                PotentialImpact = "Aumento de 50% na retenção de audiência"
            });
        }
        
        // Recomendar melhores horários de postagem
        recommendations.Add(new ContentRecommendation
        {
            Id = Guid.NewGuid(),
            CreatorId = creatorId,
            Title = "Otimize seus horários de postagem",
            Description = "Poste consistentemente nos horários de maior engajamento para sua audiência.",
            Type = MicroSaaS.Shared.Enums.RecommendationType.PostingTime,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Priority = RecommendationPriority.High,
            PotentialImpact = "Aumento de 30% no alcance inicial das postagens"
        });
        
        return recommendations;
    }

    public async Task<ContentAnalysis> AnalyzeContentPerformanceAsync(Guid contentId)
    {
        var post = await _contentRepository.GetByIdAsync(contentId);
        if (post == null)
            throw new ArgumentException("Conteúdo não encontrado", nameof(contentId));
            
        var performances = await _contentPerformanceRepository.GetByPostIdAsync(contentId.ToString());
        
        if (!performances.Any())
            throw new Exception("Não há dados de performance para este conteúdo");
            
        var analysis = new ContentAnalysis
        {
            ContentId = contentId,
            Title = post.Title,
            PerformanceFactors = new Dictionary<string, double>(),
            StrengthPoints = new List<string>(),
            ImprovementSuggestions = new List<string>(),
            PlatformPerformance = new Dictionary<SocialMediaPlatform, double>()
        };
        
        double totalEngagement = 0;
        double totalReach = 0;
        int platformCount = 0;
        
        foreach (var perf in performances)
        {
            var engagementRate = CalculateEngagementScore(perf);
            totalEngagement += engagementRate;
            totalReach += perf.Views;
            platformCount++;
            
            analysis.PlatformPerformance[perf.Platform] = engagementRate;
            
            // Analisar fatores de desempenho
            if (perf.Likes > 0)
                analysis.PerformanceFactors["Likes"] = (double)perf.Likes / perf.Views * 100;
                
            if (perf.Comments > 0)
                analysis.PerformanceFactors["Comentários"] = (double)perf.Comments / perf.Views * 100;
                
            if (perf.Shares > 0)
                analysis.PerformanceFactors["Compartilhamentos"] = (double)perf.Shares / perf.Views * 100;
        }
        
        // Calcular médias
        if (platformCount > 0)
        {
            analysis.EngagementScore = totalEngagement / platformCount;
            analysis.ReachScore = totalReach / platformCount;
            
            // Se tivermos alguma estimativa de conversão, usamos isso. Caso contrário, aproximamos
            analysis.ConversionScore = performances.Any(p => p.EstimatedRevenue > 0) 
                ? (double)performances.Average(p => p.EstimatedRevenue / p.Views * 100)
                : analysis.EngagementScore * 0.2; // Aproximação simples
        }
        
        // Identificar pontos fortes
        if (analysis.EngagementScore > 5.0)
            analysis.StrengthPoints.Add("Alto engajamento da audiência");
            
        if (analysis.ReachScore > 1000)
            analysis.StrengthPoints.Add("Bom alcance de visualizações");
            
        if (analysis.PerformanceFactors.ContainsKey("Comentários") && analysis.PerformanceFactors["Comentários"] > 1.0)
            analysis.StrengthPoints.Add("Forte geração de comentários");
            
        if (analysis.PerformanceFactors.ContainsKey("Compartilhamentos") && analysis.PerformanceFactors["Compartilhamentos"] > 2.0)
            analysis.StrengthPoints.Add("Alto índice de compartilhamentos");
        
        // Sugestões de melhoria
        if (analysis.EngagementScore < 3.0)
            analysis.ImprovementSuggestions.Add("Adicione chamadas para ação para aumentar o engajamento");
            
        if (!analysis.PerformanceFactors.ContainsKey("Comentários") || analysis.PerformanceFactors["Comentários"] < 0.5)
            analysis.ImprovementSuggestions.Add("Faça perguntas ou solicite a opinião da audiência para aumentar comentários");
            
        if (!analysis.PerformanceFactors.ContainsKey("Compartilhamentos") || analysis.PerformanceFactors["Compartilhamentos"] < 1.0)
            analysis.ImprovementSuggestions.Add("Crie conteúdo mais compartilhável, como dicas úteis ou informações surpreendentes");
            
        return analysis;
    }

    public async Task RefreshRecommendationsAsync(Guid creatorId)
    {
        // Essa função seria usada para atualizar recomendações em segundo plano
        // ou para forçar uma atualização imediata das recomendações
        
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(creatorId));
            
        // Para o MVP, simplesmente limpamos o cache interno em implementações futuras
        // e retornamos as recomendações atualizadas
        
        // Na implementação completa, chamaríamos as APIs externas para obter dados frescos,
        // analisaríamos tendências atuais e atualizaríamos as recomendações no banco de dados
        
        await Task.CompletedTask; // Placeholder para implementação futura
    }

    // Métodos auxiliares adicionais
    private TrendTopic CreateTrendTopic(string name, string description, double score, SocialMediaPlatform platform, List<string> hashtags)
    {
        return new TrendTopic
        {
            Name = name,
            Description = description,
            PopularityScore = score,
            DiscoveredAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(0, 5)),
            RelatedHashtags = hashtags,
            Platform = platform
        };
    }
    
    private SocialMediaPlatform GetPrimaryPlatform(Guid creatorId)
    {
        // Em uma implementação real, analisaríamos a plataforma com maior engajamento
        // Para o MVP, retornaremos uma plataforma aleatória ou a primeira disponível
        var random = new Random();
        var values = Enum.GetValues(typeof(SocialMediaPlatform));
        
        if (values.Length > 0)
        {
            int index = random.Next(values.Length);
            if (values.GetValue(index) is SocialMediaPlatform platform)
            {
                return platform;
            }
        }
        
        // Valor padrão caso não consiga obter um valor válido
        return SocialMediaPlatform.Instagram;
    }

    private async Task<decimal> CalculateFollowerGrowthPercent(Guid creatorId, SocialMediaPlatform platform, DateTime startDate, DateTime endDate)
    {
        var metrics = await _metricsRepository.GetByCreatorIdAsync(creatorId);
        // Filtrar por plataforma
        metrics = metrics.Where(m => m.Platform == platform).ToList();
        
        var startMetrics = metrics
            .Where(m => m.Date.Date <= startDate.Date)
            .OrderByDescending(m => m.Date)
            .FirstOrDefault();
            
        var endMetrics = metrics
            .Where(m => m.Date.Date <= endDate.Date)
            .OrderByDescending(m => m.Date)
            .FirstOrDefault();
            
        if (startMetrics == null || endMetrics == null || startMetrics.Followers == 0)
            return 0;
            
        return ((decimal)(endMetrics.Followers - startMetrics.Followers) / startMetrics.Followers) * 100;
    }

    // Novos métodos implementados para cumprir a interface
    public async Task<List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>> GetBestPostingTimesAsync(Guid creatorId, SocialMediaPlatform platform)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(creatorId));

        // Obter recomendações através do método existente
        var timeRecommendations = await GetBestTimeToPostAsync(creatorId, platform);
        
        // Converter de PostTimeRecommendation para BestTimeSlotDto
        return timeRecommendations.Select(r => new MicroSaaS.Shared.DTOs.BestTimeSlotDto
        {
            Id = Guid.NewGuid(),
            CreatorId = creatorId,
            Platform = platform,
            DayOfWeek = r.DayOfWeek,
            TimeOfDay = r.TimeOfDay,
            Hour = r.TimeOfDay.Hours,
            EngagementScore = (decimal)r.EngagementScore,
            ConfidenceScore = (decimal)(r.EngagementScore / 10.0),
            EngagementPotential = (int)(r.EngagementScore * 10),
            RecommendationStrength = r.EngagementScore > 7.0 ? "Alto" : 
                                    r.EngagementScore > 4.0 ? "Médio" : "Baixo",
            CreatedAt = DateTime.UtcNow
        }).ToList();
    }

    public async Task<List<ContentRecommendationDto>> GetContentRecommendationsAsync(Guid creatorId, SocialMediaPlatform platform)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(creatorId));

        // Implementação de exemplo - na versão real faria análise dos dados de performance
        var result = new List<ContentRecommendationDto>
        {
            new ContentRecommendationDto
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                Title = "Diversifique seus tópicos",
                Description = "Baseado na análise do seu conteúdo, recomendamos expandir para tópicos relacionados para aumentar o alcance.",
                Type = MicroSaaS.Shared.DTOs.RecommendationType.Topic,
                Platform = platform,
                ConfidenceScore = 0.85m,
                ExampleContentIds = new List<string> { "post123", "post456" },
                SuggestedHashtags = new List<string> { "#dicas", "#tutorial" },
                SuggestedKeywords = new List<string> { "como fazer", "passo a passo" },
                CreatedAt = DateTime.UtcNow
            },
            new ContentRecommendationDto
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                Title = "Experimente vídeos curtos",
                Description = "Vídeos de 60-90 segundos estão gerando maior engajamento em sua niche.",
                Type = MicroSaaS.Shared.DTOs.RecommendationType.Format,
                Platform = platform,
                ConfidenceScore = 0.9m,
                ExampleContentIds = new List<string> { "post789", "post012" },
                SuggestedHashtags = new List<string> { "#shorts", "#viral" },
                SuggestedKeywords = new List<string> { "rápido", "simples" },
                CreatedAt = DateTime.UtcNow
            }
        };

        return result;
    }

    public async Task<List<GrowthRecommendationDto>> GetGrowthRecommendationsAsync(Guid creatorId, SocialMediaPlatform platform)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(creatorId));

        // Implementação de exemplo - versão real analisaria dados de crescimento
        var result = new List<GrowthRecommendationDto>
        {
            new GrowthRecommendationDto
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                Title = "Aumente sua consistência",
                Description = "Publicar conteúdo em horários regulares aumenta o engajamento.",
                Category = MicroSaaS.Shared.DTOs.GrowthCategory.Engagement,
                Platform = platform,
                ImplementationSteps = new List<string> 
                { 
                    "Estabeleça um calendário semanal de publicações",
                    "Mantenha ao menos 3 publicações por semana",
                    "Priorize qualidade sobre quantidade"
                },
                ExpectedOutcome = "Aumento de 30% na retenção de audiência",
                Difficulty = 2,
                TimeToImplement = "1-2 semanas",
                CreatedAt = DateTime.UtcNow
            },
            new GrowthRecommendationDto
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                Title = "Interaja mais com sua audiência",
                Description = "Responder comentários aumenta a fidelidade dos seguidores.",
                Category = MicroSaaS.Shared.DTOs.GrowthCategory.Followers,
                Platform = platform,
                ImplementationSteps = new List<string> 
                { 
                    "Reserve 30 minutos diários para responder comentários",
                    "Faça perguntas que incentivem interação",
                    "Agradeça publicamente participações relevantes"
                },
                ExpectedOutcome = "Aumento de 45% na taxa de comentários",
                Difficulty = 1,
                TimeToImplement = "Imediato",
                CreatedAt = DateTime.UtcNow
            }
        };

        return result;
    }

    public async Task<ContentRecommendationDto> GenerateCustomRecommendationAsync(CustomRecommendationRequestDto request)
    {
        var creator = await _creatorRepository.GetByIdAsync(request.CreatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(request.CreatorId));

        // Implementação de exemplo - versão real usaria dados históricos e ML
        var recommendation = new ContentRecommendationDto
        {
            Id = Guid.NewGuid(),
            CreatorId = request.CreatorId,
            Title = $"Recomendação personalizada: {request.RecommendationType}",
            Description = $"Recomendação baseada em seus critérios específicos para {request.Platform}.",
            Type = request.RecommendationType,
            Platform = request.Platform,
            ConfidenceScore = 0.85m,
            ExampleContentIds = new List<string> { "post123", "post456" },
            SuggestedHashtags = new List<string> { "#personalizado", "#recomendado" },
            SuggestedKeywords = new List<string> { request.SpecificTopic ?? "conteúdo", request.ContentGoal ?? "engajamento" },
            CreatedAt = DateTime.UtcNow
        };

        return recommendation;
    }

    public async Task<MicroSaaS.Shared.DTOs.AudienceSensitivityDto> GetAudienceSensitivityAnalysisAsync(Guid creatorId, SocialMediaPlatform platform)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(creatorId));

        // Implementação simplificada para primeira versão
        // Em uma versão completa, analisaríamos histórico de postagens e reações para definir a sensibilidade
        
        var posts = await _contentRepository.GetByCreatorIdAsync(creatorId);
        var recentPosts = posts.Where(p => p.PublishedAt.HasValue)
                              .OrderByDescending(p => p.PublishedAt)
                              .Take(20).ToList();
                              
        var performances = new List<ContentPerformance>();
        foreach (var post in recentPosts)
        {
            var postPerformances = await _contentPerformanceRepository.GetByPostIdAsync(post.Id.ToString());
            performances.AddRange(postPerformances.Where(p => p.Platform == platform));
        }
        
        // Análise baseada em dados
        var sensitivityTopics = new Dictionary<string, int>
        {
            { "Política", 0 },
            { "Temas sociais", 0 },
            { "Religião", 0 },
            { "Saúde", 0 },
            { "Finanças", 0 },
            { "Controvérsias", 0 }
        };
        
        // Em uma implementação real, faríamos análise de conteúdo e feedback
        // Aqui estamos gerando resultados simulados baseados no ID do criador
        var creatorIdValue = creatorId.GetHashCode();
        var random = new Random(creatorIdValue);
        
        foreach (var topic in sensitivityTopics.Keys.ToList())
        {
            sensitivityTopics[topic] = random.Next(1, 10);
        }
        
        var result = new MicroSaaS.Shared.DTOs.AudienceSensitivityDto
        {
            CreatorId = creatorId,
            Platform = platform.ToString(),
            TopResponsiveTopics = new List<string> { "tutorial", "dicas", "novidades" },
            TopResponsiveFormats = new List<string> { "vídeo curto", "carrossel", "guia passo-a-passo" },
            BestTimeOfDay = new List<TimeSpan> { new TimeSpan(18, 0, 0), new TimeSpan(21, 0, 0) },
            BestDaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Friday },
            ConfidenceScore = 0.8m,
            LastUpdated = DateTime.UtcNow,
            SensitivityTopics = sensitivityTopics.Select(kvp => new MicroSaaS.Shared.DTOs.TopicSensitivityDto 
            {
                Topic = kvp.Key,
                SensitivityLevel = kvp.Value,
                RecommendedApproach = GetRecommendedApproach(kvp.Value)
            }).ToList(),
            OverallSensitivity = sensitivityTopics.Values.Average(),
            Analysis = "Análise baseada nas últimas " + performances.Count + " publicações na plataforma " + platform,
            RecommendedContentApproach = GetGeneralContentApproach(sensitivityTopics)
        };

        return result;
    }
    
    private string GetRecommendedApproach(int sensitivityLevel)
    {
        if (sensitivityLevel >= 8)
            return "Evitar o tópico ou tratar com extrema cautela";
        else if (sensitivityLevel >= 5)
            return "Abordar de maneira neutra e educativa";
        else
            return "Pode abordar normalmente com sensibilidade regular";
    }
    
    private string GetGeneralContentApproach(Dictionary<string, int> sensitivities)
    {
        var avgSensitivity = sensitivities.Values.Average();
        
        if (avgSensitivity > 7)
            return "Sua audiência é bastante sensível a tópicos controversos. Recomendamos foco em conteúdo positivo e educativo.";
        else if (avgSensitivity > 4)
            return "Sua audiência tem sensibilidade moderada. Balance conteúdo neutro com alguns tópicos mais envolventes.";
        else
            return "Sua audiência é receptiva a diversos tópicos. Você tem flexibilidade para explorar conteúdo variado.";
    }
} 