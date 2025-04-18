using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Application.DTOs.Performance;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Application.Interfaces.Services
{
    /// <summary>
    /// Interface para o serviço de Dashboard, que fornece insights e métricas agregadas para criadores de conteúdo
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// Obtém os insights mais recentes para um criador específico
        /// </summary>
        /// <param name="creatorId">ID do criador de conteúdo</param>
        /// <returns>Insights do dashboard</returns>
        Task<DashboardInsights> GetLatestInsightsAsync(Guid creatorId);

        /// <summary>
        /// Gera novos insights para um criador específico com base em dados atuais
        /// </summary>
        /// <param name="creatorId">ID do criador de conteúdo</param>
        /// <returns>Novos insights gerados</returns>
        Task<DashboardInsights> GenerateInsightsAsync(Guid creatorId);

        /// <summary>
        /// Obtém métricas de desempenho para um criador específico
        /// </summary>
        /// <param name="creatorId">ID do criador de conteúdo</param>
        /// <param name="startDate">Data de início opcional para filtrar as métricas</param>
        /// <param name="endDate">Data de fim opcional para filtrar as métricas</param>
        /// <param name="platform">Plataforma opcional para filtrar as métricas</param>
        /// <returns>Lista de métricas de desempenho</returns>
        Task<IEnumerable<PerformanceMetrics>> GetMetricsAsync(Guid creatorId, DateTime? startDate = null, DateTime? endDate = null, SocialMediaPlatform? platform = null);

        /// <summary>
        /// Obtém métricas de desempenho diárias para um criador específico
        /// </summary>
        /// <param name="creatorId">ID do criador de conteúdo</param>
        /// <param name="date">Data específica para obter métricas</param>
        /// <param name="platform">Plataforma específica para obter métricas</param>
        /// <returns>Métricas de desempenho diárias</returns>
        Task<PerformanceMetrics> GetDailyMetricsAsync(Guid creatorId, DateTime? date = null, SocialMediaPlatform platform = SocialMediaPlatform.Instagram);

        /// <summary>
        /// Obtém o conteúdo de melhor desempenho para um criador específico
        /// </summary>
        /// <param name="creatorId">ID do criador de conteúdo</param>
        /// <param name="limit">Número máximo de itens a retornar</param>
        /// <returns>Lista dos principais conteúdos</returns>
        Task<List<ContentPost>> GetTopContentAsync(Guid creatorId, int limit = 5);

        /// <summary>
        /// Obtém recomendações de melhores horários para publicação
        /// </summary>
        /// <param name="creatorId">ID do criador de conteúdo</param>
        /// <param name="platform">Plataforma específica para obter recomendações</param>
        /// <returns>Lista de recomendações de horários (DTO)</returns>
        Task<List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>> GetBestTimeToPostAsync(Guid creatorId, SocialMediaPlatform platform = SocialMediaPlatform.Instagram);

        /// <summary>
        /// Obtém a taxa média de engajamento para um criador específico
        /// </summary>
        /// <param name="creatorId">ID do criador de conteúdo</param>
        /// <param name="platform">Plataforma específica para calcular a taxa</param>
        /// <returns>Taxa média de engajamento</returns>
        Task<decimal> GetAverageEngagementRateAsync(Guid creatorId, SocialMediaPlatform platform = SocialMediaPlatform.Instagram);

        /// <summary>
        /// Obtém o crescimento de receita para um criador específico
        /// </summary>
        /// <param name="creatorId">ID do criador de conteúdo</param>
        /// <param name="startDate">Data de início opcional para o cálculo</param>
        /// <param name="endDate">Data de fim opcional para o cálculo</param>
        /// <returns>Percentual de crescimento de receita</returns>
        Task<decimal> GetRevenueGrowthAsync(Guid creatorId, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Obtém o crescimento de seguidores para um criador específico
        /// </summary>
        /// <param name="creatorId">ID do criador de conteúdo</param>
        /// <param name="platform">Plataforma específica para calcular o crescimento</param>
        /// <param name="startDate">Data de início opcional para o cálculo</param>
        /// <param name="endDate">Data de fim opcional para o cálculo</param>
        /// <returns>Número de novos seguidores no período</returns>
        Task<int> GetFollowerGrowthAsync(Guid creatorId, SocialMediaPlatform platform = SocialMediaPlatform.Instagram, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Adiciona métricas de desempenho para um criador
        /// </summary>
        /// <param name="metricsDto">DTO com as métricas de desempenho a serem adicionadas</param>
        /// <returns>Métricas adicionadas (retornando entidade, pode mudar para DTO)</returns>
        Task<PerformanceMetrics> AddMetricsAsync(DashboardMetricsDto metricsDto);

        /// <summary>
        /// Adiciona dados de desempenho para um conteúdo específico
        /// </summary>
        /// <param name="performanceDto">DTO com os dados de desempenho do conteúdo</param>
        /// <returns>Dados de desempenho adicionados (retornando entidade, pode mudar para DTO)</returns>
        Task<ContentPerformance> AddContentPerformanceAsync(MicroSaaS.Application.DTOs.ContentPerformanceDto performanceDto);
    }
}