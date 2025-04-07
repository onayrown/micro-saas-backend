using MicroSaaS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Application.Interfaces.Services
{
    /// <summary>
    /// Interface para o serviço de insights do dashboard
    /// </summary>
    public interface IDashboardInsightsService
    {
        /// <summary>
        /// Obtém insights do dashboard por ID
        /// </summary>
        /// <param name="id">ID dos insights</param>
        /// <returns>Insights do dashboard</returns>
        Task<DashboardInsights> GetByIdAsync(Guid id);

        /// <summary>
        /// Obtém todos os insights para um criador específico
        /// </summary>
        /// <param name="creatorId">ID do criador de conteúdo</param>
        /// <returns>Lista de insights do dashboard</returns>
        Task<List<DashboardInsights>> GetByCreatorIdAsync(Guid creatorId);

        /// <summary>
        /// Obtém os insights mais recentes para um criador específico
        /// </summary>
        /// <param name="creatorId">ID do criador de conteúdo</param>
        /// <returns>Insights mais recentes do dashboard</returns>
        Task<DashboardInsights> GetLatestByCreatorIdAsync(Guid creatorId);

        /// <summary>
        /// Gera novos insights para um criador específico
        /// </summary>
        /// <param name="creatorId">ID do criador de conteúdo</param>
        /// <returns>Novos insights gerados</returns>
        Task<DashboardInsights> GenerateInsightsAsync(Guid creatorId);

        /// <summary>
        /// Atualiza insights existentes
        /// </summary>
        /// <param name="insights">Insights a serem atualizados</param>
        /// <returns>Insights atualizados</returns>
        Task<DashboardInsights> UpdateAsync(DashboardInsights insights);

        /// <summary>
        /// Exclui insights por ID
        /// </summary>
        /// <param name="id">ID dos insights</param>
        Task DeleteAsync(Guid id);
    }
}
