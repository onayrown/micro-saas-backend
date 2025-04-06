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
using Microsoft.Extensions.Logging;

namespace MicroSaaS.Infrastructure.Repositories;

public class ContentPostRepository : IContentPostRepository
{
    private readonly IMongoCollection<ContentPostEntity> _contentPosts;
    private readonly ILogger<ContentPostRepository>? _logger;

    public ContentPostRepository(IMongoDbContext context, ILogger<ContentPostRepository>? logger = null)
    {
        _contentPosts = context.GetCollection<ContentPostEntity>(CollectionNames.ContentPosts);
        _logger = logger;
    }

    public async Task<ContentPost> GetByIdAsync(Guid id)
    {
        var filter = Builders<ContentPostEntity>.Filter.Eq(x => x.Id, id);
        var entity = await _contentPosts.Find(filter).FirstOrDefaultAsync();
        return ContentPostMapper.ToDomain(entity);
    }

    public async Task<IEnumerable<ContentPost>> GetByCreatorIdAsync(Guid creatorId)
    {
        var filter = Builders<ContentPostEntity>.Filter.Eq(x => x.CreatorId, creatorId);
        var entities = await _contentPosts.Find(filter).ToListAsync();
        return entities.Select(ContentPostMapper.ToDomain);
    }

    public async Task<IEnumerable<ContentPost>> GetByPlatformAsync(SocialMediaPlatform platform)
    {
        var filter = Builders<ContentPostEntity>.Filter.Eq(x => x.Platform, platform);
        var entities = await _contentPosts.Find(filter).ToListAsync();
        return entities.Select(ContentPostMapper.ToDomain);
    }

    public async Task<ContentPost> AddAsync(ContentPost contentPost)
    {
        var entity = ContentPostMapper.ToEntity(contentPost);
        await _contentPosts.InsertOneAsync(entity);
        return ContentPostMapper.ToDomain(entity);
    }

    public async Task<ContentPost> UpdateAsync(ContentPost contentPost)
    {
        var entity = ContentPostMapper.ToEntity(contentPost);
        var filter = Builders<ContentPostEntity>.Filter.Eq(x => x.Id, contentPost.Id);
        await _contentPosts.ReplaceOneAsync(filter, entity);
        return ContentPostMapper.ToDomain(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var filter = Builders<ContentPostEntity>.Filter.Eq(x => x.Id, id);
        var result = await _contentPosts.DeleteOneAsync(filter);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    public async Task<int> CountByCreatorAsync(Guid creatorId)
    {
        var filter = Builders<ContentPostEntity>.Filter.Eq(x => x.CreatorId, creatorId);
        return (int)await _contentPosts.CountDocumentsAsync(filter);
    }

    public async Task<IEnumerable<ContentPost>> GetAllAsync()
    {
        var posts = await _contentPosts.Find(_ => true).ToListAsync();
        return posts.Select(ContentPostMapper.ToDomain);
    }

    public async Task<IEnumerable<ContentPost>> GetByStatusAsync(PostStatus status)
    {
        var posts = await _contentPosts.Find(p => p.Status == status).ToListAsync();
        return posts.Select(ContentPostMapper.ToDomain);
    }

    public async Task<IEnumerable<ContentPost>> GetScheduledByCreatorIdAsync(Guid creatorId, DateTime? from = null, DateTime? to = null)
    {
        var builder = Builders<ContentPostEntity>.Filter;
        var filter = builder.Eq(p => p.CreatorId, creatorId) & 
                     builder.Eq(p => p.Status, PostStatus.Scheduled);

        if (from.HasValue)
        {
            filter = filter & builder.Gte(p => p.ScheduledFor, from.Value);
        }

        if (to.HasValue)
        {
            filter = filter & builder.Lte(p => p.ScheduledFor, to.Value);
        }

        var posts = await _contentPosts.Find(filter).ToListAsync();
        return posts.Select(ContentPostMapper.ToDomain);
    }

    public async Task<bool> UpdateStatusAsync(Guid id, PostStatus status)
    {
        var update = Builders<ContentPostEntity>.Update
            .Set(p => p.Status, status)
            .Set(p => p.UpdatedAt, DateTime.UtcNow);

        if (status == PostStatus.Published)
        {
            update = update.Set(p => p.PublishedAt, DateTime.UtcNow);
        }

        var result = await _contentPosts.UpdateOneAsync(
            p => p.Id == id,
            update);

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<IEnumerable<ContentPost>> GetByCreatorIdBetweenDatesAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var filter = Builders<ContentPostEntity>.Filter.And(
                Builders<ContentPostEntity>.Filter.Eq(p => p.CreatorId, creatorId),
                Builders<ContentPostEntity>.Filter.Gte(p => p.PublishedAt, startDate),
                Builders<ContentPostEntity>.Filter.Lte(p => p.PublishedAt, endDate)
            );

            var entities = await _contentPosts.Find(filter).ToListAsync();
            return entities.Select(ContentPostMapper.ToDomain);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Erro ao buscar posts do criador {creatorId} entre {startDate} e {endDate}");
            return Enumerable.Empty<ContentPost>();
        }
    }
}
