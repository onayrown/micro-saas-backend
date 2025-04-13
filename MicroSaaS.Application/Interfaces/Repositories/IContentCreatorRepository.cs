using MicroSaaS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Application.Interfaces.Repositories;

public interface IContentCreatorRepository
{
    Task<ContentCreator> GetByIdAsync(Guid id);
    Task<IEnumerable<ContentCreator>> GetAllAsync();
    Task<ContentCreator> AddAsync(ContentCreator creator);
    Task<ContentCreator> UpdateAsync(ContentCreator creator);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<ContentCreator>> GetByUserIdAsync(Guid userId);
    Task<ContentCreator?> GetByUsernameAsync(string username);
    Task<bool> IsUsernameUniqueAsync(string username);
    Task<int> CountAsync();
} 