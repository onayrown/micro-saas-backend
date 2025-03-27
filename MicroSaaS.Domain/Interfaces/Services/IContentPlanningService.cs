using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Domain.Interfaces.Services;

public interface IContentPlanningService
{
    Task<ContentPost> CreatePostAsync(ContentPost post);
    Task<ContentPost> UpdatePostAsync(ContentPost post);
    Task DeletePostAsync(Guid id);
    Task<IEnumerable<ContentPost>> GetPostsByCreatorAsync(Guid creatorId);
    Task<IEnumerable<ContentPost>> GetPostsByStatusAsync(PostStatus status);
    Task<IEnumerable<ContentPost>> GetPostsByScheduledTimeRangeAsync(DateTime start, DateTime end);
} 