using MicroSaaS.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Application.Interfaces.Repositories
{
    /// <summary>
    /// Interface para o repositório de mídia
    /// </summary>
    public interface IMediaRepository
    {
        /// <summary>
        /// Adiciona uma nova mídia
        /// </summary>
        /// <param name="mediaDto">DTO de mídia</param>
        /// <returns>DTO de mídia adicionada</returns>
        Task<MediaDto> AddAsync(MediaDto mediaDto);

        /// <summary>
        /// Obtém uma mídia pelo ID
        /// </summary>
        /// <param name="id">ID da mídia</param>
        /// <returns>DTO de mídia ou null se não encontrada</returns>
        Task<MediaDto> GetByIdAsync(Guid id);

        /// <summary>
        /// Obtém todas as mídias de um criador
        /// </summary>
        /// <param name="creatorId">ID do criador</param>
        /// <returns>Lista de DTOs de mídia</returns>
        Task<List<MediaDto>> GetByCreatorIdAsync(Guid creatorId);

        /// <summary>
        /// Atualiza uma mídia existente
        /// </summary>
        /// <param name="mediaDto">DTO de mídia com as alterações</param>
        /// <returns>DTO de mídia atualizada</returns>
        Task<MediaDto> UpdateAsync(MediaDto mediaDto);

        /// <summary>
        /// Exclui uma mídia (exclusão lógica)
        /// </summary>
        /// <param name="id">ID da mídia</param>
        /// <returns>True se excluída com sucesso, False caso contrário</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Verifica se uma mídia pertence a um criador
        /// </summary>
        /// <param name="mediaId">ID da mídia</param>
        /// <param name="creatorId">ID do criador</param>
        /// <returns>True se a mídia pertence ao criador, False caso contrário</returns>
        Task<bool> IsOwnedByCreatorAsync(Guid mediaId, Guid creatorId);
    }
}
