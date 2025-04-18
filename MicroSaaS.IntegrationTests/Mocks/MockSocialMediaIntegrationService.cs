using MicroSaaS.Application.DTOs;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;
using MicroSaaS.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.IntegrationTests.Mocks
{
    public class MockSocialMediaIntegrationService : ISocialMediaIntegrationService
    {
        private readonly List<ContentPost> _publishedPosts = new List<ContentPost>();
        private readonly List<ContentPost> _scheduledPosts = new List<ContentPost>();
        private readonly Dictionary<Guid, List<ContentPerformanceDto>> _performanceData = new Dictionary<Guid, List<ContentPerformanceDto>>();
        private readonly Dictionary<Guid, List<MicroSaaS.Shared.Models.PostTimeRecommendation>> _bestPostingTimes = new Dictionary<Guid, List<MicroSaaS.Shared.Models.PostTimeRecommendation>>();

        public MockSocialMediaIntegrationService()
        {
            // Preencher com dados de teste se necessário
            SetupDefaultMockData();
        }

        private void SetupDefaultMockData()
        {
            var creatorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var postId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var accountId = Guid.Parse("33333333-3333-3333-3333-333333333333");

            // Exemplo de post publicado
            _publishedPosts.Add(new ContentPost { Id = postId, CreatorId = creatorId, Title = "Mock Published Post", Status = PostStatus.Published });

            // Exemplo de performance - Usar PostId e remover EngagementRate
            _performanceData[postId] = new List<ContentPerformanceDto> { new ContentPerformanceDto { PostId = postId, Views = 100, Likes = 10 /* Remover EngagementRate */ } };

            // Exemplo de recomendação de horário (usando Shared.Models)
            _bestPostingTimes[accountId] = new List<MicroSaaS.Shared.Models.PostTimeRecommendation>
            {
                new MicroSaaS.Shared.Models.PostTimeRecommendation { DayOfWeek = DayOfWeek.Monday, TimeOfDay = new TimeSpan(18, 0, 0), EngagementScore = 8.0m },
                new MicroSaaS.Shared.Models.PostTimeRecommendation { DayOfWeek = DayOfWeek.Friday, TimeOfDay = new TimeSpan(12, 0, 0), EngagementScore = 7.5m }
            };
        }

        // Simula a URL de autenticação para uma plataforma
        public Task<string> GetAuthUrlAsync(SocialMediaPlatform platform)
        {
            return Task.FromResult($"https://mockauth.com/{platform}?client_id=mock_client&redirect_uri=mock_redirect&scope=read,write");
        }

        // Simula o tratamento do callback de autenticação
        public Task<SocialMediaAccount> HandleAuthCallbackAsync(SocialMediaPlatform platform, string code)
        {
            // Simular retorno de conta conectada
            var account = new SocialMediaAccount
            {
                Id = Guid.NewGuid(),
                Platform = platform,
                Username = $"MockUser{platform}",
                AccessToken = $"mock_access_token_{code}",
                RefreshToken = $"mock_refresh_token_{code}",
                TokenExpiresAt = DateTime.UtcNow.AddHours(1),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            return Task.FromResult(account);
        }

        // Simula a validação de token
        public Task<bool> ValidateTokenAsync(SocialMediaAccount account)
        {
            // Mock: Assume que o token é sempre válido se expirar no futuro
            return Task.FromResult(account.TokenExpiresAt > DateTime.UtcNow);
        }

        // Simula a renovação de token
        public Task RefreshTokenAsync(SocialMediaAccount account)
        {
            // Mock: Simula a atualização do token
            account.AccessToken = $"refreshed_access_token_{Guid.NewGuid()}";
            account.TokenExpiresAt = DateTime.UtcNow.AddHours(1);
            account.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        // Simula a conexão de uma conta
        public Task ConnectAccountAsync(SocialMediaAccount account)
        {
            // Mock: Nenhuma ação complexa necessária
            Console.WriteLine($"Mock: Connecting account {account.Username} for {account.Platform}");
            return Task.CompletedTask;
        }

        // Simula a desconexão de uma conta
        public Task DisconnectAccountAsync(SocialMediaAccount account)
        {
            // Mock: Nenhuma ação complexa necessária
            Console.WriteLine($"Mock: Disconnecting account {account.Username} for {account.Platform}");
            return Task.CompletedTask;
        }

        // Simula a obtenção de estatísticas da conta
        public Task<Dictionary<string, int>> GetAccountStatsAsync(SocialMediaAccount account)
        {
            // Mock: Retorna estatísticas fixas
            var stats = new Dictionary<string, int>
            {
                { "Followers", 1000 },
                { "Posts", 50 },
                { "EngagementRate", 5 } // Expresso como inteiro aqui para simplificar
            };
            return Task.FromResult(stats);
        }

        // Simula a postagem de conteúdo
        public Task PostContentAsync(ContentPost post)
        {
            // Mock: Adiciona à lista de posts publicados e define status
            post.Status = PostStatus.Published;
            post.PublishedAt = DateTime.UtcNow;
            _publishedPosts.Add(post);
            Console.WriteLine($"Mock: Posting content '{post.Title}' to {post.Platform}");
            return Task.CompletedTask;
        }

        // Simula o agendamento de post
        public Task SchedulePostAsync(ContentPost post, DateTime scheduledTime)
        {
            // Mock: Adiciona à lista de posts agendados e define status/data
            post.Status = PostStatus.Scheduled;
            post.ScheduledTime = scheduledTime; // Usar ScheduledTime em vez de ScheduledDate
            _scheduledPosts.Add(post);
            Console.WriteLine($"Mock: Scheduling content '{post.Title}' for {post.Platform} at {scheduledTime}");
            return Task.CompletedTask;
        }

        // Simula o cancelamento de agendamento
        public Task CancelScheduledPostAsync(Guid postId)
        {
            // Mock: Remove da lista de agendados (ou marca como cancelado)
            var post = _scheduledPosts.FirstOrDefault(p => p.Id == postId);
            if (post != null)
            {
                _scheduledPosts.Remove(post);
                Console.WriteLine($"Mock: Cancelling scheduled post {postId}");
            }
            return Task.CompletedTask;
        }

        // Simula a obtenção de posts agendados
        public Task<IEnumerable<ContentPost>> GetScheduledPostsAsync(Guid creatorId)
        {
            return Task.FromResult(_scheduledPosts.Where(p => p.CreatorId == creatorId).AsEnumerable());
        }

        // Simula a obtenção de posts publicados
        public Task<IEnumerable<ContentPost>> GetPublishedPostsAsync(Guid creatorId)
        {
            return Task.FromResult(_publishedPosts.Where(p => p.CreatorId == creatorId).AsEnumerable());
        }

        // Simula a obtenção de performance de post
        public Task<IEnumerable<ContentPerformanceDto>> GetPostPerformanceAsync(Guid postId)
        {
            if (_performanceData.TryGetValue(postId, out var performance))
            {
                return Task.FromResult(performance.AsEnumerable());
            }
            return Task.FromResult(Enumerable.Empty<ContentPerformanceDto>());
        }

        // Simula a obtenção de performance da conta
        public Task<IEnumerable<ContentPerformanceDto>> GetAccountPerformanceAsync(Guid accountId, DateTime startDate, DateTime endDate)
        {
            // Mock: Retorna performance agregada fixa - Usar PostId e remover EngagementRate
            var performance = new List<ContentPerformanceDto>
            {
                new ContentPerformanceDto { PostId = null, Views = 5000, Likes = 500, Comments = 50 /* Remover EngagementRate */ }
            };
            return Task.FromResult(performance.AsEnumerable());
        }

        // Simula a obtenção de métricas de receita
        public Task<Dictionary<string, decimal>> GetRevenueMetricsAsync(Guid accountId, DateTime startDate, DateTime endDate)
        {
            // Mock: Retorna métricas de receita fixas
            var revenue = new Dictionary<string, decimal>
            {
                { "TotalRevenue", 150.75m },
                { "AdRevenue", 120.50m },
                { "SponsorshipRevenue", 30.25m }
            };
            return Task.FromResult(revenue);
        }

        // Simula a obtenção de melhores horários para postar
        public Task<IEnumerable<MicroSaaS.Shared.DTOs.BestTimeSlotDto>> GetBestPostingTimesAsync(Guid accountId)
        {
            if (_bestPostingTimes.TryGetValue(accountId, out var recommendations))
            {
                // Converte de Shared.Models.PostTimeRecommendation para Shared.DTOs.BestTimeSlotDto
                var dtos = recommendations.Select(r => new MicroSaaS.Shared.DTOs.BestTimeSlotDto
                {
                    Id = r.Id,
                    CreatorId = accountId,
                    Platform = SocialMediaPlatform.Instagram, // Valor padrão
                    DayOfWeek = r.DayOfWeek,
                    TimeOfDay = r.TimeOfDay,
                    Hour = r.TimeOfDay.Hours,
                    EngagementScore = r.EngagementScore,
                    ConfidenceScore = 0.7m,
                    CreatedAt = DateTime.UtcNow,
                    EngagementPotential = (int)(r.EngagementScore * 10),
                    RecommendationStrength = r.EngagementScore > 8 ? "Forte" : (r.EngagementScore > 6 ? "Média" : "Baixa")
                });
                return Task.FromResult(dtos.AsEnumerable());
            }
            // Retorna lista vazia do tipo correto
            return Task.FromResult(Enumerable.Empty<MicroSaaS.Shared.DTOs.BestTimeSlotDto>());
        }

         // Simula a obtenção de recomendações de conteúdo
        public Task<List<ContentRecommendation>> GetContentRecommendationsAsync(Guid creatorId)
        {
             // Retorna recomendações mockadas de conteúdo usando a entidade ContentRecommendation
             var recommendations = new List<ContentRecommendation>
             {
                  // Usar Title, Description, Type da Entidade
                 new ContentRecommendation { Id = Guid.NewGuid(), CreatorId = creatorId, Type = MicroSaaS.Shared.Enums.RecommendationType.Topic, Title = "Novo Software", Description = "Try a tutorial on [New Software]", Platform = SocialMediaPlatform.All, CreatedAt = DateTime.UtcNow },
                 new ContentRecommendation { Id = Guid.NewGuid(), CreatorId = creatorId, Type = MicroSaaS.Shared.Enums.RecommendationType.ContentFormat, Title = "Vídeo Curto", Description = "Create a short video (Reel/Short)", Platform = SocialMediaPlatform.All, CreatedAt = DateTime.UtcNow }
             };
             return Task.FromResult(recommendations);
        }
    }
}