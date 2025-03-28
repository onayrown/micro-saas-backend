using MicroSaaS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Domain.Repositories
{
    public interface IContentCreatorRepository
    {
        Task<ContentCreator> GetByIdAsync(Guid id);
        Task<ContentCreator> GetByUserIdAsync(Guid userId);
        Task<ContentCreator> GetByUsernameAsync(string username);
        Task<IEnumerable<ContentCreator>> GetAllAsync();
        Task<ContentCreator> AddAsync(ContentCreator creator);
        Task<ContentCreator> UpdateAsync(ContentCreator creator);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> IsUsernameUniqueAsync(string username);
    }
} 