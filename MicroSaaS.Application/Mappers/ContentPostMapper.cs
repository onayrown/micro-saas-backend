using MicroSaaS.Application.DTOs.ContentPost;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using System;

namespace MicroSaaS.Application.Mappers;

public static class ContentPostMapper
{
    public static ContentPostDto? ToDto(ContentPost? post)
    {
        if (post == null) return null;

        return new ContentPostDto
        {
            Id = post.Id,
            CreatorId = post.CreatorId,
            Title = post.Title,
            Content = post.Content,
            MediaUrl = post.MediaUrl ?? string.Empty,
            Platform = post.Platform,
            ScheduledTime = post.ScheduledTime?.TimeOfDay,
            ScheduledFor = post.ScheduledFor,
            Status = post.Status,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }

    public static ContentPost? ToEntity(CreatePostRequest? request)
    {
        if (request == null) return null;

        var now = DateTime.UtcNow;
        var scheduledTime = request.ScheduledTime.HasValue ? now.Date.Add(request.ScheduledTime.Value) : (DateTime?)null;

        return new ContentPost
        {
            Id = Guid.NewGuid(),
            CreatorId = request.CreatorId,
            Title = request.Title ?? "Novo Post",
            Content = request.Content,
            MediaUrl = request.MediaUrl,
            Platform = request.Platform,
            ScheduledTime = scheduledTime,
            ScheduledFor = request.ScheduledFor,
            Status = PostStatus.Scheduled,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public static void UpdateEntity(ContentPost post, UpdatePostRequest request)
    {
        if (post == null || request == null) return;

        post.Title = request.Title ?? post.Title;
        post.Content = request.Content ?? post.Content;
        post.MediaUrl = request.MediaUrl ?? post.MediaUrl;
        post.ScheduledTime = request.ScheduledTime.HasValue ? DateTime.UtcNow.Date.Add(request.ScheduledTime.Value) : post.ScheduledTime;
        post.ScheduledFor = request.ScheduledFor ?? post.ScheduledFor;
        post.Status = request.Status;
        post.UpdatedAt = DateTime.UtcNow;
    }
} 