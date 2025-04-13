using MicroSaaS.Domain.Entities;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Infrastructure.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Namespace ajustado para Persistence.Repositories
namespace MicroSaaS.Infrastructure.Persistence.Repositories 
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoDbContext context)
        {
            _users = context.Users;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var user = await _users
                .Find(u => u.Id == id)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var user = await _users
                .Find(u => u.Email == email)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var user = await _users
                .Find(u => u.Username == username)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _users
                .Find(_ => true)
                .ToListAsync();

            return users;
        }

        public async Task AddAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;

            await _users.ReplaceOneAsync(
                u => u.Id == user.Id,
                user);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _users.DeleteOneAsync(u => u.Id == id);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            var count = await _users
                .CountDocumentsAsync(u => u.Email == email);

            return count == 0;
        }

        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            var count = await _users
                .CountDocumentsAsync(u => u.Username == username);

            return count == 0;
        }
    }
} 