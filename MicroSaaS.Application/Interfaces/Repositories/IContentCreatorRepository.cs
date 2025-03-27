using MicroSaaS.Domain.Entities;

namespace MicroSaaS.Application.Interfaces.Repositories;

public interface IContentCreatorRepository
{
    Task<ContentCreator> GetByIdAsync(Guid id);
    Task<ContentCreator> AddAsync(ContentCreator creator);
    Task UpdateAsync(ContentCreator creator);
    Task DeleteAsync(Guid id);
}
