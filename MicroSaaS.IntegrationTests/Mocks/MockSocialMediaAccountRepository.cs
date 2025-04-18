using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.IntegrationTests.Mocks
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
                _accounts.Add(account); // Note: In a real scenario, properties should be updated, not replace the object
                return Task.FromResult(account);
            }
            // Consider throwing a specific exception or returning a Result object
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
                account.UpdatedAt = DateTime.UtcNow; // Update timestamp
            }
            // Consider logging or returning a status if the account is not found
            return Task.CompletedTask;
        }

        // Método para verificar se existe um criador
        public Task<bool> CreatorExistsAsync(Guid creatorId)
        {
            // Para fins de teste, vamos considerar que qualquer ID de criador fornecido existe
            return Task.FromResult(true);
        }

        // Método para obter contas por plataforma (implementação simples para exemplo)
        public Task<IEnumerable<SocialMediaAccount>> GetByPlatformAsync(Guid creatorId, SocialMediaPlatform platform)
        {
             return Task.FromResult(_accounts.Where(a => a.CreatorId == creatorId && a.Platform == platform));
        }

        // Método para obter o total de seguidores (valor fixo para teste)
        public Task<int> GetTotalFollowersAsync()
        {
            // Retorna um valor fixo para simplificar o mock
            return Task.FromResult(1000);
        }

        // Método para obter o total de seguidores por criador (valor fixo para teste)
        public Task<int> GetTotalFollowersByCreatorAsync(Guid creatorId)
        {
             // Retorna um valor fixo associado ao creatorId padrão
            return Task.FromResult(creatorId == CREATOR_ID_1 ? 500 : 0);
        }

        // Método para atualizar métricas de redes sociais (apenas simulação)
        public Task RefreshSocialMediaMetricsAsync()
        {
            Console.WriteLine("Mock RefreshSocialMediaMetricsAsync called.");
            // Simula a atualização de métricas, sem ação real no mock
            return Task.CompletedTask;
        }
    }
} 