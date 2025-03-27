using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Entities;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Enums;
using System;

namespace MicroSaaS.Infrastructure.Mappers;

public static class ContentPostMapper
{
    public static ContentPostEntity? ToEntity(this ContentPost post)
    {
        if (post == null) return null;

        return new ContentPostEntity
        {
            Id = post.Id,
            CreatorId = post.CreatorId,
            Title = post.Title,
            Content = post.Content,
            MediaUrl = post.MediaUrl,
            Platform = post.Platform,
            Status = post.Status,
            ScheduledTime = post.ScheduledTime,
            ScheduledFor = post.ScheduledFor,
            PublishedAt = post.PublishedAt,
            PostedTime = post.PostedTime,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }

    public static ContentPost? ToDomain(this ContentPostEntity entity)
    {
        if (entity == null) return null;

        return new ContentPost
        {
            Id = entity.Id,
            CreatorId = entity.CreatorId,
            Title = entity.Title ?? "Sem título",
            Content = entity.Content,
            MediaUrl = entity.MediaUrl,
            Platform = entity.Platform,
            Status = entity.Status,
            ScheduledTime = entity.ScheduledTime,
            ScheduledFor = entity.ScheduledFor,
            PublishedAt = entity.PublishedAt,
            PostedTime = entity.PostedTime,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Creator = null // Isso precisará ser preenchido pelo repositório
        };
    }

    public static ContentPost ToEntity(CreatePostRequest request)
    {
        return new ContentPost
        {
            Id = Guid.NewGuid(),
            CreatorId = request.CreatorId,
            Title = request.Title ?? "Novo Post",
            Content = request.Content,
            MediaUrl = request.MediaUrl,
            Platform = request.Platform,
            ScheduledTime = request.ScheduledTime ?? DateTime.UtcNow.AddDays(1),
            Status = PostStatus.Scheduled,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Creator = null // Isso precisará ser preenchido pelo repositório
        };
    }

    public static ContentPost ToEntity(UpdatePostRequest request, ContentPost existingPost)
    {
        existingPost.Title = request.Title ?? existingPost.Title;
        existingPost.Content = request.Content;
        existingPost.MediaUrl = request.MediaUrl;
        existingPost.ScheduledTime = request.ScheduledTime ?? existingPost.ScheduledTime;
        existingPost.Status = request.Status;
        existingPost.UpdatedAt = DateTime.UtcNow;
        return existingPost;
    }

    public static PostDto ToDto(ContentPost post)
    {
        return new PostDto
        {
            Id = post.Id,
            CreatorId = post.CreatorId,
            Title = post.Title,
            Content = post.Content,
            MediaUrl = post.MediaUrl,
            Platform = post.Platform,
            ScheduledTime = post.ScheduledTime,
            Status = post.Status,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }
} 