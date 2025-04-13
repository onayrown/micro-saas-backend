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
    public class ContentPostRepository : IContentPostRepository
    {
        private readonly IMongoCollection<ContentPost> _collection;

        public ContentPostRepository(IMongoDbContext context)
        {
            _collection = context.GetCollection<ContentPost>("ContentPosts");
        }

        public async Task<ContentPost> GetByIdAsync(Guid id)
        {
            var filter = Builders<ContentPost>.Filter.Eq(p => p.Id, id);
            var post = await _collection.Find(filter).FirstOrDefaultAsync();
            if (post == null)
            {
                throw new KeyNotFoundException($"ContentPost with id {id} not found.");
            }
            return post;
        }

        public async Task<IEnumerable<ContentPost>> GetAllAsync()
        {
             return await _collection.Find(Builders<ContentPost>.Filter.Empty).ToListAsync();
        }

        public async Task<ContentPost> AddAsync(ContentPost post)
        {
            if (post.Id == Guid.Empty) post.Id = Guid.NewGuid();
            post.CreatedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;
            await _collection.InsertOneAsync(post);
            return post;
        }

        public async Task<ContentPost> UpdateAsync(ContentPost post)
        {
             var filter = Builders<ContentPost>.Filter.Eq(p => p.Id, post.Id);
             post.UpdatedAt = DateTime.UtcNow;
             var result = await _collection.ReplaceOneAsync(filter, post);
             if (result.MatchedCount == 0)
             {
                 throw new KeyNotFoundException($"ContentPost with id {post.Id} not found for update.");
             }
             return post;
        }

        public async Task DeleteAsync(Guid id)
        {
            var filter = Builders<ContentPost>.Filter.Eq(p => p.Id, id);
            var result = await _collection.DeleteOneAsync(filter);
        }

        public async Task<IEnumerable<ContentPost>> GetByCreatorIdAsync(Guid creatorId)
        {
            var filter = Builders<ContentPost>.Filter.Eq(p => p.CreatorId, creatorId);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<ContentPost>> GetByStatusAsync(PostStatus status)
        {
            var filter = Builders<ContentPost>.Filter.Eq(p => p.Status, status);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<ContentPost>> GetByPlatformAsync(SocialMediaPlatform platform)
        {
            var filter = Builders<ContentPost>.Filter.Eq(p => p.Platform, platform);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<ContentPost>> GetByScheduledTimeRangeAsync(DateTime start, DateTime end)
        {
            var filter = Builders<ContentPost>.Filter.And(
                Builders<ContentPost>.Filter.Gte(p => p.ScheduledTime, start),
                Builders<ContentPost>.Filter.Lte(p => p.ScheduledTime, end)
            );
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<ContentPost>> GetScheduledPostsAsync(Guid creatorId)
        {
            var filter = Builders<ContentPost>.Filter.And(
                Builders<ContentPost>.Filter.Eq(p => p.CreatorId, creatorId),
                Builders<ContentPost>.Filter.Eq(p => p.Status, PostStatus.Scheduled)
            );
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<ContentPost>> GetScheduledByCreatorIdAsync(Guid creatorId, DateTime? from = null, DateTime? to = null)
        {
            var filterBuilder = Builders<ContentPost>.Filter;
            var baseFilter = filterBuilder.And(
                filterBuilder.Eq(p => p.CreatorId, creatorId),
                filterBuilder.Eq(p => p.Status, PostStatus.Scheduled)
            );

            if (from.HasValue && to.HasValue)
            {
                var dateFilter = filterBuilder.And(
                    filterBuilder.Gte(p => p.ScheduledTime, from.Value),
                    filterBuilder.Lte(p => p.ScheduledTime, to.Value)
                );
                return await _collection.Find(filterBuilder.And(baseFilter, dateFilter)).ToListAsync();
            }
            else if (from.HasValue)
            {
                var dateFilter = filterBuilder.Gte(p => p.ScheduledTime, from.Value);
                return await _collection.Find(filterBuilder.And(baseFilter, dateFilter)).ToListAsync();
            }
            else if (to.HasValue)
            {
                var dateFilter = filterBuilder.Lte(p => p.ScheduledTime, to.Value);
                return await _collection.Find(filterBuilder.And(baseFilter, dateFilter)).ToListAsync();
            }

            return await _collection.Find(baseFilter).ToListAsync();
        }

        public async Task<bool> UpdateStatusAsync(Guid id, PostStatus status)
        {
            var filter = Builders<ContentPost>.Filter.Eq(p => p.Id, id);
            var update = Builders<ContentPost>.Update
                .Set(p => p.Status, status)
                .Set(p => p.UpdatedAt, DateTime.UtcNow);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<int> CountAsync()
        {
            var count = await _collection.CountDocumentsAsync(Builders<ContentPost>.Filter.Empty);
            return (int)count; // MongoDB retorna long
        }

        public async Task<int> CountByCreatorAsync(Guid creatorId)
        {
             var filter = Builders<ContentPost>.Filter.Eq(p => p.CreatorId, creatorId);
             var count = await _collection.CountDocumentsAsync(filter);
             return (int)count;
        }

        public async Task<IEnumerable<ContentPost>> GetByCreatorIdBetweenDatesAsync(Guid creatorId, DateTime startDate, DateTime endDate)
        {
            var filter = Builders<ContentPost>.Filter.And(
                Builders<ContentPost>.Filter.Eq(p => p.CreatorId, creatorId),
                Builders<ContentPost>.Filter.Gte(p => p.CreatedAt, startDate),
                Builders<ContentPost>.Filter.Lte(p => p.CreatedAt, endDate)
            );
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<ContentPost>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            var filter = Builders<ContentPost>.Filter.In(p => p.Id, ids);
            return await _collection.Find(filter).ToListAsync();
        }
    }
} 