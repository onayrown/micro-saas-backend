using MicroSaaS.Domain.Entities;

namespace MicroSaaS.Application.Interfaces.Repositories;

public interface IContentCreatorRepository
{
    Task<ContentCreator> GetByIdAsync(Guid id);
    Task<IEnumerable<ContentCreator>> GetAllAsync();
    Task<ContentCreator> AddAsync(ContentCreator creator);
    Task<ContentCreator> UpdateAsync(ContentCreator creator);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<ContentCreator>> GetByUserIdAsync(Guid userId);
} 