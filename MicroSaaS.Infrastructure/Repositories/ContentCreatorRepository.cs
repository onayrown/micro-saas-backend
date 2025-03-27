using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Data;
using MongoDB.Driver;

namespace MicroSaaS.Infrastructure.Repositories;

public class ContentCreatorRepository : IContentCreatorRepository
{
    private readonly MongoDbContext _context;

    public ContentCreatorRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<ContentCreator> GetByIdAsync(Guid id)
    {
        return await _context.ContentCreators.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ContentCreator>> GetAllAsync()
    {
        return await _context.ContentCreators.Find(_ => true).ToListAsync();
    }

    public async Task<ContentCreator> AddAsync(ContentCreator creator)
    {
        await _context.ContentCreators.InsertOneAsync(creator);
        return creator;
    }

    public async Task<ContentCreator> UpdateAsync(ContentCreator creator)
    {
        var filter = Builders<ContentCreator>.Filter.Eq(c => c.Id, creator.Id);
        await _context.ContentCreators.ReplaceOneAsync(filter, creator);
        return creator;
    }

    public async Task DeleteAsync(Guid id)
    {
        var filter = Builders<ContentCreator>.Filter.Eq(c => c.Id, id);
        await _context.ContentCreators.DeleteOneAsync(filter);
    }

    public async Task<IEnumerable<ContentCreator>> GetByUserIdAsync(Guid userId)
    {
        return await _context.ContentCreators.Find(c => c.UserId == userId).ToListAsync();
    }
}
