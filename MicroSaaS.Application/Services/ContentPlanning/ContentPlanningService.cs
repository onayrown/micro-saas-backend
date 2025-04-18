using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Results;
using MicroSaaS.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MicroSaaS.Application.Services;

/// <summary>
/// Implementação do serviço de planejamento de conteúdo.
/// Este serviço pertence à camada de Aplicação conforme a Clean Architecture.
/// </summary>
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

    public async Task<ContentChecklistDto> GetChecklistByIdAsync(Guid id)
    {
        var checklist = await _checklistRepository.GetByIdAsync(id);
        return MapToChecklistDto(checklist);
    }

    public async Task<List<ContentChecklistDto>> GetChecklistsByCreatorIdAsync(Guid creatorId)
    {
        var checklists = await _checklistRepository.GetByCreatorIdAsync(creatorId);
        return checklists.Select(MapToChecklistDto).ToList();
    }

    public async Task<bool> DeleteChecklistAsync(Guid id)
    {
        var checklist = await _checklistRepository.GetByIdAsync(id);
        if (checklist == null)
            return false;
        
        await _checklistRepository.DeleteAsync(id);
        return true;
    }

    public async Task<ContentChecklistDto> CreateChecklistAsync(Guid creatorId, string title, string description = null)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado");

        var checklist = new ContentChecklist
        {
            Id = Guid.NewGuid(),
            CreatorId = creator.Id,
            Title = title,
            Description = description ?? "Nova lista de verificação",
            Items = new List<ChecklistItem>(),
            Status = ChecklistStatus.NotStarted,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var addedChecklist = await _checklistRepository.AddAsync(checklist);
        return MapToChecklistDto(addedChecklist);
    }

    public async Task<ContentChecklistDto> AddChecklistItemAsync(Guid checklistId, string description, bool isRequired = false)
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

        var updatedChecklist = await _checklistRepository.UpdateAsync(checklist);
        return MapToChecklistDto(updatedChecklist);
    }

    public async Task UpdateChecklistItemAsync(Guid checklistId, Guid itemId, bool isCompleted)
    {
        await _checklistRepository.UpdateChecklistItemAsync(checklistId, itemId, isCompleted);
    }

    public async Task<ChecklistItemDto> SetItemDueDateAsync(Guid checklistId, Guid itemId, DateTime dueDate)
    {
        var checklist = await _checklistRepository.GetByIdAsync(checklistId);
        if (checklist == null) throw new KeyNotFoundException("Checklist não encontrada.");
        var item = checklist.Items?.FirstOrDefault(i => i.Id == itemId);
        if (item == null) throw new KeyNotFoundException("Item não encontrado na checklist.");
        
        item.DueDate = dueDate;
        checklist.UpdatedAt = DateTime.UtcNow;
        
        await _checklistRepository.UpdateAsync(checklist);
        return MapToChecklistItemDto(item);
    }
    
    public async Task<ChecklistItemDto> SetItemReminderAsync(Guid checklistId, Guid itemId, DateTime reminderDate)
    {
        var checklist = await _checklistRepository.GetByIdAsync(checklistId);
        if (checklist == null) throw new KeyNotFoundException("Checklist não encontrada.");
        var item = checklist.Items?.FirstOrDefault(i => i.Id == itemId);
        if (item == null) throw new KeyNotFoundException("Item não encontrado na checklist.");

        item.HasReminder = true;
        item.ReminderDate = reminderDate;
        checklist.UpdatedAt = DateTime.UtcNow;
        
        await _checklistRepository.UpdateAsync(checklist);
        return MapToChecklistItemDto(item);
    }
    
    public async Task<ChecklistItemDto> SetItemPriorityAsync(Guid checklistId, Guid itemId, TaskPriority priority)
    {
        var checklist = await _checklistRepository.GetByIdAsync(checklistId);
        if (checklist == null) throw new KeyNotFoundException("Checklist não encontrada.");
        var item = checklist.Items?.FirstOrDefault(i => i.Id == itemId);
        if (item == null) throw new KeyNotFoundException("Item não encontrado na checklist.");

        item.Priority = priority;
        checklist.UpdatedAt = DateTime.UtcNow;
        
        await _checklistRepository.UpdateAsync(checklist);
        return MapToChecklistItemDto(item);
    }
    
    public async Task<IEnumerable<ChecklistItemDto>> GetItemsWithUpcomingDeadlinesAsync(Guid creatorId, int daysAhead = 7)
    {
        var checklists = await _checklistRepository.GetByCreatorIdAsync(creatorId);
        var upcomingDeadline = DateTime.UtcNow.AddDays(daysAhead);
        var items = checklists.SelectMany(c => c.Items ?? new List<ChecklistItem>())
                              .Where(i => i.DueDate.HasValue && i.DueDate.Value <= upcomingDeadline && i.DueDate.Value >= DateTime.UtcNow && !i.IsCompleted)
                              .OrderBy(i => i.DueDate.Value);
        return items.Select(MapToChecklistItemDto);
    }
    
    public async Task<IEnumerable<ChecklistItemDto>> GetDueItemsAsync(Guid creatorId)
    {
        var checklists = await _checklistRepository.GetByCreatorIdAsync(creatorId);
        var today = DateTime.UtcNow.Date;
        var items = checklists.SelectMany(c => c.Items ?? new List<ChecklistItem>())
                              .Where(i => i.DueDate.HasValue && i.DueDate.Value.Date == today && !i.IsCompleted)
                              .OrderBy(i => i.Priority);
         return items.Select(MapToChecklistItemDto);
    }
    
    public async Task<IEnumerable<ChecklistItemDto>> GetOverdueItemsAsync(Guid creatorId)
    {
        var checklists = await _checklistRepository.GetByCreatorIdAsync(creatorId);
         var now = DateTime.UtcNow;
         var items = checklists.SelectMany(c => c.Items ?? new List<ChecklistItem>())
                               .Where(i => i.DueDate.HasValue && i.DueDate.Value < now && !i.IsCompleted)
                               .OrderByDescending(i => i.DueDate.Value);
         return items.Select(MapToChecklistItemDto);
    }

    private DateTime GetNextDateForDayOfWeek(DateTime currentDate, DayOfWeek targetDay)
    {
        var daysUntilTarget = ((int)targetDay - (int)currentDate.DayOfWeek + 7) % 7;
        return currentDate.AddDays(daysUntilTarget);
    }

    private ContentChecklistDto MapToChecklistDto(ContentChecklist entity)
    {
        if (entity == null) return null;
        return new ContentChecklistDto
        {
            Id = entity.Id,
            CreatorId = entity.CreatorId,
            Title = entity.Title,
            Description = entity.Description,
            Status = entity.Status.ToString(),
            Items = entity.Items?.Select(MapToChecklistItemDto).ToList() ?? new List<ChecklistItemDto>(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private ChecklistItemDto MapToChecklistItemDto(ChecklistItem entity)
    {
        if (entity == null) return null;
        return new ChecklistItemDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            IsCompleted = entity.IsCompleted,
            DueDate = entity.DueDate,
            Priority = entity.Priority.ToString(),
            CreatedAt = entity.CreatedAt
        };
    }
} 