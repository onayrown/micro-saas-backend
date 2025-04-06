using MicroSaaS.Domain.Entities;
using MicroSaaS.Domain.Repositories;
using MicroSaaS.Infrastructure.Database;
using MicroSaaS.Infrastructure.Entities;
using MicroSaaS.Infrastructure.Mappers;
using MicroSaaS.Shared.Enums;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.Repositories
{
    public class SocialMediaAccountRepository : ISocialMediaAccountRepository
    {
        private readonly IMongoCollection<SocialMediaAccountEntity> _accounts;

        public SocialMediaAccountRepository(IMongoDbContext context)
        {
            _accounts = context.GetCollection<SocialMediaAccountEntity>(CollectionNames.SocialMediaAccounts);
        }

        public async Task<SocialMediaAccount> GetByIdAsync(Guid id)
        {
            var entity = await _accounts.Find(x => x.Id == id).FirstOrDefaultAsync();
            return SocialMediaAccountMapper.ToDomain(entity);
        }

        public async Task<IEnumerable<SocialMediaAccount>> GetAllAsync()
        {
            var accounts = await _accounts
                .Find(_ => true)
                .ToListAsync();

            return accounts.Select(SocialMediaAccountMapper.ToDomain);
        }

        public async Task<SocialMediaAccount> AddAsync(SocialMediaAccount account)
        {
            var entity = SocialMediaAccountMapper.ToEntity(account);
            await _accounts.InsertOneAsync(entity);
            return SocialMediaAccountMapper.ToDomain(entity);
        }

        public async Task<SocialMediaAccount> UpdateAsync(SocialMediaAccount account)
        {
            var entity = SocialMediaAccountMapper.ToEntity(account);
            var filter = Builders<SocialMediaAccountEntity>.Filter.Eq(x => x.Id, account.Id);
            await _accounts.ReplaceOneAsync(filter, entity);
            return SocialMediaAccountMapper.ToDomain(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var filter = Builders<SocialMediaAccountEntity>.Filter.Eq(x => x.Id, id);
            var result = await _accounts.DeleteOneAsync(filter);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<IEnumerable<SocialMediaAccount>> GetByCreatorIdAsync(Guid creatorId)
        {
            var entities = await _accounts.Find(x => x.CreatorId == creatorId).ToListAsync();
            return entities.Select(SocialMediaAccountMapper.ToDomain);
        }

        public async Task<SocialMediaAccount> GetByCreatorIdAndPlatformAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            var filter = Builders<SocialMediaAccountEntity>.Filter.And(
                Builders<SocialMediaAccountEntity>.Filter.Eq(x => x.Platform, platform),
                Builders<SocialMediaAccountEntity>.Filter.Eq(x => x.CreatorId, creatorId)
            );
            var entity = await _accounts.Find(filter).FirstOrDefaultAsync();
            return SocialMediaAccountMapper.ToDomain(entity);
        }

        public async Task UpdateTokenAsync(Guid id, string accessToken, string refreshToken, DateTime expiresAt)
        {
            var update = Builders<SocialMediaAccountEntity>.Update
                .Set(x => x.AccessToken, accessToken)
                .Set(x => x.RefreshToken, refreshToken)
                .Set(x => x.TokenExpiresAt, expiresAt)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            var filter = Builders<SocialMediaAccountEntity>.Filter.Eq(x => x.Id, id);
            await _accounts.UpdateOneAsync(filter, update);
        }

        public async Task<int> GetTotalFollowersByCreatorAsync(Guid creatorId)
        {
            var accounts = await _accounts.Find(x => x.CreatorId == creatorId).ToListAsync();
            return accounts.Sum(a => a.FollowersCount);
        }

        public async Task RefreshSocialMediaMetricsAsync()
        {
            var accounts = await _accounts.Find(_ => true).ToListAsync();
            foreach (var account in accounts)
            {
                // Implementar lógica de atualização de métricas
                account.UpdatedAt = DateTime.UtcNow;
                await _accounts.ReplaceOneAsync(x => x.Id == account.Id, account);
            }
        }
    }
} 