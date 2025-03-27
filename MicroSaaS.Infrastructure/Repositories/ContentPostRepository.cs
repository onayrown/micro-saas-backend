using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Data;
using MicroSaaS.Infrastructure.Entities;
using MicroSaaS.Infrastructure.Mappers;
using MicroSaaS.Shared.Enums;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.Repositories;

public class ContentPostRepository : IContentPostRepository
{
    private readonly IMongoCollection<ContentPostEntity> _contentPosts;
    private readonly IContentCreatorRepository _creatorRepository;

    public ContentPostRepository(MongoDbContext context, IContentCreatorRepository creatorRepository)
    {
        _contentPosts = context.ContentPosts;
        _creatorRepository = creatorRepository;
    }

    public async Task<ContentPost> GetByIdAsync(Guid id)
    {
        var idString = id.ToString();
        var entity = await _contentPosts
            .Find(p => p.Id == idString)
            .FirstOrDefaultAsync();

        if (entity == null)
            return null;

        // Converter para domínio
        var post = entity.ToDomain();
        
        // Buscar o Creator se tivermos um CreatorId
        if (!string.IsNullOrEmpty(entity.CreatorId) && Guid.TryParse(entity.CreatorId, out var creatorId))
        {
            post.Creator = await _creatorRepository.GetByIdAsync(creatorId);
        }
        
        return post;
    }

    public async Task<List<ContentPost>> GetScheduledPostsAsync(Guid creatorId)
    {
        var creatorIdString = creatorId.ToString();
        var entities = await _contentPosts
            .Find(p => p.CreatorId == creatorIdString && p.Status == PostStatus.Scheduled)
            .ToListAsync();

        var posts = new List<ContentPost>();
        foreach (var entity in entities)
        {
            var post = entity.ToDomain();
            post.Creator = await _creatorRepository.GetByIdAsync(creatorId);
            posts.Add(post);
        }

        return posts;
    }

    public async Task<ContentPost> AddAsync(ContentPost post)
    {
        var entity = post.ToEntity();
        await _contentPosts.InsertOneAsync(entity);
        
        // Retornar o post com o mesmo Creator
        var savedPost = entity.ToDomain();
        savedPost.Creator = post.Creator;
        return savedPost;
    }

    public async Task UpdateAsync(ContentPost post)
    {
        var entity = post.ToEntity();
        await _contentPosts
            .ReplaceOneAsync(p => p.Id == entity.Id, entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var idString = id.ToString();
        await _contentPosts
            .DeleteOneAsync(p => p.Id == idString);
    }
}
