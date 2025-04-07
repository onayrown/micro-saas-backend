using MicroSaaS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interface para o repositório de mídia
    /// </summary>
    public interface IMediaRepository
    {
        /// <summary>
        /// Adiciona uma nova mídia
        /// </summary>
        /// <param name="media">Entidade de mídia</param>
        /// <returns>Mídia adicionada</returns>
        Task<Media> AddAsync(Media media);

        /// <summary>
        /// Obtém uma mídia pelo ID
        /// </summary>
        /// <param name="id">ID da mídia</param>
        /// <returns>Mídia ou null se não encontrada</returns>
        Task<Media> GetByIdAsync(Guid id);

        /// <summary>
        /// Obtém todas as mídias de um criador
        /// </summary>
        /// <param name="creatorId">ID do criador</param>
        /// <returns>Lista de mídias</returns>
        Task<List<Media>> GetByCreatorIdAsync(Guid creatorId);

        /// <summary>
        /// Atualiza uma mídia existente
        /// </summary>
        /// <param name="media">Mídia com as alterações</param>
        /// <returns>Mídia atualizada</returns>
        Task<Media> UpdateAsync(Media media);

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
