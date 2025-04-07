using Microsoft.AspNetCore.Http;
using MicroSaaS.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Application.Interfaces.Services
{
    /// <summary>
    /// Interface para o serviço de gerenciamento de mídia
    /// </summary>
    public interface IMediaService
    {
        /// <summary>
        /// Faz upload de arquivos de mídia
        /// </summary>
        /// <param name="files">Arquivos a serem enviados</param>
        /// <param name="creatorId">ID do criador de conteúdo</param>
        /// <returns>Lista de DTOs de mídia com URLs e metadados</returns>
        Task<List<MediaDto>> UploadMediaAsync(IFormFileCollection files, Guid creatorId);

        /// <summary>
        /// Obtém todas as mídias de um criador
        /// </summary>
        /// <param name="creatorId">ID do criador de conteúdo</param>
        /// <returns>Lista de DTOs de mídia</returns>
        Task<List<MediaDto>> GetMediaByCreatorAsync(Guid creatorId);

        /// <summary>
        /// Obtém uma mídia pelo ID
        /// </summary>
        /// <param name="id">ID da mídia</param>
        /// <returns>DTO de mídia ou null se não encontrada</returns>
        Task<MediaDto> GetMediaByIdAsync(Guid id);

        /// <summary>
        /// Exclui uma mídia
        /// </summary>
        /// <param name="id">ID da mídia</param>
        /// <returns>True se excluída com sucesso, False caso contrário</returns>
        Task<bool> DeleteMediaAsync(Guid id);

        /// <summary>
        /// Verifica se uma mídia pertence a um criador
        /// </summary>
        /// <param name="mediaId">ID da mídia</param>
        /// <param name="creatorId">ID do criador</param>
        /// <returns>True se a mídia pertence ao criador, False caso contrário</returns>
        Task<bool> IsMediaOwnedByCreatorAsync(Guid mediaId, Guid creatorId);
    }
}
