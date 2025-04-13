using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.IntegrationTests.Mocks.Services
{
    public class MockRecommendationService : IRecommendationService
    {
        public Task<List<MicroSaaS.Domain.Entities.PostTimeRecommendation>> GetBestTimeToPostAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            // Gerar recomendações de teste para hora de postagem
            var recommendations = new List<MicroSaaS.Domain.Entities.PostTimeRecommendation>
            {
                new MicroSaaS.Domain.Entities.PostTimeRecommendation 
                { 
                    DayOfWeek = DayOfWeek.Monday, 
                    TimeOfDay = new TimeSpan(12, 0, 0),
                    EngagementScore = 7.5
                },
                new MicroSaaS.Domain.Entities.PostTimeRecommendation 
                { 
                    DayOfWeek = DayOfWeek.Wednesday, 
                    TimeOfDay = new TimeSpan(18, 0, 0),
                    EngagementScore = 8.2
                },
                new MicroSaaS.Domain.Entities.PostTimeRecommendation 
                { 
                    DayOfWeek = DayOfWeek.Friday, 
                    TimeOfDay = new TimeSpan(17, 0, 0),
                    EngagementScore = 8.7
                }
            };
            
            return Task.FromResult(recommendations);
        }

        public Task<Dictionary<SocialMediaPlatform, List<MicroSaaS.Domain.Entities.PostTimeRecommendation>>> GetBestTimeToPostAllPlatformsAsync(Guid creatorId)
        {
            var result = new Dictionary<SocialMediaPlatform, List<MicroSaaS.Domain.Entities.PostTimeRecommendation>>
            {
                { 
                    SocialMediaPlatform.Instagram, 
                    new List<MicroSaaS.Domain.Entities.PostTimeRecommendation>
                    {
                        new MicroSaaS.Domain.Entities.PostTimeRecommendation 
                        { 
                            DayOfWeek = DayOfWeek.Monday, 
                            TimeOfDay = new TimeSpan(12, 0, 0),
                            EngagementScore = 7.5
                        }
                    }
                },
                { 
                    SocialMediaPlatform.YouTube, 
                    new List<MicroSaaS.Domain.Entities.PostTimeRecommendation>
                    {
                        new MicroSaaS.Domain.Entities.PostTimeRecommendation 
                        { 
                            DayOfWeek = DayOfWeek.Tuesday, 
                            TimeOfDay = new TimeSpan(16, 0, 0),
                            EngagementScore = 8.0
                        }
                    }
                }
            };
            
            return Task.FromResult(result);
        }

        public Task<List<ContentRecommendation>> GetTopicRecommendationsAsync(Guid creatorId)
        {
            return Task.FromResult(new List<ContentRecommendation>());
        }

        public Task<List<ContentRecommendation>> GetFormatRecommendationsAsync(Guid creatorId)
        {
            return Task.FromResult(new List<ContentRecommendation>());
        }

        public Task<List<string>> GetHashtagRecommendationsAsync(Guid creatorId, string contentDescription, SocialMediaPlatform platform)
        {
            return Task.FromResult(new List<string> { "#exemplo", "#teste", "#mock" });
        }

        public Task<List<TrendTopic>> GetTrendingTopicsAsync(SocialMediaPlatform platform)
        {
            return Task.FromResult(new List<TrendTopic>());
        }

        public Task<List<TrendTopic>> GetNicheTrendingTopicsAsync(Guid creatorId)
        {
            return Task.FromResult(new List<TrendTopic>());
        }

        public Task<List<ContentRecommendation>> GetMonetizationRecommendationsAsync(Guid creatorId)
        {
            return Task.FromResult(new List<ContentRecommendation>());
        }

        public Task<List<ContentRecommendation>> GetAudienceGrowthRecommendationsAsync(Guid creatorId)
        {
            return Task.FromResult(new List<ContentRecommendation>());
        }

        public Task<List<ContentRecommendation>> GetEngagementImprovementRecommendationsAsync(Guid creatorId)
        {
            return Task.FromResult(new List<ContentRecommendation>());
        }

        public Task<ContentAnalysis> AnalyzeContentPerformanceAsync(Guid contentId)
        {
            return Task.FromResult(new ContentAnalysis());
        }

        public Task RefreshRecommendationsAsync(Guid creatorId)
        {
            return Task.CompletedTask;
        }

        public Task<List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>> GetBestPostingTimesAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            return Task.FromResult(new List<MicroSaaS.Shared.DTOs.BestTimeSlotDto> 
            {
                new MicroSaaS.Shared.DTOs.BestTimeSlotDto 
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Platform = platform,
                    DayOfWeek = DayOfWeek.Monday,
                    TimeOfDay = new TimeSpan(12, 0, 0),
                    Hour = 12,
                    EngagementScore = 8.5m,
                    ConfidenceScore = 0.85m,
                    EngagementPotential = 85,
                    RecommendationStrength = "Alto",
                    CreatedAt = DateTime.UtcNow
                }
            });
        }

        public Task<List<ContentRecommendationDto>> GetContentRecommendationsAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            return Task.FromResult(new List<ContentRecommendationDto> 
            {
                new ContentRecommendationDto 
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = "Recomendação de teste",
                    Description = "Esta é uma recomendação de teste para testes de integração",
                    Type = MicroSaaS.Shared.DTOs.RecommendationType.Topic,
                    Platform = platform,
                    ConfidenceScore = 0.9m,
                    ExampleContentIds = new List<Guid> { Guid.Parse("00000000-0000-0000-0000-000000000123"), Guid.Parse("00000000-0000-0000-0000-000000000456") },
                    SuggestedHashtags = new List<string> { "#teste", "#integracao" },
                    SuggestedKeywords = new List<string> { "teste", "integração" },
                    CreatedAt = DateTime.UtcNow
                }
            });
        }

        public Task<List<GrowthRecommendationDto>> GetGrowthRecommendationsAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            return Task.FromResult(new List<GrowthRecommendationDto> 
            {
                new GrowthRecommendationDto 
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = "Recomendação de crescimento",
                    Description = "Esta é uma recomendação de crescimento para testes",
                    Category = GrowthCategory.Engagement,
                    Platform = platform,
                    ImplementationSteps = new List<string> { "Passo 1", "Passo 2" },
                    ExpectedOutcome = "Resultado esperado para teste",
                    Difficulty = 2,
                    TimeToImplement = "1 semana",
                    CreatedAt = DateTime.UtcNow
                }
            });
        }

        public Task<ContentRecommendationDto> GenerateCustomRecommendationAsync(CustomRecommendationRequestDto request)
        {
            return Task.FromResult(new ContentRecommendationDto 
            {
                Id = Guid.NewGuid(),
                CreatorId = request.CreatorId,
                Title = "Recomendação personalizada",
                Description = "Esta é uma recomendação personalizada para testes",
                Type = request.RecommendationType,
                Platform = request.Platform,
                ConfidenceScore = 0.85m,
                ExampleContentIds = new List<Guid> { Guid.Parse("00000000-0000-0000-0000-000000000123"), Guid.Parse("00000000-0000-0000-0000-000000000456") },
                SuggestedHashtags = new List<string> { "#personalizado", "#teste" },
                SuggestedKeywords = new List<string> { "personalizado", "teste" },
                CreatedAt = DateTime.UtcNow
            });
        }

        public Task<MicroSaaS.Shared.DTOs.AudienceSensitivityDto> GetAudienceSensitivityAnalysisAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            return Task.FromResult(new MicroSaaS.Shared.DTOs.AudienceSensitivityDto
            {
                CreatorId = creatorId,
                Platform = platform.ToString(),
                TopResponsiveTopics = new List<string> { "Educação", "Tecnologia" },
                TopResponsiveFormats = new List<string> { "Vídeos curtos", "Carrosséis" },
                BestTimeOfDay = new List<TimeSpan> { new TimeSpan(12, 0, 0), new TimeSpan(18, 0, 0) },
                BestDaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Friday },
                ConfidenceScore = 0.85m,
                LastUpdated = DateTime.UtcNow,
                SensitivityTopics = new List<TopicSensitivityDto> 
                {
                    new TopicSensitivityDto 
                    {
                        Topic = "Política",
                        SensitivityLevel = 8,
                        RecommendedApproach = "Evitar ou abordar com neutralidade"
                    },
                    new TopicSensitivityDto 
                    {
                        Topic = "Saúde",
                        SensitivityLevel = 6,
                        RecommendedApproach = "Abordar com responsabilidade e fontes"
                    }
                },
                OverallSensitivity = 7.0,
                Analysis = "Sua audiência é sensível a tópicos políticos e prefere conteúdo educacional",
                RecommendedContentApproach = "Foque em conteúdo educacional e evite polarizações"
            });
        }

        public Task<GrowthRecommendationDto> GenerateCustomGrowthRecommendationAsync(CustomRecommendationRequestDto request)
        {
            return Task.FromResult(new GrowthRecommendationDto
            {
                Id = Guid.NewGuid(),
                CreatorId = request.CreatorId,
                Title = "Recomendação de crescimento personalizada",
                Description = "Esta é uma recomendação de crescimento personalizada para testes",
                Category = GrowthCategory.ContentQuality,
                Platform = request.Platform,
                ImplementationSteps = new List<string> { "Passo 1 personalizado", "Passo 2 personalizado" },
                ExpectedOutcome = "Resultado esperado personalizado para teste",
                Difficulty = 3,
                TimeToImplement = "2 semanas",
                CreatedAt = DateTime.UtcNow
            });
        }

        public Task<List<Domain.Entities.ContentRecommendation>> GetContentRecommendationsAsync(Guid creatorId)
        {
            return Task.FromResult(new List<Domain.Entities.ContentRecommendation>
            {
                new Domain.Entities.ContentRecommendation
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = "Recomendação de conteúdo teste",
                    Description = "Esta é uma recomendação de conteúdo para testes de mock",
                    Type = MicroSaaS.Shared.Enums.RecommendationType.ContentTopic,
                    Platform = SocialMediaPlatform.Instagram,
                    CreatedAt = DateTime.UtcNow
                }
            });
        }

        public Task<List<ContentRecommendation>> GetRecommendationsAsync(Guid creatorId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var recommendations = new List<ContentRecommendation>
            {
                new ContentRecommendation
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = "Aumentar frequência de postagem no Instagram",
                    Description = "Análise mostra que aumentar a frequência de postagem em 30% pode melhorar seu alcance",
                    Priority = RecommendationPriority.High,
                    Type = Shared.Enums.RecommendationType.PostingFrequency
                },
                new ContentRecommendation
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = "Criar conteúdo em formato carrossel",
                    Description = "Conteúdos em formato carrossel têm engajamento 25% maior",
                    Priority = RecommendationPriority.Medium,
                    Type = Shared.Enums.RecommendationType.ContentFormat
                }
            };

            return Task.FromResult(recommendations);
        }
    }
} 