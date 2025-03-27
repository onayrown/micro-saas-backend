using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Application.Interfaces.Repositories;

public interface IContentChecklistRepository
{
    Task<ContentChecklist> GetByIdAsync(Guid id);
    Task<IEnumerable<ContentChecklist>> GetAllAsync();
    Task<ContentChecklist> AddAsync(ContentChecklist checklist);
    Task<ContentChecklist> UpdateAsync(ContentChecklist checklist);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<ContentChecklist>> GetByCreatorIdAsync(Guid creatorId);
    Task<IEnumerable<ContentChecklist>> GetByStatusAsync(ChecklistStatus status);
    Task UpdateChecklistItemAsync(Guid checklistId, Guid itemId, bool isCompleted);
}
