using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Application.Interfaces.Services;

public interface IContentPlanningService
{
    Task<List<DateTime>> SuggestPostTimesAsync(SocialMediaPlatform platform, Guid creatorId);
    Task<ContentChecklist> CreateChecklistAsync(Guid creatorId, string title);
    Task UpdateChecklistItemAsync(Guid checklistItemId, bool isCompleted);
}
