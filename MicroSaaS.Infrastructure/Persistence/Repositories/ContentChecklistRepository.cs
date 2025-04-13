using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.MongoDB;
using MicroSaaS.Shared.Enums;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace MicroSaaS.Infrastructure.Persistence.Repositories
{
    public class ContentChecklistRepository : IContentChecklistRepository
    {
        private readonly IMongoCollection<ContentChecklist> _collection;

        // Presume que você tem uma propriedade/método para obter a coleção de Checklists no seu IMongoDbContext
        // Exemplo: _context.Checklists
        public ContentChecklistRepository(IMongoDbContext dbContext)
        {
            // Ajuste o nome da coleção conforme necessário (ex: "Checklists")
            _collection = dbContext.GetCollection<ContentChecklist>("contentchecklists");
        }

        // Helper para criar FieldDefinition a partir de lambda (usado em ElemMatch/Set)
        private static FieldDefinition<ContentChecklist, TField> Field<TField>(Expression<Func<ContentChecklist, TField>> expression)
        {
            return new ExpressionFieldDefinition<ContentChecklist, TField>(expression);
        }

        public async Task<ContentChecklist> GetByIdAsync(Guid id)
        {
            // O serializador de Guid configurado no MongoDbInitializer cuidará da conversão
            var filter = Builders<ContentChecklist>.Filter.Eq(c => c.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ContentChecklist>> GetAllAsync()
        {
            return await _collection.Find(Builders<ContentChecklist>.Filter.Empty).ToListAsync();
        }

        public async Task<ContentChecklist> AddAsync(ContentChecklist checklist)
        {
            // Garantir que o ID seja sempre um novo Guid
            if (checklist.Id == Guid.Empty)
            {
                checklist.Id = Guid.NewGuid();
            }
            
            checklist.CreatedAt = DateTime.UtcNow;
            checklist.UpdatedAt = DateTime.UtcNow;
            await _collection.InsertOneAsync(checklist);
            return checklist;
        }

        public async Task<ContentChecklist> UpdateAsync(ContentChecklist checklist)
        {
            // Usar expressão lambda para que o serializador cuide da conversão
            var filter = Builders<ContentChecklist>.Filter.Eq(c => c.Id, checklist.Id);
            checklist.UpdatedAt = DateTime.UtcNow;
            await _collection.ReplaceOneAsync(filter, checklist);
            return checklist;
        }

        public async Task DeleteAsync(Guid id)
        {
            // Usar expressão lambda para que o serializador cuide da conversão
            var filter = Builders<ContentChecklist>.Filter.Eq(c => c.Id, id);
            await _collection.DeleteOneAsync(filter);
        }

        public async Task<IEnumerable<ContentChecklist>> GetByCreatorIdAsync(Guid creatorId)
        {
            // Usar expressão lambda para que o serializador cuide da conversão
            var filter = Builders<ContentChecklist>.Filter.Eq(c => c.CreatorId, creatorId);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<ContentChecklist>> GetByStatusAsync(ChecklistStatus status)
        {
            var filter = Builders<ContentChecklist>.Filter.Eq(c => c.Status, status);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task UpdateChecklistItemAsync(Guid checklistId, Guid itemId, bool isCompleted)
        {
            // Para o filtro principal, usar expressão lambda
            var checklistFilter = Builders<ContentChecklist>.Filter.Eq(c => c.Id, checklistId);
            
            // Para o ElemMatch que identifica o item, precisamos de uma abordagem diferente
            // devido a como o MongoDB trata arrays aninhados
            var itemFilter = Builders<ContentChecklist>.Filter.ElemMatch(
                c => c.Items, 
                item => item.Id == itemId
            );
            
            var combinedFilter = Builders<ContentChecklist>.Filter.And(checklistFilter, itemFilter);

            // Usar o operador de array posicional ($) para atualizar o item específico
            var update = Builders<ContentChecklist>.Update
                .Set("Items.$.IsCompleted", isCompleted)
                .Set("Items.$.UpdatedAt", DateTime.UtcNow)
                .Set("UpdatedAt", DateTime.UtcNow);

            if (isCompleted)
            {
                // Especificar explicitamente os tipos para evitar problemas de inferência
                update = update.Set<ContentChecklist, DateTime?>("Items.$.CompletedAt", DateTime.UtcNow);
            }
            else
            {
                // Especificar explicitamente os tipos para evitar problemas de inferência
                update = update.Set<ContentChecklist, DateTime?>("Items.$.CompletedAt", null);
            }

            await _collection.UpdateOneAsync(combinedFilter, update);
        }
    }
} 