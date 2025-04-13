using MicroSaaS.Domain.Entities;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Infrastructure.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.Persistence.Repositories
{
    public class ContentCreatorRepository : IContentCreatorRepository
    {
        private readonly IMongoCollection<ContentCreator> _creators;

        public ContentCreatorRepository(IMongoDbContext context)
        {
            _creators = context.ContentCreators;
        }

        public async Task<ContentCreator> GetByIdAsync(Guid id)
        {
            return await _creators.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ContentCreator>> GetByUserIdAsync(Guid userId)
        {
            return await _creators.Find(x => x.UserId == userId).ToListAsync();
        }

        public async Task<ContentCreator?> GetByUsernameAsync(string username)
        {
            return await _creators.Find(x => x.Username == username).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ContentCreator>> GetAllAsync()
        {
            return await _creators.Find(_ => true).ToListAsync();
        }

        public async Task<ContentCreator> GetByEmailAsync(string email)
        {
            return await _creators.Find(x => x.Email == email).FirstOrDefaultAsync();
        }

        public async Task<ContentCreator> AddAsync(ContentCreator creator)
        {
            await _creators.InsertOneAsync(creator);
            return creator;
        }

        public async Task<ContentCreator> UpdateAsync(ContentCreator creator)
        {
            var filter = Builders<ContentCreator>.Filter.Eq(x => x.Id, creator.Id);
            var result = await _creators.ReplaceOneAsync(filter, creator);
            return result.IsAcknowledged ? creator : null;
        }

        public async Task DeleteAsync(Guid id)
        {
            var filter = Builders<ContentCreator>.Filter.Eq(x => x.Id, id);
            await _creators.DeleteOneAsync(filter);
        }

        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            var count = await _creators
                .CountDocumentsAsync(c => c.Username == username);

            return count == 0;
        }

        public async Task<int> CountAsync()
        {
            return (int)await _creators.CountDocumentsAsync(_ => true);
        }
    }
} 