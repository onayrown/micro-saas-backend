using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.IntegrationTests.Mocks
{
    public class MockRecommendationService : IRecommendationService
    {
        // Propriedades para controlar o comportamento do mock
        public List<MicroSaaS.Shared.DTOs.BestTimeSlotDto> BestTimeToPost { get; set; } = new();
        public Dictionary<SocialMediaPlatform, List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>> BestTimeToPostAll { get; set; } = new();
        public List<ContentRecommendationDto> ContentRecommendations { get; set; } = new();
        public List<string> HashtagRecommendations { get; set; } = new();
        public List<TrendTopicDto> TrendingTopics { get; set; } = new();
        public ContentAnalysisDto AnalysisResult { get; set; } = new();
        public List<GrowthRecommendationDto> GrowthRecommendations { get; set; } = new();
        public MicroSaaS.Shared.DTOs.AudienceSensitivityDto SensitivityAnalysis { get; set; } = new();

        public MockRecommendationService()
        {
             // Inicializar ContentRecommendations com o tipo correto
             ContentRecommendations.Add(new ContentRecommendationDto { /* ... mock data ... */ });
        }

        // Simula a obtenção de melhores horários para postar (DEVE RETORNAR DTO)
        public Task<List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>> GetBestTimeToPostAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            var recommendations = new List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>
            {
                // Ajustar para criar BestTimeSlotDto com propriedades corretas
                new MicroSaaS.Shared.DTOs.BestTimeSlotDto { Platform = platform, DayOfWeek = DayOfWeek.Tuesday, Hour = 8, TimeOfDay = new TimeSpan(8, 0, 0), EngagementScore = 0.9m, ConfidenceScore = 0.8m },
                new MicroSaaS.Shared.DTOs.BestTimeSlotDto { Platform = platform, DayOfWeek = DayOfWeek.Thursday, Hour = 16, TimeOfDay = new TimeSpan(16, 0, 0), EngagementScore = 0.8m, ConfidenceScore = 0.7m }
            };
            return Task.FromResult(recommendations.Where(r => r.Platform == platform).ToList()); // Filtrar pela plataforma já que a lista mockada é genérica
        }

        // Simula a obtenção de melhores horários para todas as plataformas (DEVE RETORNAR DICIONÁRIO DE DTOs)
        public Task<Dictionary<SocialMediaPlatform, List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>>> GetBestTimeToPostAllPlatformsAsync(Guid creatorId)
        {
            var allRecommendations = new Dictionary<SocialMediaPlatform, List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>>();
            foreach (SocialMediaPlatform platform in Enum.GetValues(typeof(SocialMediaPlatform)))
            {
                if(platform != SocialMediaPlatform.All)
                {
                    allRecommendations[platform] = new List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>
                    {
                         // Ajustar para criar BestTimeSlotDto com propriedades corretas
                        new MicroSaaS.Shared.DTOs.BestTimeSlotDto { Platform = platform, DayOfWeek = DayOfWeek.Wednesday, Hour = (9 + (int)platform) % 24, TimeOfDay = new TimeSpan((9 + (int)platform) % 24, 0, 0), EngagementScore = 0.85m, ConfidenceScore = 0.75m },
                        new MicroSaaS.Shared.DTOs.BestTimeSlotDto { Platform = platform, DayOfWeek = DayOfWeek.Friday, Hour = (17 + (int)platform) % 24, TimeOfDay = new TimeSpan((17 + (int)platform) % 24, 0, 0), EngagementScore = 0.75m, ConfidenceScore = 0.65m }
                    };
                }
            }
            return Task.FromResult(allRecommendations);
        }

        // Simula a obtenção de recomendações de tópicos (DEVE RETORNAR LISTA DE DTOs)
        public Task<List<ContentRecommendationDto>> GetTopicRecommendationsAsync(Guid creatorId)
        {
            var recommendations = new List<ContentRecommendationDto>
            {
                 // Ajustar para criar ContentRecommendationDto
                new ContentRecommendationDto { Id=Guid.NewGuid(), CreatorId=creatorId, Type = MicroSaaS.Shared.Enums.RecommendationType.Topic, Title = "Explore AI", Description = "Explore AI in content creation", ConfidenceScore = 0.9m, Platform = SocialMediaPlatform.All, Score = 90.0m }
            };
            return Task.FromResult(recommendations);
        }

        // Simula a obtenção de recomendações de formato (DEVE RETORNAR LISTA DE DTOs)
        public Task<List<ContentRecommendationDto>> GetFormatRecommendationsAsync(Guid creatorId)
        {
             var recommendations = new List<ContentRecommendationDto>
            {
                 // Corrigir CS0428: Usar RecommendationType.ContentFormat
                 new ContentRecommendationDto { Id=Guid.NewGuid(), CreatorId=creatorId, Type = MicroSaaS.Shared.Enums.RecommendationType.ContentFormat, Title="Try Infographics", Description = "Try creating infographics", ConfidenceScore = 0.7m, Platform = SocialMediaPlatform.All, Score = 70.0m }
            };
            return Task.FromResult(recommendations);
        }

        // Simula a obtenção de recomendações de hashtag
        public Task<List<string>> GetHashtagRecommendationsAsync(Guid creatorId, string contentDescription, SocialMediaPlatform platform)
        {
            var hashtags = new List<string> { "#mockhashtag1", "#mockhashtag2", "#content" };
            return Task.FromResult(hashtags);
        }

        // Simula a obtenção de tópicos em alta (DEVE RETORNAR LISTA DE DTOs)
        public Task<List<TrendTopicDto>> GetTrendingTopicsAsync(SocialMediaPlatform platform)
        {
             var trends = new List<TrendTopicDto>
            {
                // Ajustar para criar TrendTopicDto com propriedades corretas
                new TrendTopicDto { Platform = platform, Topic = "Trending Mock Topic 1", Description = "Popular trend", Volume = 10000, RelatedHashtags = new List<string>{"#trend1"} },
                new TrendTopicDto { Platform = platform, Topic = "Trending Mock Topic 2", Description = "Another trend", Volume = 8000, RelatedHashtags = new List<string>{"#trend2"} }
            };
            return Task.FromResult(trends);
        }

        // Simula a obtenção de tópicos em alta no nicho (DEVE RETORNAR LISTA DE DTOs)
        public Task<List<TrendTopicDto>> GetNicheTrendingTopicsAsync(Guid creatorId)
        {
             var trends = new List<TrendTopicDto>
            {
                 // Ajustar para criar TrendTopicDto com propriedades corretas
                new TrendTopicDto { Platform = SocialMediaPlatform.All, Topic = "Niche Mock Topic A", Description = "Niche trend A", Volume = 500, RelatedHashtags = new List<string>{"#nicheA"} },
                new TrendTopicDto { Platform = SocialMediaPlatform.All, Topic = "Niche Mock Topic B", Description = "Niche trend B", Volume = 450, RelatedHashtags = new List<string>{"#nicheB"} }
            };
            return Task.FromResult(trends);
        }

        // Simula a obtenção de recomendações de monetização (DEVE RETORNAR LISTA DE DTOs)
        public Task<List<ContentRecommendationDto>> GetMonetizationRecommendationsAsync(Guid creatorId)
        {
            var recommendations = new List<ContentRecommendationDto>
            {
                 // Corrigir CS0117: Usar MonetizationStrategy
                 new ContentRecommendationDto { Id=Guid.NewGuid(), CreatorId=creatorId, Type = MicroSaaS.Shared.Enums.RecommendationType.Topic, Title="Affiliate Marketing", Description = "Consider affiliate marketing for tech products", ConfidenceScore = 0.8m, Platform = SocialMediaPlatform.All, Score = 80.0m }
            };
            return Task.FromResult(recommendations);
        }

        // Simula a obtenção de recomendações de crescimento de audiência (DEVE RETORNAR LISTA DE DTOs)
        public Task<List<ContentRecommendationDto>> GetAudienceGrowthRecommendationsAsync(Guid creatorId)
        {
             var recommendations = new List<ContentRecommendationDto>
            {
                 // Corrigir CS0117: Usar AudienceTargeting
                 new ContentRecommendationDto { Id=Guid.NewGuid(), CreatorId=creatorId, Type = MicroSaaS.Shared.Enums.RecommendationType.CollaborationOpportunity, Title="Collaboration", Description = "Collaborate with another creator in your niche", ConfidenceScore = 0.88m, Platform = SocialMediaPlatform.All, Score = 88.0m }
            };
            return Task.FromResult(recommendations);
        }

        // Simula a obtenção de recomendações de melhoria de engajamento (DEVE RETORNAR LISTA DE DTOs)
        public Task<List<ContentRecommendationDto>> GetEngagementImprovementRecommendationsAsync(Guid creatorId)
        {
            var recommendations = new List<ContentRecommendationDto>
            {
                 // Corrigir CS0117: Usar EngagementTactic
                 new ContentRecommendationDto { Id=Guid.NewGuid(), CreatorId=creatorId, Type = MicroSaaS.Shared.Enums.RecommendationType.Topic, Title="Ask Questions", Description = "Ask questions in your post captions", ConfidenceScore = 0.82m, Platform = SocialMediaPlatform.All, Score = 82.0m }
            };
            return Task.FromResult(recommendations);
        }

        // Simula a análise de performance de conteúdo (DEVE RETORNAR DTO)
        public Task<ContentAnalysisDto> AnalyzeContentPerformanceAsync(Guid contentId)
        {
            // A propriedade AnalysisResult já é do tipo DTO
            AnalysisResult.ContentId = contentId; // Atualiza o ID no DTO mockado
            // Remover uso de propriedades que não existem mais (Keywords, PositiveSentiment)
            // AnalysisResult.PositiveSentiment = 0.7; // REMOVER
            // AnalysisResult.Keywords = new List<string>{"mock", "analysis", "content"}; // REMOVER
            // Adicionar outras propriedades mockadas se necessário, baseado na definição real de ContentAnalysisDto
            AnalysisResult.Title = "Mock Analysis";
            AnalysisResult.EngagementScore = 7.5;

            return Task.FromResult(AnalysisResult);
        }

        // Simula a atualização de recomendações
        public Task RefreshRecommendationsAsync(Guid creatorId)
        {
             Console.WriteLine($"Mock RefreshRecommendationsAsync called for creator: {creatorId}");
            return Task.CompletedTask;
        }

        // ---- Métodos usando DTOs (conforme interface mais recente) ----

        // Simula a obtenção de recomendações de conteúdo (DTO)
        public Task<List<ContentRecommendationDto>> GetContentRecommendationsAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            var recommendations = new List<ContentRecommendationDto>
            {
                 // Corrigir CS0428: Usar RecommendationType.ContentFormat
                 new ContentRecommendationDto { Id = Guid.NewGuid(), CreatorId = creatorId, Type = MicroSaaS.Shared.Enums.RecommendationType.Topic, Title = $"Specific for {platform}", Description = $"Focus on {platform} trends", Score = 90.0m, Platform = platform, ConfidenceScore = 0.9m },
                 new ContentRecommendationDto { Id = Guid.NewGuid(), CreatorId = creatorId, Type = MicroSaaS.Shared.Enums.RecommendationType.ContentFormat, Title = "Live Stream", Description = $"Go live on {platform}", Score = 80.0m, Platform = platform, ConfidenceScore = 0.8m }
            };
            // Não é necessário fazer cast ou conversão, pois estamos usando o enum correto diretamente

            return Task.FromResult(ContentRecommendations.Where(r => r.Platform == platform).Select(r => { r.CreatorId = creatorId; return r; }).ToList());
        }

        // Simula a obtenção de recomendações de crescimento (DTO)
        public Task<List<GrowthRecommendationDto>> GetGrowthRecommendationsAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            return Task.FromResult(GrowthRecommendations.Where(r => r.Platform == platform).Select(r => { r.CreatorId = creatorId; return r; }).ToList());
        }

        // Simula a geração de recomendação customizada (DTO)
        public Task<ContentRecommendationDto> GenerateCustomRecommendationAsync(CustomRecommendationRequestDto request)
        {
            var customRec = new ContentRecommendationDto
            {
                Id = Guid.NewGuid(),
                CreatorId = request.CreatorId,
                Title = $"Custom Recommendation: {request.RecommendationType}",
                Description = $"Mock recommendation for {request.Platform} based on {request.SpecificTopic ?? "general analysis"}",
                Type = MicroSaaS.Shared.Enums.RecommendationType.Topic,
                Platform = request.Platform,
                ConfidenceScore = 0.75m,
                CreatedAt = DateTime.UtcNow
            };
            return Task.FromResult(customRec);
        }

        // Simula a análise de sensibilidade da audiência (DTO)
        public Task<MicroSaaS.Shared.DTOs.AudienceSensitivityDto> GetAudienceSensitivityAnalysisAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            // Usar a propriedade já qualificada
            this.SensitivityAnalysis.CreatorId = creatorId;
            this.SensitivityAnalysis.Platform = platform.ToString();
            return Task.FromResult<MicroSaaS.Shared.DTOs.AudienceSensitivityDto>(this.SensitivityAnalysis);
        }

        // Simula a geração de recomendação de crescimento customizada (DTO)
         public Task<GrowthRecommendationDto> GenerateCustomGrowthRecommendationAsync(CustomRecommendationRequestDto request)
         {
             var customGrowthRec = new GrowthRecommendationDto
             {
                 Id = Guid.NewGuid(),
                 CreatorId = request.CreatorId,
                 Title = $"Custom Growth: {request.RecommendationType}",
                 Description = $"Mock growth recommendation for {request.Platform}",
                 Category = GrowthCategory.Engagement,
                 Platform = request.Platform,
                 ExpectedOutcome = "Improved engagement",
                 Difficulty = 2,
                 TimeToImplement = "1 week",
                 CreatedAt = DateTime.UtcNow
             };
             return Task.FromResult(customGrowthRec);
         }

        // Implementação ausente na interface original, mas potencialmente útil para mock
        public Task<List<ContentRecommendation>> GetRecommendationsAsync(Guid creatorId, DateTime? startDate = null, DateTime? endDate = null)
        {
            Console.WriteLine($"Mock GetRecommendationsAsync called for creator: {creatorId}, Start: {startDate}, End: {endDate}");
            var recommendations = new List<ContentRecommendation>
            {
                 // Qualificar RecommendationType para resolver ambiguidade CS0104
                 new ContentRecommendation { Id=Guid.NewGuid(), CreatorId=creatorId, Type = MicroSaaS.Shared.Enums.RecommendationType.Topic, Title = "Recomendação Genérica", Description = "Generic Mock Recommendation", Platform = SocialMediaPlatform.All, CreatedAt = DateTime.UtcNow }
            };
            return Task.FromResult(recommendations);
        }

        // Implementação FALTANTE para GetContentRecommendationsAsync(Guid)
        public Task<List<ContentRecommendationDto>> GetContentRecommendationsAsync(Guid creatorId)
        {
            Console.WriteLine($"Mock GetContentRecommendationsAsync(Guid creatorId) called.");
            // Retorna a lista de ContentRecommendationDto da propriedade do mock
            var recommendations = ContentRecommendations
                                .Select(r => { r.CreatorId = creatorId; return r; })
                                .ToList();
            return Task.FromResult(recommendations);
        }

        // Implementação FALTANTE para GetBestPostingTimesAsync
        public Task<List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>> GetBestPostingTimesAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            // A interface define este método E GetBestTimeToPostAsync.
            // Como GetBestTimeToPostAsync já foi corrigido para retornar DTO, podemos chamá-lo.
            return GetBestTimeToPostAsync(creatorId, platform);
        }

        // Implementação FALTANTE para GetAudienceSensitivityAsync
        public Task<MicroSaaS.Shared.DTOs.AudienceSensitivityDto> GetAudienceSensitivityAsync(Guid creatorId)
        {
            // Retornar a propriedade qualificada
            return Task.FromResult<MicroSaaS.Shared.DTOs.AudienceSensitivityDto>(this.SensitivityAnalysis);
        }

        // Implementação FALTANTE para GetEngagementRecommendationsAsync
        public Task<List<MicroSaaS.Shared.DTOs.RecommendationDto>> GetEngagementRecommendationsAsync(Guid creatorId)
        {
            return Task.FromResult(new List<MicroSaaS.Shared.DTOs.RecommendationDto>
            {
                 new MicroSaaS.Shared.DTOs.RecommendationDto { Id = Guid.NewGuid(), Type = MicroSaaS.Shared.Enums.RecommendationType.Topic, Title="Run a Poll", Description="Engage with audience", Score = 88, CreatedAt = DateTime.UtcNow }
            });
        }

        public Task UpdateRecommendationFeedbackAsync(Guid recommendationId, bool liked)
        {
            Console.WriteLine($"Mock: Feedback for {recommendationId} - Liked: {liked}");
            return Task.CompletedTask;
        }

        // Métodos GetRecommendationsAsync(Guid, RecommendationType, int) e GetGeneralRecommendationsAsync(Guid, int)
        // Qualificar RecommendationType nesses métodos também
        public Task<IEnumerable<ContentRecommendation>> GetRecommendationsAsync(Guid creatorId, MicroSaaS.Shared.Enums.RecommendationType type, int limit = 5)
        {
            Console.WriteLine($"Mock: GetRecommendationsAsync (type: {type}) called.");
             var mockRecs = new List<ContentRecommendation>
             {
                  new ContentRecommendation { Id = Guid.NewGuid(), CreatorId = creatorId, Type = MicroSaaS.Shared.Enums.RecommendationType.Topic, Title = "Mock Topic Rec", Description = "Desc..." },
                  new ContentRecommendation { Id = Guid.NewGuid(), CreatorId = creatorId, Type = MicroSaaS.Shared.Enums.RecommendationType.ContentFormat, Title = "Mock Format Rec", Description = "Desc..." }
             };
             return Task.FromResult(mockRecs.Where(r => r.Type == type).Take(limit));
        }

        public Task<IEnumerable<ContentRecommendation>> GetGeneralRecommendationsAsync(Guid creatorId, int limit = 5)
        {
             Console.WriteLine("Mock: GetGeneralRecommendationsAsync called.");
             var mockRecs = new List<ContentRecommendation>
             {
                  new ContentRecommendation { Id = Guid.NewGuid(), CreatorId = creatorId, Type = MicroSaaS.Shared.Enums.RecommendationType.Topic, Title = "General Mock Topic Rec", Description = "Desc..." },
                  new ContentRecommendation { Id = Guid.NewGuid(), CreatorId = creatorId, Type = MicroSaaS.Shared.Enums.RecommendationType.Strategy, Title = "General Mock Strategy Rec", Description = "Desc..." }
             };
             return Task.FromResult(mockRecs.Take(limit));
        }
    }
}