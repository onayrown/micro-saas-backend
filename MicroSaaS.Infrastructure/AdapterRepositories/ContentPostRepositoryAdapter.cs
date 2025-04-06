using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Domain.Repositories;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.AdapterRepositories
{
    public class ContentPostRepositoryAdapter : Application.Interfaces.Repositories.IContentPostRepository
    {
        private readonly Domain.Repositories.IContentPostRepository _domainRepository;

        public ContentPostRepositoryAdapter(Domain.Repositories.IContentPostRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public async Task<ContentPost> GetByIdAsync(Guid id)
        {
            return await _domainRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ContentPost>> GetAllAsync()
        {
            return await _domainRepository.GetAllAsync();
        }

        public async Task<ContentPost> AddAsync(ContentPost post)
        {
            return await _domainRepository.AddAsync(post);
        }

        public async Task<ContentPost> UpdateAsync(ContentPost post)
        {
            return await _domainRepository.UpdateAsync(post);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _domainRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ContentPost>> GetByCreatorIdAsync(Guid creatorId)
        {
            return await _domainRepository.GetByCreatorIdAsync(creatorId);
        }

        public async Task<IEnumerable<ContentPost>> GetByStatusAsync(PostStatus status)
        {
            return await _domainRepository.GetByStatusAsync(status);
        }

        public async Task<IEnumerable<ContentPost>> GetByScheduledTimeRangeAsync(DateTime start, DateTime end)
        {
            // Adaptação usando o método do domínio com parâmetros from e to
            return await _domainRepository.GetScheduledByCreatorIdAsync(Guid.Empty, start, end);
        }

        public async Task<IEnumerable<ContentPost>> GetScheduledPostsAsync(Guid creatorId)
        {
            return await _domainRepository.GetScheduledByCreatorIdAsync(creatorId);
        }

        public async Task<IEnumerable<ContentPost>> GetScheduledByCreatorIdAsync(Guid creatorId)
        {
            return await _domainRepository.GetScheduledByCreatorIdAsync(creatorId);
        }

        public async Task<int> CountAsync()
        {
            var posts = await _domainRepository.GetAllAsync();
            return posts.Count();
        }

        public async Task<int> CountByCreatorAsync(Guid creatorId)
        {
            var posts = await _domainRepository.GetByCreatorIdAsync(creatorId);
            return posts.Count();
        }

        public async Task<IEnumerable<ContentPost>> GetByCreatorIdBetweenDatesAsync(Guid creatorId, DateTime startDate, DateTime endDate)
        {
            // Verificar se o repositório de domínio tem implementação equivalente
            try
            {
                // Se o repositório de domínio implementa o método, usar diretamente
                if (_domainRepository is Domain.Repositories.IContentPostRepository domainRepo 
                    && domainRepo.GetType().GetMethod("GetByCreatorIdBetweenDatesAsync") != null)
                {
                    // Usar reflexão para invocar de forma segura
                    var method = domainRepo.GetType().GetMethod("GetByCreatorIdBetweenDatesAsync");
                    if (method != null)
                    {
                        return await (Task<IEnumerable<ContentPost>>)method.Invoke(domainRepo, new object[] { creatorId, startDate, endDate });
                    }
                }
                
                // Caso contrário, buscar todos os posts do criador e filtrar por datas
                var posts = await _domainRepository.GetByCreatorIdAsync(creatorId);
                return posts.Where(p => p.PublishedAt.HasValue && 
                                      p.PublishedAt.Value >= startDate && 
                                      p.PublishedAt.Value <= endDate);
            }
            catch (Exception ex)
            {
                // Log do erro e retorno de lista vazia
                Console.WriteLine($"Erro ao buscar posts do criador {creatorId} entre {startDate} e {endDate}: {ex.Message}");
                return Enumerable.Empty<ContentPost>();
            }
        }
    }
} 