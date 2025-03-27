using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using MongoDB.Driver;

namespace MicroSaaS.Infrastructure.Repositories;

public class SocialMediaAccountRepository : ISocialMediaAccountRepository
{
    private readonly IMongoCollection<SocialMediaAccount> _collection;

    public SocialMediaAccountRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<SocialMediaAccount>("social_media_accounts");
    }

    public async Task<SocialMediaAccount?> GetByIdAsync(Guid id)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<SocialMediaAccount>> GetByCreatorIdAsync(Guid creatorId)
    {
        return await _collection.Find(x => x.CreatorId == creatorId).ToListAsync();
    }

    public async Task<IEnumerable<SocialMediaAccount>> GetByPlatformAsync(Guid creatorId, SocialMediaPlatform platform)
    {
        return await _collection.Find(x => x.CreatorId == creatorId && x.Platform == platform).ToListAsync();
    }

    public async Task<SocialMediaAccount> AddAsync(SocialMediaAccount account)
    {
        await _collection.InsertOneAsync(account);
        return account;
    }

    public async Task UpdateAsync(SocialMediaAccount account)
    {
        await _collection.ReplaceOneAsync(x => x.Id == account.Id, account);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
    }

    public async Task UpdateTokenAsync(Guid id, string accessToken, string refreshToken, DateTime expiresAt)
    {
        var update = Builders<SocialMediaAccount>.Update
            .Set(x => x.AccessToken, accessToken)
            .Set(x => x.RefreshToken, refreshToken)
            .Set(x => x.TokenExpiresAt, expiresAt)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        await _collection.UpdateOneAsync(x => x.Id == id, update);
    }
} 