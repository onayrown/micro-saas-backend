using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Entities;
using System;

namespace MicroSaaS.Infrastructure.Mappers;

public static class ContentPostMapper
{
    public static ContentPostEntity ToEntity(this ContentPost post)
    {
        if (post == null) return null;

        return new ContentPostEntity
        {
            Id = post.Id.ToString(),
            CreatorId = post.Creator?.Id.ToString(),
            Platform = post.Platform,
            Content = post.Content,
            ScheduledTime = post.ScheduledTime,
            PostedTime = post.PostedTime,
            Status = post.Status
        };
    }

    public static ContentPost ToDomain(this ContentPostEntity entity)
    {
        if (entity == null) return null;

        return new ContentPost
        {
            Id = string.IsNullOrEmpty(entity.Id) ? Guid.Empty : Guid.Parse(entity.Id),
            Platform = entity.Platform,
            Content = entity.Content,
            ScheduledTime = entity.ScheduledTime,
            PostedTime = entity.PostedTime,
            Status = entity.Status,
            Creator = null // Isso precisará ser preenchido pelo repositório
        };
    }
} 