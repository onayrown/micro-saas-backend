using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.MongoDB;
using MicroSaaS.Shared.Enums;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.Persistence.Repositories
{
    public class SocialMediaAccountRepository : ISocialMediaAccountRepository
    {
        private readonly IMongoCollection<SocialMediaAccount> _collection;
        private readonly IMongoCollection<ContentCreator> _creatorCollection; // Para CreatorExistsAsync

        public SocialMediaAccountRepository(IMongoDbContext context)
        {
            _collection = context.GetCollection<SocialMediaAccount>("SocialMediaAccounts");
            _creatorCollection = context.GetCollection<ContentCreator>("ContentCreators");
        }

        public Task<SocialMediaAccount?> GetByIdAsync(Guid id)
        {
            var filter = Builders<SocialMediaAccount>.Filter.Eq(a => a.Id, id);
            return _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SocialMediaAccount>> GetByCreatorIdAsync(Guid creatorId)
        {
            var filter = Builders<SocialMediaAccount>.Filter.Eq(a => a.CreatorId, creatorId);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<SocialMediaAccount>> GetByPlatformAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            var filter = Builders<SocialMediaAccount>.Filter.And(
                Builders<SocialMediaAccount>.Filter.Eq(a => a.CreatorId, creatorId),
                Builders<SocialMediaAccount>.Filter.Eq(a => a.Platform, platform)
            );
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<SocialMediaAccount> AddAsync(SocialMediaAccount account)
        {
            if (account.Id == Guid.Empty) account.Id = Guid.NewGuid();
            account.CreatedAt = DateTime.UtcNow;
            account.UpdatedAt = DateTime.UtcNow;
            await _collection.InsertOneAsync(account);
            return account;
        }

        public async Task<SocialMediaAccount> UpdateAsync(SocialMediaAccount account)
        {
             var filter = Builders<SocialMediaAccount>.Filter.Eq(a => a.Id, account.Id);
             account.UpdatedAt = DateTime.UtcNow;
             await _collection.ReplaceOneAsync(filter, account);
             return account;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
             var filter = Builders<SocialMediaAccount>.Filter.Eq(a => a.Id, id);
             var result = await _collection.DeleteOneAsync(filter);
             return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public Task UpdateTokenAsync(Guid id, string accessToken, string refreshToken, DateTime expiresAt)
        {
            var filter = Builders<SocialMediaAccount>.Filter.Eq(a => a.Id, id);
            var update = Builders<SocialMediaAccount>.Update
                .Set(a => a.AccessToken, accessToken)
                .Set(a => a.RefreshToken, refreshToken)
                .Set(a => a.TokenExpiresAt, expiresAt)
                .Set(a => a.UpdatedAt, DateTime.UtcNow);
            return _collection.UpdateOneAsync(filter, update);
        }

        public Task<int> GetTotalFollowersAsync()
        {
            // Implementação placeholder - precisa de agregação
            throw new NotImplementedException();
        }

        public Task<int> GetTotalFollowersByCreatorAsync(Guid creatorId)
        {
            // Implementação placeholder - precisa de agregação
            throw new NotImplementedException();
        }

        public Task RefreshSocialMediaMetricsAsync()
        {
            // Lógica de atualização de métricas (pode chamar serviços externos)
            throw new NotImplementedException();
        }

        public async Task<bool> CreatorExistsAsync(Guid creatorId)
        {
            // Verifica se um criador com o ID existe na coleção de ContentCreators
            var filter = Builders<ContentCreator>.Filter.Eq(c => c.Id, creatorId);
            var count = await _creatorCollection.CountDocumentsAsync(filter);
            return count > 0;
        }
    }
} 