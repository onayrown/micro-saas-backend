using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Entities;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroSaaS.Infrastructure.Mappers;

public static class ContentChecklistMapper
{
    public static ContentChecklistEntity? ToEntity(this ContentChecklist checklist)
    {
        if (checklist == null) return null;

        return new ContentChecklistEntity
        {
            Id = checklist.Id.ToString(),
            CreatorId = checklist.CreatorId.ToString(),
            Title = checklist.Title,
            Description = checklist.Description,
            Status = checklist.Status,
            Items = checklist.Items?.Select(i => new ChecklistItemEntity
            {
                Id = i.Id.ToString(),
                Title = i.Title,
                Description = i.Description,
                IsCompleted = i.IsCompleted,
                CompletedAt = i.CompletedAt
            }).ToList() ?? new List<ChecklistItemEntity>(),
            CreatedAt = checklist.CreatedAt,
            UpdatedAt = checklist.UpdatedAt,
            CompletedAt = checklist.CompletedAt
        };
    }

    public static ContentChecklist? ToDomain(this ContentChecklistEntity entity)
    {
        if (entity == null) return null;

        return new ContentChecklist
        {
            Id = string.IsNullOrEmpty(entity.Id) ? Guid.Empty : Guid.Parse(entity.Id),
            CreatorId = string.IsNullOrEmpty(entity.CreatorId) ? Guid.Empty : Guid.Parse(entity.CreatorId),
            Title = entity.Title,
            Description = entity.Description,
            Status = entity.Status,
            Items = entity.Items?.Select(i => new ChecklistItem
            {
                Id = string.IsNullOrEmpty(i.Id) ? Guid.Empty : Guid.Parse(i.Id),
                Title = i.Title,
                Description = i.Description,
                IsCompleted = i.IsCompleted,
                CompletedAt = i.CompletedAt
            }).ToList() ?? new List<ChecklistItem>(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            CompletedAt = entity.CompletedAt,
            Creator = null // Isso precisará ser preenchido pelo repositório
        };
    }
} 