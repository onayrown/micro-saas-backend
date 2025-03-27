using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Application.Interfaces.Repositories;

public interface IContentPostRepository
{
    Task<ContentPost> GetByIdAsync(Guid id);
    Task<IEnumerable<ContentPost>> GetAllAsync();
    Task<ContentPost> AddAsync(ContentPost post);
    Task<ContentPost> UpdateAsync(ContentPost post);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<ContentPost>> GetByCreatorIdAsync(Guid creatorId);
    Task<IEnumerable<ContentPost>> GetByStatusAsync(PostStatus status);
    Task<IEnumerable<ContentPost>> GetByScheduledTimeRangeAsync(DateTime start, DateTime end);
    Task<IEnumerable<ContentPost>> GetScheduledPostsAsync(Guid creatorId);
    Task<IEnumerable<ContentPost>> GetScheduledByCreatorIdAsync(Guid creatorId);
} 