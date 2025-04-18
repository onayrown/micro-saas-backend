using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Results;
using MicroSaaS.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Application.Interfaces.Services;

public interface IContentPlanningService
{
    Task<ContentPost> CreatePostAsync(ContentPost post);
    Task<ContentPost> UpdatePostAsync(ContentPost post);
    Task DeletePostAsync(Guid id);
    Task<IEnumerable<ContentPost>> GetPostsByCreatorAsync(Guid creatorId);
    Task<IEnumerable<ContentPost>> GetPostsByStatusAsync(PostStatus status);
    Task<IEnumerable<ContentPost>> GetPostsByScheduledTimeRangeAsync(DateTime start, DateTime end);
    Task<List<DateTime>> SuggestPostTimesAsync(SocialMediaPlatform platform, Guid creatorId);
    Task<ContentChecklistDto> GetChecklistByIdAsync(Guid id);
    Task<List<ContentChecklistDto>> GetChecklistsByCreatorIdAsync(Guid creatorId);
    Task<bool> DeleteChecklistAsync(Guid id);
    Task<ContentChecklistDto> CreateChecklistAsync(Guid creatorId, string title, string description = null);
    Task UpdateChecklistItemAsync(Guid checklistId, Guid itemId, bool isCompleted);
    Task<ContentChecklistDto> AddChecklistItemAsync(Guid checklistId, string description, bool isRequired = false);
    Task<ChecklistItemDto> SetItemDueDateAsync(Guid checklistId, Guid itemId, DateTime dueDate);
    Task<ChecklistItemDto> SetItemReminderAsync(Guid checklistId, Guid itemId, DateTime reminderDate);
    Task<ChecklistItemDto> SetItemPriorityAsync(Guid checklistId, Guid itemId, TaskPriority priority);
    Task<IEnumerable<ChecklistItemDto>> GetItemsWithUpcomingDeadlinesAsync(Guid creatorId, int daysAhead = 7);
    Task<IEnumerable<ChecklistItemDto>> GetDueItemsAsync(Guid creatorId);
    Task<IEnumerable<ChecklistItemDto>> GetOverdueItemsAsync(Guid creatorId);
}
