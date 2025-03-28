using MicroSaaS.Domain.Entities;
using MicroSaaS.Domain.Repositories;
using MicroSaaS.Infrastructure.Database;
using MicroSaaS.Infrastructure.Entities;
using MicroSaaS.Infrastructure.Mappers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.Repositories
{
    public class ContentCreatorRepository : IContentCreatorRepository
    {
        private readonly IMongoCollection<ContentCreatorEntity> _creators;

        public ContentCreatorRepository(IMongoDbContext context)
        {
            _creators = context.GetCollection<ContentCreatorEntity>("ContentCreators");
        }

        public async Task<ContentCreator> GetByIdAsync(Guid id)
        {
            var entity = await _creators.Find(x => x.Id == id).FirstOrDefaultAsync();
            return ContentCreatorMapper.ToDomain(entity);
        }

        public async Task<ContentCreator> GetByUserIdAsync(Guid userId)
        {
            var entity = await _creators.Find(x => x.UserId == userId).FirstOrDefaultAsync();
            return ContentCreatorMapper.ToDomain(entity);
        }

        public async Task<ContentCreator> GetByUsernameAsync(string username)
        {
            var entity = await _creators.Find(x => x.Username == username).FirstOrDefaultAsync();
            return ContentCreatorMapper.ToDomain(entity);
        }

        public async Task<IEnumerable<ContentCreator>> GetAllAsync()
        {
            var entities = await _creators.Find(_ => true).ToListAsync();
            return entities.Select(ContentCreatorMapper.ToDomain);
        }

        public async Task<ContentCreator> GetByEmailAsync(string email)
        {
            var entity = await _creators.Find(x => x.Email == email).FirstOrDefaultAsync();
            return ContentCreatorMapper.ToDomain(entity);
        }

        public async Task<ContentCreator> AddAsync(ContentCreator creator)
        {
            var entity = ContentCreatorMapper.ToEntity(creator);
            await _creators.InsertOneAsync(entity);
            return ContentCreatorMapper.ToDomain(entity);
        }

        public async Task<ContentCreator> UpdateAsync(ContentCreator creator)
        {
            var entity = ContentCreatorMapper.ToEntity(creator);
            var filter = Builders<ContentCreatorEntity>.Filter.Eq(x => x.Id, creator.Id);
            await _creators.ReplaceOneAsync(filter, entity);
            return ContentCreatorMapper.ToDomain(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var filter = Builders<ContentCreatorEntity>.Filter.Eq(x => x.Id, id);
            var result = await _creators.DeleteOneAsync(filter);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            var count = await _creators
                .CountDocumentsAsync(c => c.Username == username);

            return count == 0;
        }
    }
}
