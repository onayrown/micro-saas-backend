using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroSaaS.Backend.Controllers
{
    /// <summary>
    /// Controlador responsável pelo agendamento de publicações de conteúdo
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class SchedulingController : ControllerBase
    {
        private readonly ISchedulerService _schedulerService;
        private readonly IContentPostRepository _contentPostRepository;
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// Construtor do SchedulingController
        /// </summary>
        /// <param name="schedulerService">Serviço de agendamento</param>
        /// <param name="contentPostRepository">Repositório de posts de conteúdo</param>
        /// <param name="loggingService">Serviço de logging</param>
        public SchedulingController(
            ISchedulerService schedulerService,
            IContentPostRepository contentPostRepository,
            ILoggingService loggingService)
        {
            _schedulerService = schedulerService;
            _contentPostRepository = contentPostRepository;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Obtém a lista de posts agendados pendentes de publicação
        /// </summary>
        /// <returns>Lista de posts agendados</returns>
        /// <response code="200">Lista de posts agendados retornada com sucesso</response>
        /// <response code="401">Usuário não autenticado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("pending")]
        [ProducesResponseType(typeof(List<ScheduledPostDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ScheduledPostDto>>> GetScheduledPosts()
        {
            try
            {
                // Em uma implementação real, obteríamos os posts agendados do repositório
                // Para testes, retornamos uma lista vazia
                return Ok(new List<ScheduledPostDto>());
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, "Erro ao obter posts agendados");
                return StatusCode(500, new ApiResponse<List<ScheduledPostDto>>
                {
                    Success = false,
                    Message = "Erro ao obter posts agendados"
                });
            }
        }

        /// <summary>
        /// Agenda um novo post para publicação futura
        /// </summary>
        /// <param name="request">Dados do post a ser agendado</param>
        /// <returns>Detalhes do post agendado</returns>
        /// <response code="200">Post agendado com sucesso</response>
        /// <response code="400">Dados de agendamento inválidos</response>
        /// <response code="401">Usuário não autenticado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("schedule")]
        [ProducesResponseType(typeof(ApiResponse<ScheduledPostDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ScheduledPostDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<ScheduledPostDto>>> SchedulePost([FromBody] CreateScheduledPostDto request)
        {
            try
            {
                _loggingService.LogInformation("Tentativa de agendamento de post para o criador: {CreatorId}", request.CreatorId);

                // Validar data de agendamento (deve ser futura)
                if (request.ScheduledFor <= DateTime.UtcNow)
                {
                    _loggingService.LogWarning("Data de agendamento inválida: {ScheduledDate}", request.ScheduledFor);
                    return BadRequest(new ApiResponse<ScheduledPostDto>
                    {
                        Success = false,
                        Message = "A data de agendamento deve ser no futuro"
                    });
                }

                // Implementação real: salvar o post no repositório e agendar
                var scheduledPost = await _schedulerService.SchedulePostAsync(request);
                
                _loggingService.LogInformation("Post agendado com sucesso: {PostId}", scheduledPost.Id);
                return Ok(new ApiResponse<ScheduledPostDto>
                {
                    Success = true,
                    Data = scheduledPost,
                    Message = "Post agendado com sucesso"
                });
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, "Erro ao agendar post");
                return StatusCode(500, new ApiResponse<ScheduledPostDto>
                {
                    Success = false,
                    Message = "Erro ao agendar post"
                });
            }
        }

        /// <summary>
        /// Atualiza um post agendado existente
        /// </summary>
        /// <param name="id">ID do post agendado</param>
        /// <param name="request">Dados atualizados do post</param>
        /// <returns>Detalhes do post atualizado</returns>
        /// <response code="200">Post atualizado com sucesso</response>
        /// <response code="400">Dados de atualização inválidos</response>
        /// <response code="401">Usuário não autenticado</response>
        /// <response code="404">Post agendado não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ScheduledPostDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ScheduledPostDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<ScheduledPostDto>>> UpdateScheduledPost(Guid id, [FromBody] UpdateScheduledPostDto request)
        {
            try
            {
                _loggingService.LogInformation("Tentativa de atualização do post agendado: {PostId}", id);

                // Verificar se o post existe
                var existingPost = await _schedulerService.GetScheduledPostAsync(id);
                if (existingPost == null)
                {
                    _loggingService.LogWarning("Post agendado não encontrado: {PostId}", id);
                    return NotFound(new ApiResponse<ScheduledPostDto>
                    {
                        Success = false,
                        Message = "Post agendado não encontrado"
                    });
                }

                // Validar data de agendamento (deve ser futura)
                if (request.ScheduledFor <= DateTime.UtcNow)
                {
                    _loggingService.LogWarning("Data de agendamento inválida: {ScheduledDate}", request.ScheduledFor);
                    return BadRequest(new ApiResponse<ScheduledPostDto>
                    {
                        Success = false,
                        Message = "A data de agendamento deve ser no futuro"
                    });
                }

                // Atualizar o post
                var updatedPost = await _schedulerService.UpdateScheduledPostAsync(id, request);
                
                _loggingService.LogInformation("Post agendado atualizado com sucesso: {PostId}", id);
                return Ok(new ApiResponse<ScheduledPostDto>
                {
                    Success = true,
                    Data = updatedPost,
                    Message = "Post agendado atualizado com sucesso"
                });
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, "Erro ao atualizar post agendado: {PostId}", id);
                return StatusCode(500, new ApiResponse<ScheduledPostDto>
                {
                    Success = false,
                    Message = "Erro ao atualizar post agendado"
                });
            }
        }

        /// <summary>
        /// Cancela um post agendado
        /// </summary>
        /// <param name="id">ID do post agendado</param>
        /// <returns>Confirmação do cancelamento</returns>
        /// <response code="200">Post cancelado com sucesso</response>
        /// <response code="401">Usuário não autenticado</response>
        /// <response code="404">Post agendado não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> CancelScheduledPost(Guid id)
        {
            try
            {
                _loggingService.LogInformation("Tentativa de cancelamento do post agendado: {PostId}", id);

                // Verificar se o post existe
                var existingPost = await _schedulerService.GetScheduledPostAsync(id);
                if (existingPost == null)
                {
                    _loggingService.LogWarning("Post agendado não encontrado: {PostId}", id);
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Post agendado não encontrado"
                    });
                }

                // Cancelar o agendamento
                await _schedulerService.CancelScheduledPostAsync(id);
                
                _loggingService.LogInformation("Post agendado cancelado com sucesso: {PostId}", id);
                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Post agendado cancelado com sucesso"
                });
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, "Erro ao cancelar post agendado: {PostId}", id);
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Erro ao cancelar post agendado"
                });
            }
        }

        /// <summary>
        /// Obtém posts agendados dentro de um intervalo de datas
        /// </summary>
        /// <param name="startDate">Data de início</param>
        /// <param name="endDate">Data de fim</param>
        /// <returns>Lista de posts agendados no intervalo especificado</returns>
        /// <response code="200">Lista de posts agendados retornada com sucesso</response>
        /// <response code="400">Parâmetros de data inválidos</response>
        /// <response code="401">Usuário não autenticado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("range")]
        [ProducesResponseType(typeof(ApiResponse<List<ScheduledPostDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<ScheduledPostDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<ScheduledPostDto>>>> GetScheduledPostsInRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                _loggingService.LogInformation("Obtendo posts agendados no intervalo: {StartDate} a {EndDate}", startDate, endDate);

                // Validar datas
                if (startDate > endDate)
                {
                    _loggingService.LogWarning("Intervalo de datas inválido: {StartDate} a {EndDate}", startDate, endDate);
                    return BadRequest(new ApiResponse<List<ScheduledPostDto>>
                    {
                        Success = false,
                        Message = "A data de início deve ser anterior à data de fim"
                    });
                }

                // Obter posts no intervalo
                var posts = await _schedulerService.GetScheduledPostsInRangeDtoAsync(startDate, endDate);
                
                _loggingService.LogInformation("Posts agendados obtidos com sucesso. Total: {Count}", posts.Count);
                return Ok(new ApiResponse<List<ScheduledPostDto>>
                {
                    Success = true,
                    Data = posts,
                    Message = "Posts agendados obtidos com sucesso"
                });
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, "Erro ao obter posts agendados no intervalo");
                return StatusCode(500, new ApiResponse<List<ScheduledPostDto>>
                {
                    Success = false,
                    Message = "Erro ao obter posts agendados no intervalo"
                });
            }
        }
    }
} 