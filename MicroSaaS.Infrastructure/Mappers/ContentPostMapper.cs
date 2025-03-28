using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Entities;
using System;

namespace MicroSaaS.Infrastructure.Mappers;

public static class ContentPostMapper
{
    public static ContentPost ToDomain(ContentPostEntity entity)
    {
        if (entity == null) return null;

        return new ContentPost
        {
            Id = entity.Id,
            CreatorId = entity.CreatorId,
            Title = entity.Title,
            Content = entity.Content,
            MediaUrl = entity.MediaUrl ?? string.Empty,
            Platform = entity.Platform,
            Status = entity.Status,
            ScheduledTime = entity.ScheduledTime,
            ScheduledFor = entity.ScheduledFor,
            PublishedAt = entity.PublishedAt,
            PostedTime = entity.PostedTime,
            Views = entity.Views,
            Likes = entity.Likes,
            Comments = entity.Comments,
            Shares = entity.Shares,
            EngagementRate = entity.EngagementRate,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static ContentPostEntity ToEntity(ContentPost domain)
    {
        if (domain == null) return null;

        return new ContentPostEntity
        {
            Id = domain.Id,
            CreatorId = domain.CreatorId,
            Title = domain.Title,
            Content = domain.Content,
            MediaUrl = domain.MediaUrl,
            Platform = domain.Platform,
            Status = domain.Status,
            ScheduledTime = domain.ScheduledTime,
            ScheduledFor = domain.ScheduledFor,
            PublishedAt = domain.PublishedAt,
            PostedTime = domain.PostedTime,
            Views = domain.Views,
            Likes = domain.Likes,
            Comments = domain.Comments,
            Shares = domain.Shares,
            EngagementRate = domain.EngagementRate,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }
} 