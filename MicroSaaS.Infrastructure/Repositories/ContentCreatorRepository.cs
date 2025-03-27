using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Data;
using MicroSaaS.Infrastructure.Entities;
using MicroSaaS.Infrastructure.Mappers;
using MongoDB.Driver;

namespace MicroSaaS.Infrastructure.Repositories;

public class ContentCreatorRepository : IContentCreatorRepository
{
    private readonly IMongoCollection<ContentCreatorEntity> _contentCreators;

    public ContentCreatorRepository(MongoDbContext context)
    {
        _contentCreators = context.ContentCreators;
    }

    public async Task<ContentCreator> GetByIdAsync(Guid id)
    {
        var entity = await _contentCreators
            .Find(c => c.Id == id.ToString())
            .FirstOrDefaultAsync();
        
        return entity?.ToDomain();
    }

    public async Task<ContentCreator> AddAsync(ContentCreator creator)
    {
        var entity = creator.ToEntity();
        await _contentCreators.InsertOneAsync(entity);
        return entity.ToDomain();
    }

    public async Task UpdateAsync(ContentCreator creator)
    {
        var entity = creator.ToEntity();
        await _contentCreators
            .ReplaceOneAsync(c => c.Id == entity.Id, entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _contentCreators
            .DeleteOneAsync(c => c.Id == id.ToString());
    }
}
