using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Data;
using MicroSaaS.Shared.Enums;
using MongoDB.Driver;

namespace MicroSaaS.Infrastructure.Repositories;

public class ContentPostRepository : IContentPostRepository
{
    private readonly MongoDbContext _context;

    public ContentPostRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<ContentPost> GetByIdAsync(Guid id)
    {
        return await _context.ContentPosts.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ContentPost>> GetAllAsync()
    {
        return await _context.ContentPosts.Find(_ => true).ToListAsync();
    }

    public async Task<ContentPost> AddAsync(ContentPost post)
    {
        await _context.ContentPosts.InsertOneAsync(post);
        return post;
    }

    public async Task<ContentPost> UpdateAsync(ContentPost post)
    {
        var filter = Builders<ContentPost>.Filter.Eq(p => p.Id, post.Id);
        await _context.ContentPosts.ReplaceOneAsync(filter, post);
        return post;
    }

    public async Task DeleteAsync(Guid id)
    {
        var filter = Builders<ContentPost>.Filter.Eq(p => p.Id, id);
        await _context.ContentPosts.DeleteOneAsync(filter);
    }

    public async Task<IEnumerable<ContentPost>> GetByCreatorIdAsync(Guid creatorId)
    {
        return await _context.ContentPosts.Find(p => p.CreatorId == creatorId).ToListAsync();
    }

    public async Task<IEnumerable<ContentPost>> GetByStatusAsync(PostStatus status)
    {
        return await _context.ContentPosts.Find(p => p.Status == status).ToListAsync();
    }

    public async Task<IEnumerable<ContentPost>> GetByScheduledTimeRangeAsync(DateTime start, DateTime end)
    {
        return await _context.ContentPosts.Find(p => p.ScheduledTime >= start && p.ScheduledTime <= end).ToListAsync();
    }

    public async Task<IEnumerable<ContentPost>> GetScheduledPostsAsync(Guid creatorId)
    {
        var filter = Builders<ContentPost>.Filter.And(
            Builders<ContentPost>.Filter.Eq(p => p.CreatorId, creatorId),
            Builders<ContentPost>.Filter.Eq(p => p.Status, PostStatus.Scheduled),
            Builders<ContentPost>.Filter.Gt(p => p.ScheduledTime, DateTime.UtcNow)
        );
        
        return await _context.ContentPosts.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<ContentPost>> GetScheduledByCreatorIdAsync(Guid creatorId)
    {
        var filter = Builders<ContentPost>.Filter.And(
            Builders<ContentPost>.Filter.Eq(p => p.CreatorId, creatorId),
            Builders<ContentPost>.Filter.Eq(p => p.Status, PostStatus.Scheduled)
        );
        
        return await _context.ContentPosts.Find(filter).ToListAsync();
    }
}
