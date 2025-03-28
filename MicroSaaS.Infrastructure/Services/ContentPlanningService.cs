using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.Services;

public class ContentPlanningService : IContentPlanningService
{
    private readonly IContentCreatorRepository _creatorRepository;
    private readonly ISocialMediaIntegrationService _socialMediaService;
    private readonly IContentChecklistRepository _checklistRepository;
    private readonly IContentPostRepository _postRepository;

    public ContentPlanningService(
        IContentCreatorRepository creatorRepository,
        ISocialMediaIntegrationService socialMediaService,
        IContentChecklistRepository checklistRepository,
        IContentPostRepository postRepository)
    {
        _creatorRepository = creatorRepository;
        _socialMediaService = socialMediaService;
        _checklistRepository = checklistRepository;
        _postRepository = postRepository;
    }

    public async Task<ContentPost> CreatePostAsync(ContentPost post)
    {
        if (post == null)
            throw new ArgumentNullException(nameof(post));

        // Verificar se o criador existe
        var creator = await _creatorRepository.GetByIdAsync(post.CreatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado");

        // Configurar propriedades do post
        post.Id = post.Id == Guid.Empty ? Guid.NewGuid() : post.Id;
        post.CreatedAt = DateTime.UtcNow;
        post.UpdatedAt = DateTime.UtcNow;

        // Se o post estiver agendado, definir o status correto
        if (post.ScheduledFor.HasValue && post.ScheduledFor > DateTime.UtcNow)
        {
            post.Status = PostStatus.Scheduled;
        }
        else if (post.Status == PostStatus.Published)
        {
            post.PublishedAt = DateTime.UtcNow;
        }
        else
        {
            post.Status = PostStatus.Draft;
        }

        // Salvar o post no repositório
        return await _postRepository.AddAsync(post);
    }

    public async Task<ContentPost> UpdatePostAsync(ContentPost post)
    {
        if (post == null)
            throw new ArgumentNullException(nameof(post));

        var existingPost = await _postRepository.GetByIdAsync(post.Id);
        if (existingPost == null)
            throw new ArgumentException("Post não encontrado");

        // Atualizar propriedades
        post.UpdatedAt = DateTime.UtcNow;

        // Atualizar status conforme agendamento
        if (post.ScheduledFor.HasValue && post.ScheduledFor > DateTime.UtcNow)
        {
            post.Status = PostStatus.Scheduled;
        }
        else if (post.Status == PostStatus.Published && !existingPost.PublishedAt.HasValue)
        {
            post.PublishedAt = DateTime.UtcNow;
        }

        // Atualizar no repositório
        return await _postRepository.UpdateAsync(post);
    }

    public async Task DeletePostAsync(Guid id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null)
            throw new ArgumentException("Post não encontrado");

        await _postRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ContentPost>> GetPostsByCreatorAsync(Guid creatorId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado");

        return await _postRepository.GetByCreatorIdAsync(creatorId);
    }

    public async Task<IEnumerable<ContentPost>> GetPostsByStatusAsync(PostStatus status)
    {
        return await _postRepository.GetByStatusAsync(status);
    }

    public async Task<IEnumerable<ContentPost>> GetPostsByScheduledTimeRangeAsync(DateTime start, DateTime end)
    {
        return await _postRepository.GetByScheduledTimeRangeAsync(start, end);
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

    public async Task<ContentChecklist> AddChecklistItemAsync(Guid checklistId, string description, bool isRequired = false)
    {
        var checklist = await _checklistRepository.GetByIdAsync(checklistId);
        if (checklist == null)
            throw new ArgumentException("Lista de verificação não encontrada");

        if (checklist.Items == null)
            checklist.Items = new List<ChecklistItem>();

        var newItem = new ChecklistItem
        {
            Id = Guid.NewGuid(),
            Title = $"Item {checklist.Items.Count + 1}",
            Description = description,
            IsRequired = isRequired,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            Order = checklist.Items.Count + 1
        };

        checklist.Items.Add(newItem);
        checklist.UpdatedAt = DateTime.UtcNow;

        return await _checklistRepository.UpdateAsync(checklist);
    }

    public async Task UpdateChecklistItemAsync(Guid checklistItemId, bool isCompleted)
    {
        var (checklist, item) = await FindChecklistAndItemAsync(checklistItemId);
        await _checklistRepository.UpdateChecklistItemAsync(checklist.Id, item.Id, isCompleted);
    }

    public async Task<ChecklistItem> SetItemDueDateAsync(Guid checklistItemId, DateTime dueDate)
    {
        var (checklist, item) = await FindChecklistAndItemAsync(checklistItemId);
        
        item.DueDate = dueDate;
        checklist.UpdatedAt = DateTime.UtcNow;
        
        await _checklistRepository.UpdateAsync(checklist);
        return item;
    }
    
    public async Task<ChecklistItem> SetItemReminderAsync(Guid checklistItemId, DateTime reminderDate)
    {
        var (checklist, item) = await FindChecklistAndItemAsync(checklistItemId);
        
        item.HasReminder = true;
        item.ReminderDate = reminderDate;
        checklist.UpdatedAt = DateTime.UtcNow;
        
        await _checklistRepository.UpdateAsync(checklist);
        return item;
    }
    
    public async Task<ChecklistItem> SetItemPriorityAsync(Guid checklistItemId, TaskPriority priority)
    {
        var (checklist, item) = await FindChecklistAndItemAsync(checklistItemId);
        
        item.Priority = priority;
        checklist.UpdatedAt = DateTime.UtcNow;
        
        await _checklistRepository.UpdateAsync(checklist);
        return item;
    }
    
    public async Task<IEnumerable<ChecklistItem>> GetItemsWithUpcomingDeadlinesAsync(Guid creatorId, int daysAhead = 7)
    {
        var checklists = await _checklistRepository.GetByCreatorIdAsync(creatorId);
        var now = DateTime.UtcNow;
        var deadline = now.AddDays(daysAhead);
        
        var upcomingItems = new List<ChecklistItem>();
        
        foreach (var checklist in checklists)
        {
            if (checklist.Items == null) continue;
            
            var items = checklist.Items.Where(i => 
                !i.IsCompleted && 
                i.DueDate.HasValue && 
                i.DueDate.Value > now && 
                i.DueDate.Value <= deadline
            ).OrderBy(i => i.DueDate);
            
            upcomingItems.AddRange(items);
        }
        
        return upcomingItems;
    }
    
    public async Task<IEnumerable<ChecklistItem>> GetDueItemsAsync(Guid creatorId)
    {
        var checklists = await _checklistRepository.GetByCreatorIdAsync(creatorId);
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        
        var dueItems = new List<ChecklistItem>();
        
        foreach (var checklist in checklists)
        {
            if (checklist.Items == null) continue;
            
            var items = checklist.Items.Where(i => 
                !i.IsCompleted && 
                i.DueDate.HasValue && 
                i.DueDate.Value >= today && 
                i.DueDate.Value < tomorrow
            ).OrderBy(i => i.Priority).ThenBy(i => i.Order);
            
            dueItems.AddRange(items);
        }
        
        return dueItems;
    }
    
    public async Task<IEnumerable<ChecklistItem>> GetOverdueItemsAsync(Guid creatorId)
    {
        var checklists = await _checklistRepository.GetByCreatorIdAsync(creatorId);
        var today = DateTime.UtcNow.Date;
        
        var overdueItems = new List<ChecklistItem>();
        
        foreach (var checklist in checklists)
        {
            if (checklist.Items == null) continue;
            
            var items = checklist.Items.Where(i => 
                !i.IsCompleted && 
                i.DueDate.HasValue && 
                i.DueDate.Value < today
            ).OrderBy(i => i.DueDate).ThenBy(i => i.Priority);
            
            overdueItems.AddRange(items);
        }
        
        return overdueItems;
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