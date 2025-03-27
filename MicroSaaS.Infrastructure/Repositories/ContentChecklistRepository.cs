using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Data;
using MicroSaaS.Shared.Enums;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.Repositories;

public class ContentChecklistRepository : IContentChecklistRepository
{
    private readonly MongoDbContext _context;

    public ContentChecklistRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<ContentChecklist> GetByIdAsync(Guid id)
    {
        return await _context.GetCollection<ContentChecklist>("content_checklists")
            .Find(c => c.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ContentChecklist>> GetAllAsync()
    {
        return await _context.GetCollection<ContentChecklist>("content_checklists")
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task<ContentChecklist> AddAsync(ContentChecklist checklist)
    {
        await _context.GetCollection<ContentChecklist>("content_checklists")
            .InsertOneAsync(checklist);
        return checklist;
    }

    public async Task<ContentChecklist> UpdateAsync(ContentChecklist checklist)
    {
        var filter = Builders<ContentChecklist>.Filter.Eq(c => c.Id, checklist.Id);
        await _context.GetCollection<ContentChecklist>("content_checklists")
            .ReplaceOneAsync(filter, checklist);
        return checklist;
    }

    public async Task DeleteAsync(Guid id)
    {
        var filter = Builders<ContentChecklist>.Filter.Eq(c => c.Id, id);
        await _context.GetCollection<ContentChecklist>("content_checklists")
            .DeleteOneAsync(filter);
    }

    public async Task<IEnumerable<ContentChecklist>> GetByCreatorIdAsync(Guid creatorId)
    {
        return await _context.GetCollection<ContentChecklist>("content_checklists")
            .Find(c => c.CreatorId == creatorId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ContentChecklist>> GetByStatusAsync(ChecklistStatus status)
    {
        return await _context.GetCollection<ContentChecklist>("content_checklists")
            .Find(c => c.Status == status)
            .ToListAsync();
    }

    public async Task UpdateChecklistItemAsync(Guid checklistId, Guid itemId, bool isCompleted)
    {
        var filter = Builders<ContentChecklist>.Filter.And(
            Builders<ContentChecklist>.Filter.Eq(c => c.Id, checklistId),
            Builders<ContentChecklist>.Filter.ElemMatch(c => c.Items, i => i.Id == itemId)
        );

        var update = Builders<ContentChecklist>.Update
            .Set(c => c.Items[-1].IsCompleted, isCompleted)
            .Set(c => c.Items[-1].CompletedAt, isCompleted ? DateTime.UtcNow : null);

        await _context.GetCollection<ContentChecklist>("content_checklists")
            .UpdateOneAsync(filter, update);
    }
} 