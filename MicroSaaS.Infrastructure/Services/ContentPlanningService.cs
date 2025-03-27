using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;

namespace MicroSaaS.Infrastructure.Services;

public class ContentPlanningService : IContentPlanningService
{
    private readonly IContentCreatorRepository _creatorRepository;
    private readonly ISocialMediaIntegrationService _socialMediaService;
    private readonly IContentChecklistRepository _checklistRepository;

    public ContentPlanningService(
        IContentCreatorRepository creatorRepository,
        ISocialMediaIntegrationService socialMediaService,
        IContentChecklistRepository checklistRepository)
    {
        _creatorRepository = creatorRepository;
        _socialMediaService = socialMediaService;
        _checklistRepository = checklistRepository;
    }

    public async Task<ContentPost> CreatePostAsync(ContentPost post)
    {
        // Implementação temporária
        return post;
    }

    public async Task<ContentPost> UpdatePostAsync(ContentPost post)
    {
        // Implementação temporária
        return post;
    }

    public async Task DeletePostAsync(Guid id)
    {
        // Implementação temporária
    }

    public async Task<IEnumerable<ContentPost>> GetPostsByCreatorAsync(Guid creatorId)
    {
        // Implementação temporária
        return new List<ContentPost>();
    }

    public async Task<IEnumerable<ContentPost>> GetPostsByStatusAsync(PostStatus status)
    {
        // Implementação temporária
        return new List<ContentPost>();
    }

    public async Task<IEnumerable<ContentPost>> GetPostsByScheduledTimeRangeAsync(DateTime start, DateTime end)
    {
        // Implementação temporária
        return new List<ContentPost>();
    }

    public async Task<List<DateTime>> SuggestPostTimesAsync(SocialMediaPlatform platform, Guid creatorId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado");

        // Obter recomendações de horários baseadas em métricas históricas
        var account = creator.SocialMediaAccounts?.FirstOrDefault(a => a.Platform == platform && a.IsActive)?.Id ?? Guid.Empty;
        var recommendations = await _socialMediaService.GetBestPostingTimesAsync(account);
        
        // Converter recomendações em datas concretas para os próximos 7 dias
        var suggestedTimes = new List<DateTime>();
        var now = DateTime.UtcNow;
        
        foreach (var recommendation in recommendations.OrderByDescending(r => r.EngagementScore))
        {
            var nextDate = GetNextDateForDayOfWeek(now, recommendation.DayOfWeek);
            var suggestedTime = nextDate.Date.Add(recommendation.TimeOfDay);
            
            // Ajustar para UTC se necessário
            if (suggestedTime.Kind == DateTimeKind.Unspecified)
                suggestedTime = DateTime.SpecifyKind(suggestedTime, DateTimeKind.Utc);
            
            suggestedTimes.Add(suggestedTime);
        }

        return suggestedTimes;
    }

    public async Task<ContentChecklist> CreateChecklistAsync(Guid creatorId, string title)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado");

        var checklist = new ContentChecklist
        {
            Id = Guid.NewGuid(),
            CreatorId = creatorId,
            Creator = creator,
            Title = title,
            Description = "Nova lista de verificação",
            Items = new List<ChecklistItem>(),
            Status = ChecklistStatus.NotStarted,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _checklistRepository.AddAsync(checklist);
    }

    public async Task UpdateChecklistItemAsync(Guid checklistItemId, bool isCompleted)
    {
        var (checklist, item) = await FindChecklistAndItemAsync(checklistItemId);
        await _checklistRepository.UpdateChecklistItemAsync(checklist.Id, item.Id, isCompleted);
    }

    private async Task<(ContentChecklist checklist, ChecklistItem item)> FindChecklistAndItemAsync(Guid checklistItemId)
    {
        var checklists = await _checklistRepository.GetAllAsync();
        foreach (var checklist in checklists)
        {
            var item = checklist.Items.FirstOrDefault(i => i.Id == checklistItemId);
            if (item != null)
            {
                return (checklist, item);
            }
        }

        throw new ArgumentException("Item não encontrado em nenhum checklist");
    }

    private DateTime GetNextDateForDayOfWeek(DateTime currentDate, DayOfWeek targetDay)
    {
        var daysUntilTarget = ((int)targetDay - (int)currentDate.DayOfWeek + 7) % 7;
        return currentDate.AddDays(daysUntilTarget);
    }
} 