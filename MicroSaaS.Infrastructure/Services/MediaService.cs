using Microsoft.AspNetCore.Http;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace MicroSaaS.Infrastructure.Services
{
    /// <summary>
    /// Implementação do serviço de gerenciamento de mídia
    /// </summary>
    public class MediaService : IMediaService
    {
        private readonly IMediaRepository _mediaRepository;
        private readonly IStorageService _storageService;
        private readonly ILoggingService _loggingService;

        public MediaService(
            IMediaRepository mediaRepository,
            IStorageService storageService,
            ILoggingService loggingService)
        {
            _mediaRepository = mediaRepository ?? throw new ArgumentNullException(nameof(mediaRepository));
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        }

        /// <summary>
        /// Faz upload de arquivos de mídia
        /// </summary>
        public async Task<List<MediaDto>> UploadMediaAsync(IFormFileCollection files, Guid creatorId)
        {
            if (files == null || files.Count == 0)
                throw new ArgumentException("Nenhum arquivo enviado", nameof(files));

            var result = new List<MediaDto>();

            foreach (var file in files)
            {
                try
                {
                    // Validar o arquivo
                    ValidateFile(file);

                    // Fazer upload do arquivo
                    var (url, path) = await _storageService.UploadFileAsync(file, "media");

                    // Criar a entidade Media
                    var media = new Media
                    {
                        Id = Guid.NewGuid(),
                        CreatorId = creatorId,
                        Url = url,
                        StoragePath = path, // Store the storage path
                        FileName = file.FileName,
                        FileType = file.ContentType,
                        FileSize = file.Length,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    // Extrair metadados do arquivo (agora passa a entidade Media)
                    await ExtractMetadataAsync(media, file);

                    // Salvar a mídia no banco de dados (espera Media, retorna Media)
                    var savedMediaEntity = await _mediaRepository.AddAsync(media);

                    // Mapear a entidade salva para DTO e adicionar ao resultado
                    result.Add(MapToDto(savedMediaEntity));
                }
                catch (Exception ex)
                {
                    _loggingService.LogError(ex, "Erro ao fazer upload do arquivo {FileName}: {Message}", file.FileName, ex.Message);
                    // Continuar com o próximo arquivo
                }
            }

            return result;
        }

        /// <summary>
        /// Obtém todas as mídias de um criador
        /// </summary>
        public async Task<List<MediaDto>> GetMediaByCreatorAsync(Guid creatorId)
        {
            // Repositório retorna List<Media>
            var mediaEntities = await _mediaRepository.GetByCreatorIdAsync(creatorId);
            // Mapear para List<MediaDto>
            return mediaEntities.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Obtém uma mídia pelo ID
        /// </summary>
        public async Task<MediaDto?> GetMediaByIdAsync(Guid id) // Return nullable DTO
        {
            // Repositório retorna Media?
            var mediaEntity = await _mediaRepository.GetByIdAsync(id);
            // Mapear para MediaDto?
            return mediaEntity != null ? MapToDto(mediaEntity) : null;
        }

        /// <summary>
        /// Exclui uma mídia
        /// </summary>
        public async Task<bool> DeleteMediaAsync(Guid id)
        {
            // Obter a entidade Media primeiro para pegar o StoragePath
            var mediaEntity = await _mediaRepository.GetByIdAsync(id);
            if (mediaEntity == null)
                return false;

            // Excluir o arquivo do armazenamento usando StoragePath da entidade
            if (!string.IsNullOrEmpty(mediaEntity.StoragePath))
            {
                await _storageService.DeleteFileAsync(mediaEntity.StoragePath);
            }
            else // Fallback: tentar extrair da URL se StoragePath estiver vazio
            {
                 string storagePathFromUrl = ExtractStoragePathFromUrl(mediaEntity.Url);
                 if (!string.IsNullOrEmpty(storagePathFromUrl))
                 {
                     await _storageService.DeleteFileAsync(storagePathFromUrl);
                     _loggingService.LogWarning("StoragePath estava vazio para MediaId {MediaId}, usando caminho extraído da URL: {Path}", id, storagePathFromUrl);
                 }
                 else
                 {
                     _loggingService.LogError(null, "Não foi possível determinar o caminho de armazenamento para excluir o arquivo da MediaId {MediaId}", id);
                     // Considerar se deve continuar com a exclusão do DB mesmo assim
                 }
            }


            // Excluir a mídia do banco de dados (exclusão lógica)
            return await _mediaRepository.DeleteAsync(id); // Este método já existe no repositório e opera por ID
        }

        /// <summary>
        /// Verifica se uma mídia pertence a um criador
        /// </summary>
        public async Task<bool> IsMediaOwnedByCreatorAsync(Guid mediaId, Guid creatorId)
        {
            return await _mediaRepository.IsOwnedByCreatorAsync(mediaId, creatorId);
        }

        /// <summary>
        /// Extrai o caminho de armazenamento a partir da URL
        /// </summary>
        private string ExtractStoragePathFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            try
            {
                // Exemplo: https://localhost:7171/uploads/media/file.jpg -> uploads/media/file.jpg
                Uri uri = new Uri(url);
                string path = uri.AbsolutePath.TrimStart('/');
                return path;
            }
            catch
            {
                return string.Empty;
            }
        }

        #region Métodos Privados de Mapeamento

        private MediaDto MapToDto(Media media)
        {
            return new MediaDto
            {
                Id = media.Id,
                CreatorId = media.CreatorId,
                Url = media.Url,
                // StoragePath não costuma ir para o DTO
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

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Valida um arquivo
        /// </summary>
        private void ValidateFile(IFormFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            if (file.Length == 0)
                throw new ArgumentException("Arquivo vazio", nameof(file));

            // Validar o tamanho do arquivo (100 MB)
            if (file.Length > 100 * 1024 * 1024)
                throw new ArgumentException("Arquivo muito grande. O tamanho máximo é 100 MB", nameof(file));

            // Validar o tipo do arquivo
            var allowedTypes = new[] { "image/", "video/", "audio/" };
            if (!allowedTypes.Any(t => file.ContentType.StartsWith(t, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("Tipo de arquivo não permitido. Apenas imagens, vídeos e áudios são permitidos", nameof(file));
        }

        /// <summary>
        /// Extrai metadados de um arquivo (agora opera na entidade Media)
        /// </summary>
        private async Task ExtractMetadataAsync(Media media, IFormFile file) // Recebe Media
        {
            try
            {
                // Extrair metadados de imagens
                if (file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    using (var stream = file.OpenReadStream())
                    {
                        using (var image = await Image.LoadAsync(stream))
                        {
                            media.Width = image.Width; // Modifica a entidade
                            media.Height = image.Height; // Modifica a entidade
                        }
                    }
                }
                // else if (file.ContentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
                // {
                //     // Extrair duração do vídeo (ex: usando FFmpeg)
                //     // media.Duration = GetVideoDuration(file);
                // }
                // else if (file.ContentType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase))
                // {
                //     // Extrair duração do áudio
                //     // media.Duration = GetAudioDuration(file);
                // }

            }
            catch (Exception ex)
            {
                _loggingService.LogWarning("Erro ao extrair metadados do arquivo {FileName}: {Message}. Detalhes: {ExceptionMessage}", file.FileName, ex.Message, ex.ToString());
                // Não propagar a exceção, apenas logar o erro
            }
        }

        #endregion
    }
}
