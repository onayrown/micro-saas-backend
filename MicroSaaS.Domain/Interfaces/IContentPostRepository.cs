using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Domain.Interfaces;

public interface IContentPostRepository
{
    Task<ContentPost> GetByIdAsync(Guid id);
    Task<List<ContentPost>> GetByCreatorIdAsync(Guid creatorId);
    Task<List<ContentPost>> GetByPlatformAsync(SocialMediaPlatform platform);
    Task<ContentPost> AddAsync(ContentPost post);
    Task<ContentPost> UpdateAsync(ContentPost post);
    Task DeleteAsync(Guid id);
} 