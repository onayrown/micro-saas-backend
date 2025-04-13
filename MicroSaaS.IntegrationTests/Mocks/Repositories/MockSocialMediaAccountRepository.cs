using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.IntegrationTests.Mocks.Repositories
{
    public class MockSocialMediaAccountRepository : ISocialMediaAccountRepository
    {
        private readonly List<SocialMediaAccount> _accounts = new();
        
        // IDs fixos para testes consistentes
        private static readonly Guid ACCOUNT_ID_1 = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid ACCOUNT_ID_2 = Guid.Parse("33333333-3333-3333-3333-333333333333");
        private static readonly Guid CREATOR_ID_1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        
        // Construtor que inicializa contas padrão para testes
        public MockSocialMediaAccountRepository()
        {
            // Inicializar com algumas contas de teste se a lista estiver vazia
            if (_accounts.Count == 0)
            {
                _accounts.Add(new SocialMediaAccount
                {
                    Id = ACCOUNT_ID_1,
                    CreatorId = CREATOR_ID_1,
                    Platform = SocialMediaPlatform.Instagram,
                    Username = "testcreator1_instagram",
                    AccessToken = "token_instagram_1",
                    RefreshToken = "refresh_instagram_1",
                    TokenExpiresAt = DateTime.UtcNow.AddDays(60),
                    IsActive = true,
                    ProfileUrl = "profile_instagram_1",
                    CreatedAt = DateTime.UtcNow.AddDays(-25),
                    UpdatedAt = DateTime.UtcNow.AddDays(-25)
                });
                
                _accounts.Add(new SocialMediaAccount
                {
                    Id = ACCOUNT_ID_2,
                    CreatorId = CREATOR_ID_1,
                    Platform = SocialMediaPlatform.YouTube,
                    Username = "testcreator1_youtube",
                    AccessToken = "token_youtube_1",
                    RefreshToken = "refresh_youtube_1",
                    TokenExpiresAt = DateTime.UtcNow.AddDays(60),
                    IsActive = true,
                    ProfileUrl = "profile_youtube_1",
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    UpdatedAt = DateTime.UtcNow.AddDays(-20)
                });
            }
        }

        public Task<SocialMediaAccount> AddAsync(SocialMediaAccount account)
        {
            // Se o ID não foi especificado, atribuir um novo
            account.Id = account.Id == Guid.Empty ? Guid.NewGuid() : account.Id;
            _accounts.Add(account);
            return Task.FromResult(account);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var account = _accounts.FirstOrDefault(a => a.Id == id);
            if (account != null)
            {
                _accounts.Remove(account);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<SocialMediaAccount?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_accounts.FirstOrDefault(a => a.Id == id));
        }

        public Task<IEnumerable<SocialMediaAccount>> GetByCreatorIdAsync(Guid creatorId)
        {
            return Task.FromResult(_accounts.Where(a => a.CreatorId == creatorId));
        }

        public Task<SocialMediaAccount?> GetByCreatorIdAndPlatformAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            return Task.FromResult(_accounts.FirstOrDefault(a => a.CreatorId == creatorId && a.Platform == platform));
        }

        public Task<SocialMediaAccount> UpdateAsync(SocialMediaAccount account)
        {
            var existingAccount = _accounts.FirstOrDefault(a => a.Id == account.Id);
            if (existingAccount != null)
            {
                _accounts.Remove(existingAccount);
                _accounts.Add(account);
                return Task.FromResult(account);
            }
            throw new InvalidOperationException("Account not found for update");
        }

        public Task UpdateTokenAsync(Guid id, string accessToken, string refreshToken, DateTime expiresAt)
        {
            var account = _accounts.FirstOrDefault(a => a.Id == id);
            if (account != null)
            {
                account.AccessToken = accessToken;
                account.RefreshToken = refreshToken;
                account.TokenExpiresAt = expiresAt;
            }
            return Task.CompletedTask;
        }

        // Método para verificar se existe um criador
        public Task<bool> CreatorExistsAsync(Guid creatorId)
        {
            // Para fins de teste, vamos considerar que qualquer ID fornecido existe
            return Task.FromResult(true);
        }

        // Método para obter contas por plataforma
        public Task<IEnumerable<SocialMediaAccount>> GetByPlatformAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            return Task.FromResult<IEnumerable<SocialMediaAccount>>(new List<SocialMediaAccount>());
        }

        // Método para obter o total de seguidores
        public Task<int> GetTotalFollowersAsync()
        {
            return Task.FromResult(1000);
        }

        // Método para obter o total de seguidores por criador
        public Task<int> GetTotalFollowersByCreatorAsync(Guid creatorId)
        {
            return Task.FromResult(500);
        }

        // Método para atualizar métricas de redes sociais
        public Task RefreshSocialMediaMetricsAsync()
        {
            return Task.CompletedTask;
        }
    }
} 