using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.DTOs;

namespace MicroSaaS.Application.Interfaces.Services;

public interface ISchedulerService
{
    /// <summary>
    /// Inicia o serviço de agendamento.
    /// </summary>
    Task StartAsync();
    
    /// <summary>
    /// Para o serviço de agendamento.
    /// </summary>
    Task StopAsync();
    
    /// <summary>
    /// Agenda uma postagem para publicação.
    /// </summary>
    /// <param name="post">A postagem a ser agendada</param>
    /// <returns>A postagem agendada com o status atualizado</returns>
    Task<ContentPost> SchedulePostAsync(ContentPost post);
    
    /// <summary>
    /// Cancela o agendamento de uma postagem.
    /// </summary>
    /// <param name="postId">ID da postagem</param>
    Task CancelScheduledPostAsync(Guid postId);
    
    /// <summary>
    /// Processa todas as postagens que estão agendadas para publicação imediata.
    /// </summary>
    Task ProcessScheduledPostsAsync();
    
    /// <summary>
    /// Retorna todas as postagens agendadas para um período específico.
    /// </summary>
    /// <param name="startDate">Data inicial</param>
    /// <param name="endDate">Data final</param>
    Task<IEnumerable<ContentPost>> GetScheduledPostsInRangeAsync(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Envia notificações para postagens que serão publicadas em breve.
    /// </summary>
    /// <param name="hoursAhead">Número de horas para antecipar a notificação</param>
    Task SendUpcomingPostNotificationsAsync(int hoursAhead = 1);
    
    /// <summary>
    /// Agenda uma postagem para publicação usando um DTO.
    /// </summary>
    /// <param name="request">DTO com dados da postagem a ser agendada</param>
    /// <returns>DTO da postagem agendada</returns>
    Task<ScheduledPostDto> SchedulePostAsync(CreateScheduledPostDto request);
    
    /// <summary>
    /// Obtém uma postagem agendada pelo seu ID.
    /// </summary>
    /// <param name="id">ID da postagem agendada</param>
    /// <returns>DTO da postagem agendada</returns>
    Task<ScheduledPostDto?> GetScheduledPostAsync(Guid id);
    
    /// <summary>
    /// Atualiza uma postagem agendada.
    /// </summary>
    /// <param name="id">ID da postagem agendada</param>
    /// <param name="request">DTO com dados atualizados</param>
    /// <returns>DTO da postagem atualizada</returns>
    Task<ScheduledPostDto?> UpdateScheduledPostAsync(Guid id, UpdateScheduledPostDto request);
    
    /// <summary>
    /// Retorna todas as postagens agendadas para um período específico como uma lista de DTOs.
    /// </summary>
    /// <param name="startDate">Data inicial</param>
    /// <param name="endDate">Data final</param>
    /// <returns>Lista de DTOs das postagens agendadas</returns>
    Task<List<ScheduledPostDto>> GetScheduledPostsInRangeDtoAsync(DateTime startDate, DateTime endDate);
} 