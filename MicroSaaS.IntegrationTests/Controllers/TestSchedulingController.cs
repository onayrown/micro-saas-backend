using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Models;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MicroSaaS.IntegrationTests.Controllers
{
    [ApiController]
    [Route("api/v1/scheduling")]
    public class TestSchedulingController : ControllerBase
    {
        private readonly ILogger<TestSchedulingController> _logger;
        private static readonly List<ScheduledPostDto> _scheduledPosts = new List<ScheduledPostDto>();

        public TestSchedulingController(ILogger<TestSchedulingController> logger)
        {
            _logger = logger;
        }

        [HttpGet("pending")]
        public ActionResult<List<ScheduledPostDto>> GetScheduledPosts()
        {
            _logger.LogInformation("TestSchedulingController.GetScheduledPosts: Retornando posts agendados");
            return Ok(_scheduledPosts);
        }

        [HttpPost("schedule")]
        public ActionResult<ApiResponse<ScheduledPostDto>> CreateScheduledPost([FromBody] CreateScheduledPostDto request)
        {
            _logger.LogInformation("TestSchedulingController.CreateScheduledPost: Criando post agendado para {ScheduledFor}", request.ScheduledFor);

            var post = new ScheduledPostDto
            {
                Id = Guid.NewGuid(),
                CreatorId = request.CreatorId,
                Title = request.Title,
                Content = request.Content,
                Platform = request.Platform,
                ScheduledFor = request.ScheduledFor,
                MediaUrls = request.MediaUrls,
                Tags = request.Tags,
                Status = PostStatus.Scheduled,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _scheduledPosts.Add(post);

            return Created($"/api/v1/scheduling/{post.Id}", new ApiResponse<ScheduledPostDto>
            {
                Success = true,
                Data = post,
                Message = "Post agendado com sucesso"
            });
        }

        [HttpPut("update")]
        public ActionResult<ApiResponse<ScheduledPostDto>> UpdateScheduledPost([FromBody] UpdateScheduledPostDto request)
        {
            _logger.LogInformation("TestSchedulingController.UpdateScheduledPost: Atualizando post agendado {PostId}", request.Id);

            var post = _scheduledPosts.FirstOrDefault(p => p.Id == request.Id);
            if (post == null)
            {
                return NotFound(new ApiResponse<ScheduledPostDto>
                {
                    Success = false,
                    Message = "Post agendado não encontrado"
                });
            }

            // Atualizar os campos que podem ser alterados
            if (!string.IsNullOrEmpty(request.Title))
                post.Title = request.Title;

            if (!string.IsNullOrEmpty(request.Content))
                post.Content = request.Content;

            if (request.ScheduledFor.HasValue)
                post.ScheduledFor = request.ScheduledFor.Value;

            if (request.Tags != null)
                post.Tags = request.Tags;

            post.UpdatedAt = DateTime.UtcNow;

            return Ok(new ApiResponse<ScheduledPostDto>
            {
                Success = true,
                Data = post,
                Message = "Post agendado atualizado com sucesso"
            });
        }

        [HttpDelete("{id}")]
        public ActionResult<ApiResponse<bool>> DeleteScheduledPost(Guid id)
        {
            _logger.LogInformation("TestSchedulingController.DeleteScheduledPost: Excluindo post agendado {PostId}", id);

            var post = _scheduledPosts.FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Post agendado não encontrado"
                });
            }

            _scheduledPosts.Remove(post);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Data = true,
                Message = "Post agendado excluído com sucesso"
            });
        }

        [HttpPost("cancel/{id}")]
        public ActionResult<ApiResponse<ScheduledPostDto>> CancelScheduledPost(Guid id)
        {
            _logger.LogInformation("TestSchedulingController.CancelScheduledPost: Cancelando post agendado {PostId}", id);

            var post = _scheduledPosts.FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return NotFound(new ApiResponse<ScheduledPostDto>
                {
                    Success = false,
                    Message = "Post agendado não encontrado"
                });
            }

            post.Status = PostStatus.Cancelled;
            post.UpdatedAt = DateTime.UtcNow;

            return Ok(new ApiResponse<ScheduledPostDto>
            {
                Success = true,
                Data = post,
                Message = "Post agendado cancelado com sucesso"
            });
        }
    }
} 