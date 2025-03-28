using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Domain.Repositories
{
    public interface IContentPostRepository
    {
        Task<ContentPost> GetByIdAsync(Guid id);
        Task<IEnumerable<ContentPost>> GetAllAsync();
        Task<IEnumerable<ContentPost>> GetByCreatorIdAsync(Guid creatorId);
        Task<IEnumerable<ContentPost>> GetByStatusAsync(PostStatus status);
        Task<IEnumerable<ContentPost>> GetByPlatformAsync(SocialMediaPlatform platform);
        Task<IEnumerable<ContentPost>> GetScheduledByCreatorIdAsync(Guid creatorId, DateTime? from = null, DateTime? to = null);
        Task<ContentPost> AddAsync(ContentPost post);
        Task<ContentPost> UpdateAsync(ContentPost post);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> UpdateStatusAsync(Guid id, PostStatus status);
    }
} 