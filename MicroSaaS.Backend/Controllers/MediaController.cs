using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Models;
using MicroSaaS.Backend.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MicroSaaS.Backend.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;
        private readonly IContentCreatorRepository _creatorRepository;
        private readonly ILoggingService _loggingService;

        public MediaController(
            IMediaService mediaService,
            IContentCreatorRepository creatorRepository,
            ILoggingService loggingService)
        {
            _mediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));
            _creatorRepository = creatorRepository ?? throw new ArgumentNullException(nameof(creatorRepository));
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        }

        /// <summary>
        /// Faz upload de arquivos de mídia
        /// </summary>
        /// <param name="creatorId">ID do criador de conteúdo</param>
        /// <returns>Lista de URLs das mídias enviadas</returns>
        [HttpPost("upload")]
        [RequestSizeLimit(100 * 1024 * 1024)] // 100 MB
        [ProducesResponseType(typeof(ApiResponse<List<MediaDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [RateLimit(10, "60s")] // 10 uploads por minuto
        public async Task<IActionResult> UploadMedia([FromForm] string creatorId)
        {
            try
            {
                _loggingService.LogInformation("Iniciando upload de mídia para o criador {CreatorId}", creatorId);

                // Validar o ID do criador
                if (!Guid.TryParse(creatorId, out Guid creatorGuid))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "ID do criador inválido"
                    });
                }

                // Verificar se o criador existe
                var creator = await _creatorRepository.GetByIdAsync(creatorGuid);
                if (creator == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Criador não encontrado"
                    });
                }

                // Verificar se há arquivos no request
                if (Request.Form.Files == null || Request.Form.Files.Count == 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Nenhum arquivo enviado"
                    });
                }

                // Fazer upload dos arquivos
                var mediaList = await _mediaService.UploadMediaAsync(Request.Form.Files, creatorGuid);

                return Ok(new ApiResponse<List<MediaDto>>
                {
                    Success = true,
                    Data = mediaList,
                    Message = $"{mediaList.Count} arquivo(s) enviado(s) com sucesso"
                });
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, "Erro ao fazer upload de mídia: {Message}", ex.Message);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Erro ao processar o upload de mídia"
                });
            }
        }

        /// <summary>
        /// Obtém todas as mídias de um criador
        /// </summary>
        /// <param name="creatorId">ID do criador de conteúdo</param>
        /// <returns>Lista de mídias do criador</returns>
        [HttpGet("creator/{creatorId}")]
        [ProducesResponseType(typeof(ApiResponse<List<MediaDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [Cache("media:creator:{creatorId}", minutes: 15)]
        public async Task<IActionResult> GetMediaByCreator(Guid creatorId)
        {
            try
            {
                _loggingService.LogInformation("Obtendo mídias do criador {CreatorId}", creatorId);

                // Verificar se o criador existe
                var creator = await _creatorRepository.GetByIdAsync(creatorId);
                if (creator == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Criador não encontrado"
                    });
                }

                // Obter as mídias do criador
                var mediaList = await _mediaService.GetMediaByCreatorAsync(creatorId);

                return Ok(new ApiResponse<List<MediaDto>>
                {
                    Success = true,
                    Data = mediaList,
                    Message = $"{mediaList.Count} mídia(s) encontrada(s)"
                });
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, "Erro ao obter mídias do criador: {Message}", ex.Message);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Erro ao obter mídias do criador"
                });
            }
        }

        /// <summary>
        /// Exclui uma mídia
        /// </summary>
        /// <param name="id">ID da mídia</param>
        /// <returns>Status da operação</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMedia(Guid id)
        {
            try
            {
                _loggingService.LogInformation("Excluindo mídia {MediaId}", id);

                // Verificar se a mídia existe
                var media = await _mediaService.GetMediaByIdAsync(id);
                if (media == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Mídia não encontrada"
                    });
                }

                // Excluir a mídia
                var result = await _mediaService.DeleteMediaAsync(id);

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = result,
                    Message = "Mídia excluída com sucesso"
                });
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, "Erro ao excluir mídia: {Message}", ex.Message);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Erro ao excluir mídia"
                });
            }
        }

        /// <summary>
        /// Obtém uma mídia pelo ID
        /// </summary>
        /// <param name="id">ID da mídia</param>
        /// <returns>Detalhes da mídia</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<MediaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [Cache("media:{id}", minutes: 30)]
        public async Task<IActionResult> GetMediaById(Guid id)
        {
            try
            {
                _loggingService.LogInformation("Obtendo mídia {MediaId}", id);

                // Obter a mídia
                var media = await _mediaService.GetMediaByIdAsync(id);
                if (media == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Mídia não encontrada"
                    });
                }

                return Ok(new ApiResponse<MediaDto>
                {
                    Success = true,
                    Data = media,
                    Message = "Mídia encontrada com sucesso"
                });
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, "Erro ao obter mídia: {Message}", ex.Message);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Erro ao obter mídia"
                });
            }
        }
    }
}
