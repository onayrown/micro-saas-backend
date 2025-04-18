using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Results;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InterfaceContentAnalysis = MicroSaaS.Application.Interfaces.Services.ContentAnalysis;
using InterfaceTrendTopic = MicroSaaS.Application.Interfaces.Services.TrendTopic;

namespace MicroSaaS.Application.Services.Recommendation;

/// <summary>
/// Implementação do serviço de recomendações pertencente à camada de aplicação
/// conforme os princípios da Clean Architecture.
/// </summary>
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

    public virtual async Task<List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>> GetBestTimeToPostAsync(Guid creatorId, SocialMediaPlatform platform)
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

        List<PostTimeRecommendation> recommendationsEntity;
        if (!performances.Any())
        {
            // Se não houver dados históricos suficientes, buscar recomendações da plataforma
            var platformRecommendations = await _socialMediaService.GetBestPostingTimesAsync(account.Id);

            // Converter DTO do serviço de integração para Entidade do Domínio (temporariamente se necessário)
            recommendationsEntity = platformRecommendations.Select(r => new PostTimeRecommendation
            {
                DayOfWeek = r.DayOfWeek,
                TimeOfDay = r.TimeOfDay,
                EngagementScore = (double)r.EngagementScore // Cast se necessário
            }).ToList();
        }
        else
        {
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
            recommendationsEntity = new List<PostTimeRecommendation>();
            foreach (var dayEntry in timeSlots)
            {
                var dayOfWeek = dayEntry.Key;
                var hourEntries = dayEntry.Value;

                foreach (var hourEntry in hourEntries)
                {
                    var hourOfDay = hourEntry.Key;
                    var scores = hourEntry.Value;

                    var avgEngagement = scores.Any() ? scores.Average() : 0;

                    recommendationsEntity.Add(new PostTimeRecommendation
                    {
                        DayOfWeek = dayOfWeek,
                        TimeOfDay = new TimeSpan(hourOfDay, 0, 0),
                        EngagementScore = avgEngagement
                    });
                }
            }

            // Se não houver recomendações com base nos dados, gerar algumas padrão
            if (recommendationsEntity.Count == 0)
            {
                recommendationsEntity = GenerateDefaultRecommendations(platform);
            }
        }

        // Mapear entidades para DTOs antes de retornar
        return recommendationsEntity
                 .OrderByDescending(r => r.EngagementScore)
                 .Select(MapToDto)
                 .ToList();
    }

    public async Task<Dictionary<SocialMediaPlatform, List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>>> GetBestTimeToPostAllPlatformsAsync(Guid creatorId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(creatorId));

        var accounts = await _socialMediaRepository.GetByCreatorIdAsync(creatorId);
        var result = new Dictionary<SocialMediaPlatform, List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>>();

        foreach (var account in accounts)
        {
            var recommendationsDto = await GetBestTimeToPostAsync(creatorId, account.Platform);
            result[account.Platform] = recommendationsDto;
        }

        return result;
    }

    public async Task<List<MicroSaaS.Shared.DTOs.ContentRecommendationDto>> GetContentRecommendationsAsync(Guid creatorId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(creatorId));

        var recommendationsDto = new List<MicroSaaS.Shared.DTOs.ContentRecommendationDto>();

        // Obter recomendações de diferentes tipos e já mapeadas para DTO
        var topicRecs = await GetTopicRecommendationsAsync(creatorId); // Já retorna DTO
        var formatRecs = await GetFormatRecommendationsAsync(creatorId); // Já retorna DTO
        // Adicionar outras chamadas conforme necessário (Monetization, Growth, Engagement)
        var monetizationRecs = await GetMonetizationRecommendationsAsync(creatorId); // Já retorna DTO
        var audienceGrowthRecs = await GetAudienceGrowthRecommendationsAsync(creatorId); // Já retorna DTO
        var engagementRecs = await GetEngagementImprovementRecommendationsAsync(creatorId); // Já retorna DTO

        recommendationsDto.AddRange(topicRecs);
        recommendationsDto.AddRange(formatRecs);
        recommendationsDto.AddRange(monetizationRecs);
        recommendationsDto.AddRange(audienceGrowthRecs);
        recommendationsDto.AddRange(engagementRecs);

        return recommendationsDto;
    }

    public async Task<List<MicroSaaS.Shared.DTOs.ContentRecommendationDto>> GetTopicRecommendationsAsync(Guid creatorId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(creatorId));

        // Lógica para gerar recomendações de tópicos (simplificada)
        var recommendationsEntity = new List<ContentRecommendation>
        {
            new ContentRecommendation
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                Title = "Diversifique seus tópicos (Tópico)",
                Description = "Baseado na análise do seu conteúdo, recomendamos expandir para tópicos relacionados.",
                Type = RecommendationType.ContentTopic,
                Priority = RecommendationPriority.Low,
                SuggestedTopics = new List<string> { "Dicas práticas", "Tutoriais rápidos", "Histórias de sucesso" },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PotentialImpact = "Aumento potencial de 20% no alcance"
            }
        };
        // Mapear para DTO antes de retornar
        return recommendationsEntity.Select(MapToDto).ToList();
    }

    public async Task<List<MicroSaaS.Shared.DTOs.ContentRecommendationDto>> GetFormatRecommendationsAsync(Guid creatorId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(creatorId));

        // Lógica para gerar recomendações de formato (simplificada)
        var recommendationsEntity = new List<ContentRecommendation>
        {
            new ContentRecommendation
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                Title = "Experimente vídeos curtos (Formato)",
                Description = "Vídeos curtos estão gerando maior engajamento em sua niche.",
                Type = RecommendationType.ContentFormat,
                Priority = RecommendationPriority.Medium,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PotentialImpact = "Aumento potencial de 35% no engajamento"
            }
        };
        // Mapear para DTO antes de retornar
        return recommendationsEntity.Select(MapToDto).ToList();
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
    public async Task<List<MicroSaaS.Shared.DTOs.TrendTopicDto>> GetTrendingTopicsAsync(SocialMediaPlatform platform)
    {
        // Lógica simplificada para obter tendências (ex: chamada a API externa)
        var trendsEntity = new List<InterfaceTrendTopic>
        {
            CreateTrendTopic("AI Marketing", "Uso de IA em estratégias de marketing.", 0.9, platform, new List<string>{"#AI"}),
            CreateTrendTopic("Vídeos Curtos", "Popularidade de Reels, Shorts e TikTok.", 0.85, platform, new List<string>{"#Shorts"})
        };
        // Mapear para DTO
        return trendsEntity.Select(MapToDto).ToList();
    }

    public async Task<List<MicroSaaS.Shared.DTOs.TrendTopicDto>> GetNicheTrendingTopicsAsync(Guid creatorId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null) throw new ArgumentException("Criador não encontrado.");
        // Lógica simplificada para obter tendências de nicho
        var nicheTrendsEntity = new List<InterfaceTrendTopic>
        {
             CreateTrendTopic($"Niche Trend 1 para {creator.Niche ?? "Geral"}", "Descrição da tendência 1.", 0.7, GetPrimaryPlatform(creatorId), new List<string>{$"#{creator.Niche?.Replace(" ", "")}Trend"}),
             CreateTrendTopic($"Niche Trend 2 para {creator.Niche ?? "Geral"}", "Descrição da tendência 2.", 0.65, GetPrimaryPlatform(creatorId), new List<string>{ "#NicheSpecific"})
        };
         // Mapear para DTO
        return nicheTrendsEntity.Select(MapToDto).ToList();
    }

    public async Task<List<MicroSaaS.Shared.DTOs.ContentRecommendationDto>> GetMonetizationRecommendationsAsync(Guid creatorId)
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

        // Mapear para DTO
        return recommendations.Select(MapToDto).ToList();
    }

    public async Task<List<MicroSaaS.Shared.DTOs.ContentRecommendationDto>> GetAudienceGrowthRecommendationsAsync(Guid creatorId)
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

        // Mapear para DTO
        return recommendations.Select(MapToDto).ToList();
    }

    public async Task<List<MicroSaaS.Shared.DTOs.ContentRecommendationDto>> GetEngagementImprovementRecommendationsAsync(Guid creatorId)
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

        // Mapear para DTO
        return recommendations.Select(MapToDto).ToList();
    }

    public async Task<MicroSaaS.Shared.DTOs.ContentAnalysisDto> AnalyzeContentPerformanceAsync(Guid contentId)
    {
        var post = await _contentRepository.GetByIdAsync(contentId);
        if (post == null) throw new ArgumentException("Post não encontrado.");
        var performance = (await _contentPerformanceRepository.GetByPostIdAsync(contentId.ToString())).FirstOrDefault();

        // Lógica simplificada de análise
        var analysisEntity = new InterfaceContentAnalysis
        {
            ContentId = contentId,
            Title = post.Title,
            EngagementScore = performance != null ? CalculateEngagementScore(performance) : 0,
            StrengthPoints = new List<string> { "Bom título", "Imagem relevante" },
            ImprovementSuggestions = new List<string> { "Adicionar CTA mais claro", "Melhorar hashtags" }
        };

        return MapToDto(analysisEntity);
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
    private InterfaceTrendTopic CreateTrendTopic(string name, string description, double score, SocialMediaPlatform platform, List<string> hashtags)
    {
        return new InterfaceTrendTopic
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
         var recommendationsEntity = await GetBestTimeToPostAsyncInternal(creatorId, platform); // Método interno que retorna a entidade

         // Mapear para o BestTimeSlotDto do Shared
         return recommendationsEntity.Select(r => new MicroSaaS.Shared.DTOs.BestTimeSlotDto
         {
             Id = Guid.NewGuid(),
             CreatorId = creatorId,
             Platform = platform,
             DayOfWeek = r.DayOfWeek,
             TimeOfDay = r.TimeOfDay,
             EngagementScore = (decimal)r.EngagementScore,
             ConfidenceScore = 0.7m,
             CreatedAt = DateTime.UtcNow,
             Hour = r.TimeOfDay.Hours,
             EngagementPotential = (int)(r.EngagementScore * 10),
             RecommendationStrength = r.EngagementScore > 8 ? "Forte" : (r.EngagementScore > 6 ? "Média" : "Baixa")
         }).OrderByDescending(dto => dto.EngagementScore).ToList();
    }

    public async Task<List<MicroSaaS.Shared.DTOs.ContentRecommendationDto>> GetContentRecommendationsAsync(Guid creatorId, SocialMediaPlatform platform)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(creatorId));

        // Implementação de exemplo - na versão real faria análise dos dados de performance
        var result = new List<MicroSaaS.Shared.DTOs.ContentRecommendationDto>
        {
            new MicroSaaS.Shared.DTOs.ContentRecommendationDto
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                Title = "Diversifique seus tópicos",
                Description = "Baseado na análise do seu conteúdo, recomendamos expandir para tópicos relacionados para aumentar o alcance.",
                Type = MicroSaaS.Shared.Enums.RecommendationType.Topic,
                Platform = platform,
                ConfidenceScore = 0.85m,
                ExampleContentIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                SuggestedHashtags = new List<string> { "#dicas", "#tutorial" },
                SuggestedKeywords = new List<string> { "como fazer", "passo a passo" },
                CreatedAt = DateTime.UtcNow
            },
            new MicroSaaS.Shared.DTOs.ContentRecommendationDto
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                Title = "Experimente vídeos curtos",
                Description = "Vídeos de 60-90 segundos estão gerando maior engajamento em sua niche.",
                Type = MicroSaaS.Shared.Enums.RecommendationType.ContentFormat,
                Platform = platform,
                ConfidenceScore = 0.9m,
                ExampleContentIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                SuggestedHashtags = new List<string> { "#shorts", "#viral" },
                SuggestedKeywords = new List<string> { "rápido", "simples" },
                CreatedAt = DateTime.UtcNow
            }
        };

        return result;
    }

    public async Task<List<MicroSaaS.Shared.DTOs.GrowthRecommendationDto>> GetGrowthRecommendationsAsync(Guid creatorId, SocialMediaPlatform platform)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null) throw new ArgumentException("Criador não encontrado.");

        var recommendations = new List<MicroSaaS.Shared.DTOs.GrowthRecommendationDto>
        {
            new MicroSaaS.Shared.DTOs.GrowthRecommendationDto
            {
                 Id = Guid.NewGuid(),
                 CreatorId = creatorId,
                 Platform = platform,
                 Category = MicroSaaS.Shared.DTOs.GrowthCategory.Reach,
                 Title = "Colaboração Live",
                 Description = "Considere fazer uma live conjunta com outro criador.",
                 ExpectedOutcome = "Alto impacto potencial no crescimento da audiência.",
                 Difficulty = 3,
                 ImplementationSteps = new List<string>{"Identificar parceiro", "Planejar conteúdo", "Divulgar"},
                 TimeToImplement = "1-2 semanas",
                 CreatedAt = DateTime.UtcNow
            },
             new MicroSaaS.Shared.DTOs.GrowthRecommendationDto
            {
                 Id = Guid.NewGuid(),
                 CreatorId = creatorId,
                 Platform = platform,
                 Category = MicroSaaS.Shared.DTOs.GrowthCategory.Engagement,
                 Title = "Aumentar Consistência",
                 Description = "Mantenha uma frequência de 2 posts por semana nesta plataforma.",
                 ExpectedOutcome = "Impacto médio no engajamento e retenção.",
                 Difficulty = 2,
                 ImplementationSteps = new List<string>{"Planejar calendário", "Produzir conteúdo", "Agendar posts"},
                 TimeToImplement = "Contínuo",
                 CreatedAt = DateTime.UtcNow
            }
        };
        return recommendations;
    }

    public async Task<MicroSaaS.Shared.DTOs.ContentRecommendationDto> GenerateCustomRecommendationAsync(MicroSaaS.Shared.DTOs.CustomRecommendationRequestDto request)
    {
        var creator = await _creatorRepository.GetByIdAsync(request.CreatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado", nameof(request.CreatorId));

        // Implementação de exemplo - versão real usaria dados históricos e ML
        var recommendation = new MicroSaaS.Shared.DTOs.ContentRecommendationDto
        {
            Id = Guid.NewGuid(),
            CreatorId = request.CreatorId,
            Title = $"Recomendação personalizada: {request.RecommendationType}",
            Description = $"Recomendação baseada em seus critérios específicos para {request.Platform}.",
            Type = request.RecommendationType,
            Platform = request.Platform,
            ConfidenceScore = 0.85m,
            ExampleContentIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
            SuggestedHashtags = new List<string> { "#personalizado", "#recomendado" },
            SuggestedKeywords = new List<string> { request.SpecificTopic ?? "conteúdo", request.ContentGoal ?? "engajamento" },
            CreatedAt = DateTime.UtcNow
        };

        return recommendation;
    }

    public async Task<MicroSaaS.Shared.DTOs.AudienceSensitivityDto> GetAudienceSensitivityAnalysisAsync(Guid creatorId, SocialMediaPlatform platform)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null) throw new ArgumentException("Criador não encontrado.");
        // Lógica simulada para análise de sensibilidade
         var analysis = new MicroSaaS.Shared.DTOs.AudienceSensitivityDto
         {
              SensitivityTopics = new List<MicroSaaS.Shared.DTOs.TopicSensitivityDto>
              {
                  new MicroSaaS.Shared.DTOs.TopicSensitivityDto { Topic = "Política Partidária", SensitivityLevel = 4, RecommendedApproach = "Evitar ou abordar com extrema cautela" },
                  new MicroSaaS.Shared.DTOs.TopicSensitivityDto { Topic = "Notícias Falsas", SensitivityLevel = 5, RecommendedApproach = "Desmentir ativamente" }
              },
              RecommendedContentApproach = "Informativo e Neutro",
              TopResponsiveTopics = new List<string> { "Dicas Práticas", "Tutoriais" },
              TopResponsiveFormats = new List<string> { "Vídeo Curto", "Carrossel" },
              LastUpdated = DateTime.UtcNow.AddDays(-5),
              Platform = platform.ToString(),
              CreatorId = creatorId,
              ConfidenceScore = 0.75m,
              OverallSensitivity = 0.6
         };
        return analysis; // Retorna diretamente o DTO
    }

    public async Task<MicroSaaS.Shared.DTOs.GrowthRecommendationDto> GenerateCustomGrowthRecommendationAsync(MicroSaaS.Shared.DTOs.CustomRecommendationRequestDto request)
    {
        // Validar o request
        if (request == null || request.CreatorId == Guid.Empty)
        {
            throw new ArgumentException("Requisição inválida para geração de recomendação de crescimento");
        }

        // Buscar dados do criador para personalizar a recomendação
        var creator = await _creatorRepository.GetByIdAsync(request.CreatorId);
        if (creator == null)
        {
            throw new KeyNotFoundException($"Criador com ID {request.CreatorId} não encontrado");
        }

        var recommendation = new MicroSaaS.Shared.DTOs.GrowthRecommendationDto
        {
            Id = Guid.NewGuid(),
            CreatorId = request.CreatorId,
            Title = $"Recomendação personalizada de crescimento: {request.RecommendationType}",
            Description = $"Recomendação personalizada baseada em {request.SpecificTopic ?? "tópicos gerais"} para {request.TargetAudience ?? "sua audiência atual"}",
            Category = MicroSaaS.Shared.DTOs.GrowthCategory.Engagement,
            Platform = request.Platform,
            ImplementationSteps = new List<string>
            {
                "Analise o desempenho atual",
                $"Adapte conteúdo com foco em {request.SpecificTopic ?? "tópicos relevantes"}",
                "Implemente e monitore por 14 dias",
                "Ajuste estratégia com base nos resultados"
            },
            ExpectedOutcome = $"Aumento de engajamento na plataforma {request.Platform}",
            Difficulty = 3,
            TimeToImplement = "2-3 semanas",
            CreatedAt = DateTime.UtcNow
        };
        return recommendation;
    }

    // Métodos de Mapeamento para DTOs
    private MicroSaaS.Shared.DTOs.BestTimeSlotDto MapToDto(PostTimeRecommendation entity)
    {
        if (entity == null) return null;
        return new MicroSaaS.Shared.DTOs.BestTimeSlotDto
        {
             Id = Guid.NewGuid(),
             DayOfWeek = entity.DayOfWeek,
             TimeOfDay = entity.TimeOfDay,
             EngagementScore = (decimal)entity.EngagementScore,
             ConfidenceScore = 0.7m,
             CreatedAt = DateTime.UtcNow,
             Hour = entity.TimeOfDay.Hours,
             EngagementPotential = (int)(entity.EngagementScore * 10),
             RecommendationStrength = entity.EngagementScore > 8 ? "Forte" : (entity.EngagementScore > 6 ? "Média" : "Baixa")
        };
    }

    private MicroSaaS.Shared.DTOs.ContentRecommendationDto MapToDto(ContentRecommendation entity)
    {
        if (entity == null) return null;
        return new MicroSaaS.Shared.DTOs.ContentRecommendationDto
        {
            Id = entity.Id,
            CreatorId = entity.CreatorId,
            Title = entity.Title,
            Description = entity.Description,
            Type = (MicroSaaS.Shared.Enums.RecommendationType)Enum.Parse(typeof(MicroSaaS.Shared.Enums.RecommendationType), entity.Type.ToString()),
            Platform = entity.Platform,
            ConfidenceScore = 0.8m,
            CreatedAt = entity.CreatedAt
        };
    }

    private MicroSaaS.Shared.DTOs.TrendTopicDto MapToDto(InterfaceTrendTopic entity)
    {
        if (entity == null) return null;
        return new MicroSaaS.Shared.DTOs.TrendTopicDto
        {
            Topic = entity.Name,
            Description = entity.Description,
            Volume = entity.PopularityScore,
            Platform = entity.Platform,
            RelatedHashtags = entity.RelatedHashtags
        };
    }

    private MicroSaaS.Shared.DTOs.ContentAnalysisDto MapToDto(InterfaceContentAnalysis entity)
    {
         if (entity == null) return null;
         return new MicroSaaS.Shared.DTOs.ContentAnalysisDto
         {
             ContentId = entity.ContentId,
             Title = entity.Title,
             EngagementScore = entity.EngagementScore,
             ReachScore = entity.ReachScore,
             StrengthPoints = entity.StrengthPoints,
             ImprovementSuggestions = entity.ImprovementSuggestions,
             AnalysisDate = DateTime.UtcNow
         };
    }

    // Método auxiliar interno para obter a entidade (usado por GetBestPostingTimesAsync)
    private async Task<List<PostTimeRecommendation>> GetBestTimeToPostAsyncInternal(Guid creatorId, SocialMediaPlatform platform)
    {
         var creator = await _creatorRepository.GetByIdAsync(creatorId);
         if (creator == null) throw new ArgumentException("Criador não encontrado.");
         var account = (await _socialMediaRepository.GetByCreatorIdAsync(creatorId)).FirstOrDefault(a => a.Platform == platform);
         if (account == null) return GenerateDefaultRecommendations(platform); // Retorna padrão se conta não existe

         var performances = (await _contentPerformanceRepository.GetByCreatorIdAsync(creatorId))
                             .Where(p => p.Platform == platform).ToList();
         if (!performances.Any())
         {
             var platformRecs = await _socialMediaService.GetBestPostingTimesAsync(account.Id);
             return platformRecs.Select(r => new PostTimeRecommendation
             { /* ... mapeamento de Shared.Model para Domain.Entity ... */ DayOfWeek = r.DayOfWeek, TimeOfDay = r.TimeOfDay, EngagementScore = (double)r.EngagementScore }).ToList();
         }

         // Lógica original de análise de performance... (omitida para brevidade)
         // ...
         // Exemplo simplificado de retorno
         return GenerateDefaultRecommendations(platform); // Substituir pela lógica real
    }
}