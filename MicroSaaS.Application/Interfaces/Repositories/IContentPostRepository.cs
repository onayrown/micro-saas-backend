using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Application.Interfaces.Repositories;

// Mesclando definições do Domain e Application
public interface IContentPostRepository
{
    Task<ContentPost> GetByIdAsync(Guid id);
    Task<IEnumerable<ContentPost>> GetAllAsync();
    Task<ContentPost> AddAsync(ContentPost post);
    Task<ContentPost> UpdateAsync(ContentPost post);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<ContentPost>> GetByCreatorIdAsync(Guid creatorId);
    Task<IEnumerable<ContentPost>> GetByStatusAsync(PostStatus status);
    Task<IEnumerable<ContentPost>> GetByPlatformAsync(SocialMediaPlatform platform); // Adicionado do Domain
    Task<IEnumerable<ContentPost>> GetByScheduledTimeRangeAsync(DateTime start, DateTime end);
    Task<IEnumerable<ContentPost>> GetScheduledPostsAsync(Guid creatorId);
    Task<IEnumerable<ContentPost>> GetScheduledByCreatorIdAsync(Guid creatorId, DateTime? from = null, DateTime? to = null); // Usar versão do Domain com datas?
    Task<bool> UpdateStatusAsync(Guid id, PostStatus status); // Adicionado do Domain
    Task<int> CountAsync();
    Task<int> CountByCreatorAsync(Guid creatorId);
    Task<IEnumerable<ContentPost>> GetByCreatorIdBetweenDatesAsync(Guid creatorId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<ContentPost>> GetByIdsAsync(IEnumerable<Guid> ids);
} 