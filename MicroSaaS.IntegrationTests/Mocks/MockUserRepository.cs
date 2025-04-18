using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.IntegrationTests.Mocks
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
                    PasswordHash = "hashed_password", // Note: Em produção, nunca use senhas fixas
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
                    Id = Guid.NewGuid(), // Gera um novo ID para simular criação
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
            // Retorna uma lista vazia ou com usuários pré-definidos, se necessário
            return Task.FromResult<IEnumerable<User>>(new List<User>());
        }

        public Task AddAsync(User user)
        {
            // Simula a adição, mas não persiste dados no mock
            Console.WriteLine($"Mock AddAsync called for user: {user.Username}");
            return Task.CompletedTask;
        }

        public Task UpdateAsync(User user)
        {
            // Simula a atualização
            Console.WriteLine($"Mock UpdateAsync called for user: {user.Username}");
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            // Simula a exclusão
             Console.WriteLine($"Mock DeleteAsync called for user ID: {id}");
            return Task.CompletedTask;
        }

        public Task<bool> IsEmailUniqueAsync(string email)
        {
            // Considera o email único se não for o email de teste padrão
            return Task.FromResult(email != "test@example.com");
        }

        public Task<bool> IsUsernameUniqueAsync(string username)
        {
            // Considera o username único se não for o username de teste padrão
            return Task.FromResult(username != "testuser");
        }
    }
} 