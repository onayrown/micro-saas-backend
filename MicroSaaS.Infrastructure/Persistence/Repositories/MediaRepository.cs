using MongoDB.Driver;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Infrastructure.MongoDB; // Usar namespace correto
// using MicroSaaS.Shared.DTOs; // Remover DTO
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MicroSaaS.Infrastructure.Persistence.Repositories 
{
    // Reafirmar que implementa a interface
    public class MediaRepository : IMediaRepository
    {
        private readonly IMongoCollection<Media> _mediaCollection;

        // Injetar IMongoDbContext
        public MediaRepository(IMongoDbContext context)
        {
            // Obter coleção do contexto (assumindo que existe ou será adicionada)
            _mediaCollection = context.GetCollection<Media>("Media"); 
        }

        // Reafirmar assinatura completa
        public async Task<Media> AddAsync(Media media)
        {
            if (media == null)
                throw new ArgumentNullException(nameof(media));
            
            // Garantir que Id e Datas estão setados se necessário
            media.Id = (media.Id == Guid.Empty) ? Guid.NewGuid() : media.Id;
            media.CreatedAt = (media.CreatedAt == default) ? DateTime.UtcNow : media.CreatedAt;
            media.UpdatedAt = DateTime.UtcNow;
            media.IsActive = true; // Geralmente ativo ao adicionar

            await _mediaCollection.InsertOneAsync(media);
            return media; // Retorna a entidade inserida
        }

        // Reafirmar assinatura completa
        public async Task<Media> GetByIdAsync(Guid id)
        {
            var filter = Builders<Media>.Filter.Eq(m => m.Id, id) &
                         Builders<Media>.Filter.Eq(m => m.IsActive, true);

            return await _mediaCollection.Find(filter).FirstOrDefaultAsync();
        }

        // Reafirmar assinatura completa
        public async Task<List<Media>> GetByCreatorIdAsync(Guid creatorId)
        {
            var filter = Builders<Media>.Filter.Eq(m => m.CreatorId, creatorId) &
                         Builders<Media>.Filter.Eq(m => m.IsActive, true);

            return await _mediaCollection.Find(filter)
                .SortByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        // Reafirmar assinatura completa
        public async Task<Media> UpdateAsync(Media media)
        {
            if (media == null)
                throw new ArgumentNullException(nameof(media));

            var filter = Builders<Media>.Filter.Eq(m => m.Id, media.Id);
            
            // Buscar entidade existente para garantir que não sobrescrevemos campos não intencionais
            // Ou usar um UpdateDefinition se quisermos apenas atualizar campos específicos
            // Por simplicidade, ReplaceOne com a entidade completa (mas garantindo UpdatedAt)
            media.UpdatedAt = DateTime.UtcNow;

            var result = await _mediaCollection.ReplaceOneAsync(filter, media);

            return result.IsAcknowledged ? media : null;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var filter = Builders<Media>.Filter.Eq(m => m.Id, id);
            var update = Builders<Media>.Update
                .Set(m => m.IsActive, false)
                .Set(m => m.UpdatedAt, DateTime.UtcNow);

            var result = await _mediaCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> IsOwnedByCreatorAsync(Guid mediaId, Guid creatorId)
        {
            var filter = Builders<Media>.Filter.Eq(m => m.Id, mediaId) &
                         Builders<Media>.Filter.Eq(m => m.CreatorId, creatorId) &
                         Builders<Media>.Filter.Eq(m => m.IsActive, true);

            var count = await _mediaCollection.CountDocumentsAsync(filter);
            return count > 0;
        }

        // Remover métodos de mapeamento privados
        // #region Métodos Privados ... #endregion
    }
} 