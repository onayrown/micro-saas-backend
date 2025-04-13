using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.IntegrationTests.Mocks.Repositories
{
    public class MockUserRepository : IUserRepository
    {
        public Task<User?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Parse("10000000-1000-1000-1000-100000000000"))
            {
                return Task.FromResult<User?>(new User
                {
                    Id = id,
                    Username = "Test User",
                    Name = "Test User",
                    Email = "test@example.com",
                    PasswordHash = "hashed_password",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            return Task.FromResult<User?>(null);
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            if (email == "test@example.com")
            {
                return Task.FromResult<User?>(new User
                {
                    Id = Guid.Parse("10000000-1000-1000-1000-100000000000"),
                    Username = "Test User",
                    Name = "Test User",
                    Email = email,
                    PasswordHash = "hashed_password",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            return Task.FromResult<User?>(null);
        }

        public Task<User?> GetByUsernameAsync(string username)
        {
            if (username == "testuser")
            {
                return Task.FromResult<User?>(new User
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    Email = "test@example.com",
                    PasswordHash = "hashedpassword",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            return Task.FromResult<User?>(null);
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<User>>(new List<User>());
        }

        public Task AddAsync(User user)
        {
            // Apenas retorna uma tarefa concluída
            return Task.CompletedTask;
        }

        public Task UpdateAsync(User user)
        {
            // Apenas retorna uma tarefa concluída
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            return Task.CompletedTask;
        }

        public Task<bool> IsEmailUniqueAsync(string email)
        {
            // Considera o email único se não for test@example.com
            return Task.FromResult(email != "test@example.com");
        }

        public Task<bool> IsUsernameUniqueAsync(string username)
        {
            // Considera o username único se não for testuser
            return Task.FromResult(username != "testuser");
        }
    }
} 