using MicroSaaS.Domain.Entities;

namespace MicroSaaS.Application.Interfaces.Repositories;

public interface IContentPostRepository
{
    Task<ContentPost> GetByIdAsync(Guid id);
    Task<List<ContentPost>> GetScheduledPostsAsync(Guid creatorId);
    Task<ContentPost> AddAsync(ContentPost post);
    Task UpdateAsync(ContentPost post);
    Task DeleteAsync(Guid id);
}
