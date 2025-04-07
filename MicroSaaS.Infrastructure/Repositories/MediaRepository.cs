using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Infrastructure.Data;
using MicroSaaS.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicroSaaS.Shared.DTOs;
using System.Linq;

namespace MicroSaaS.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação do repositório de mídia usando MongoDB
    /// </summary>
    public class MediaRepository : IMediaRepository
    {
        private readonly IMongoCollection<Media> _mediaCollection;

        public MediaRepository(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var client = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _mediaCollection = database.GetCollection<Media>("Media");
        }

        /// <summary>
        /// Adiciona uma nova mídia
        /// </summary>
        public async Task<MediaDto> AddAsync(MediaDto mediaDto)
        {
            if (mediaDto == null)
                throw new ArgumentNullException(nameof(mediaDto));

            // Converter DTO para entidade
            var media = MapToEntity(mediaDto);

            await _mediaCollection.InsertOneAsync(media);

            // Converter entidade de volta para DTO
            return MapToDto(media);
        }

        /// <summary>
        /// Obtém uma mídia pelo ID
        /// </summary>
        public async Task<MediaDto> GetByIdAsync(Guid id)
        {
            var filter = Builders<Media>.Filter.Eq(m => m.Id, id) &
                         Builders<Media>.Filter.Eq(m => m.IsActive, true);

            var media = await _mediaCollection.Find(filter).FirstOrDefaultAsync();

            return media != null ? MapToDto(media) : null;
        }

        /// <summary>
        /// Obtém todas as mídias de um criador
        /// </summary>
        public async Task<List<MediaDto>> GetByCreatorIdAsync(Guid creatorId)
        {
            var filter = Builders<Media>.Filter.Eq(m => m.CreatorId, creatorId) &
                         Builders<Media>.Filter.Eq(m => m.IsActive, true);

            var mediaList = await _mediaCollection.Find(filter)
                .SortByDescending(m => m.CreatedAt)
                .ToListAsync();

            return mediaList.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Atualiza uma mídia existente
        /// </summary>
        public async Task<MediaDto> UpdateAsync(MediaDto mediaDto)
        {
            if (mediaDto == null)
                throw new ArgumentNullException(nameof(mediaDto));

            // Obter a entidade existente
            var filter = Builders<Media>.Filter.Eq(m => m.Id, mediaDto.Id);
            var existingMedia = await _mediaCollection.Find(filter).FirstOrDefaultAsync();

            if (existingMedia == null)
                return null;

            // Atualizar a entidade com os dados do DTO
            existingMedia.Url = mediaDto.Url;
            existingMedia.FileName = mediaDto.FileName;
            existingMedia.FileType = mediaDto.FileType;
            existingMedia.FileSize = mediaDto.FileSize;
            existingMedia.Width = mediaDto.Width;
            existingMedia.Height = mediaDto.Height;
            existingMedia.Duration = mediaDto.Duration;
            existingMedia.IsActive = mediaDto.IsActive;
            existingMedia.UpdatedAt = DateTime.UtcNow;

            // Salvar as alterações
            await _mediaCollection.ReplaceOneAsync(filter, existingMedia);

            return MapToDto(existingMedia);
        }

        /// <summary>
        /// Exclui uma mídia (exclusão lógica)
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var filter = Builders<Media>.Filter.Eq(m => m.Id, id);
            var update = Builders<Media>.Update
                .Set(m => m.IsActive, false)
                .Set(m => m.UpdatedAt, DateTime.UtcNow);

            var result = await _mediaCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Verifica se uma mídia pertence a um criador
        /// </summary>
        public async Task<bool> IsOwnedByCreatorAsync(Guid mediaId, Guid creatorId)
        {
            var filter = Builders<Media>.Filter.Eq(m => m.Id, mediaId) &
                         Builders<Media>.Filter.Eq(m => m.CreatorId, creatorId) &
                         Builders<Media>.Filter.Eq(m => m.IsActive, true);

            var count = await _mediaCollection.CountDocumentsAsync(filter);

            return count > 0;
        }

        #region Métodos Privados

        /// <summary>
        /// Converte uma entidade Media para um DTO MediaDto
        /// </summary>
        private MediaDto MapToDto(Media media)
        {
            return new MediaDto
            {
                Id = media.Id,
                CreatorId = media.CreatorId,
                Url = media.Url,
                FileName = media.FileName,
                FileType = media.FileType,
                FileSize = media.FileSize,
                Width = media.Width,
                Height = media.Height,
                Duration = media.Duration,
                CreatedAt = media.CreatedAt,
                UpdatedAt = media.UpdatedAt,
                IsActive = media.IsActive
            };
        }

        /// <summary>
        /// Converte um DTO MediaDto para uma entidade Media
        /// </summary>
        private Media MapToEntity(MediaDto dto)
        {
            return new Media
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                CreatorId = dto.CreatorId,
                Url = dto.Url,
                FileName = dto.FileName,
                FileType = dto.FileType,
                FileSize = dto.FileSize,
                StoragePath = string.Empty, // Este campo não está no DTO, seria preenchido pelo serviço
                Width = dto.Width,
                Height = dto.Height,
                Duration = dto.Duration,
                CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                IsActive = dto.IsActive
            };
        }

        #endregion
    }
}
