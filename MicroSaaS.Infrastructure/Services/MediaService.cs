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

                    // Criar o DTO de mídia
                    var mediaDto = new MediaDto
                    {
                        Id = Guid.NewGuid(),
                        CreatorId = creatorId,
                        Url = url,
                        FileName = file.FileName,
                        FileType = file.ContentType,
                        FileSize = file.Length,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    // Extrair metadados do arquivo
                    await ExtractMetadataAsync(mediaDto, file);

                    // Salvar a mídia no banco de dados
                    var savedMedia = await _mediaRepository.AddAsync(mediaDto);

                    // Adicionar ao resultado
                    result.Add(savedMedia);
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
            return await _mediaRepository.GetByCreatorIdAsync(creatorId);
        }

        /// <summary>
        /// Obtém uma mídia pelo ID
        /// </summary>
        public async Task<MediaDto> GetMediaByIdAsync(Guid id)
        {
            return await _mediaRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Exclui uma mídia
        /// </summary>
        public async Task<bool> DeleteMediaAsync(Guid id)
        {
            var media = await _mediaRepository.GetByIdAsync(id);
            if (media == null)
                return false;

            // Excluir o arquivo do armazenamento
            // Nota: Precisamos de uma forma de obter o caminho de armazenamento, que não está no DTO
            // Uma solução seria adicionar o campo StoragePath ao DTO ou usar um serviço para obter o caminho
            // Por enquanto, vamos extrair o caminho da URL
            string storagePath = ExtractStoragePathFromUrl(media.Url);
            if (!string.IsNullOrEmpty(storagePath))
            {
                await _storageService.DeleteFileAsync(storagePath);
            }

            // Excluir a mídia do banco de dados (exclusão lógica)
            return await _mediaRepository.DeleteAsync(id);
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
        /// Extrai metadados de um arquivo
        /// </summary>
        private async Task ExtractMetadataAsync(MediaDto mediaDto, IFormFile file)
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
                            mediaDto.Width = image.Width;
                            mediaDto.Height = image.Height;
                        }
                    }
                }

                // Extrair metadados de vídeos e áudios
                // Nota: Para extrair metadados de vídeos e áudios, seria necessário usar uma biblioteca específica
                // como FFmpeg, o que está fora do escopo deste exemplo
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
