using MicroSaaS.Application.DTOs.ContentPost;
using MicroSaaS.Shared.Models; // Para ApiResponse
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Application.Interfaces.Services
{
    public interface IContentPostService
    {
        Task<ApiResponse<List<ContentPostDto>>> GetScheduledPostsAsync(Guid creatorId);
        Task<ApiResponse<List<ContentPostDto>>> GetPostsAsync(Guid creatorId, string? status);
        Task<ApiResponse<ContentPostDto>> CreatePostAsync(CreatePostRequest request, Guid requestingUserId);
        Task<ApiResponse<ContentPostDto>> GetPostByIdAsync(Guid id, Guid requestingUserId);
        Task<ApiResponse<bool>> UpdatePostAsync(Guid id, UpdatePostRequest request, Guid requestingUserId);
        Task<ApiResponse<bool>> DeletePostAsync(Guid id, Guid requestingUserId);
        Task<ApiResponse<ContentPostDto>> PublishPostAsync(Guid id, Guid requestingUserId);
        Task<ApiResponse<List<ContentPostDto>>> GetPostsByCreatorAsync(Guid creatorId, Guid requestingUserId);
        // Adicionar outras assinaturas conforme necessário
    }
} 