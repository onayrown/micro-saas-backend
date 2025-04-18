using MicroSaaS.Shared.DTOs; // Usar o DTO do Shared
using MicroSaaS.Shared.Models; // Para ApiResponse
using System;
using System.Threading.Tasks;

namespace MicroSaaS.Application.Interfaces.Services
{
    public interface IContentCreatorService
    {
        Task<ContentCreatorDto?> GetByIdAsync(Guid id);
        Task<ContentCreatorDto> CreateAsync(ContentCreatorDto creatorDto);
        Task<bool> UpdateAsync(Guid id, ContentCreatorDto creatorDto);
        Task<bool> DeleteAsync(Guid id);
        Task<ApiResponse<ContentCreatorDto>> GetCurrentCreatorAsync(Guid userId);
        // Adicionar outras assinaturas conforme necessário
    }
} 