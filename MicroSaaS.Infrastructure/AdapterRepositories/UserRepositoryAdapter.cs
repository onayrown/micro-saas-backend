using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.AdapterRepositories
{
    public class UserRepositoryAdapter : Application.Interfaces.Repositories.IUserRepository
    {
        private readonly Domain.Repositories.IUserRepository _domainRepository;

        public UserRepositoryAdapter(Domain.Repositories.IUserRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _domainRepository.GetByIdAsync(id);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _domainRepository.GetByEmailAsync(email);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _domainRepository.GetAllAsync();
        }

        public async Task<User> AddAsync(User user)
        {
            return await _domainRepository.AddAsync(user);
        }

        public async Task<User> UpdateAsync(User user)
        {
            return await _domainRepository.UpdateAsync(user);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _domainRepository.DeleteAsync(id);
        }
    }
} 