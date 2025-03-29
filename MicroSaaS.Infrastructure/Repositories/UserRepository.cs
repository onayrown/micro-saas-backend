using MicroSaaS.Domain.Entities;
using MicroSaaS.Domain.Repositories;
using MicroSaaS.Infrastructure.Database;
using MicroSaaS.Infrastructure.Entities;
using MicroSaaS.Infrastructure.Mappers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserEntity> _users;

        public UserRepository(IMongoDbContext context)
        {
            _users = context.Users;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var user = await _users
                .Find(u => u.Id == id.ToString())
                .FirstOrDefaultAsync();

            return user?.ToDomain();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var user = await _users
                .Find(u => u.Email == email)
                .FirstOrDefaultAsync();

            return user?.ToDomain();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var user = await _users
                .Find(u => u.Username == username)
                .FirstOrDefaultAsync();

            return user?.ToDomain();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _users
                .Find(_ => true)
                .ToListAsync();

            return users.Select(u => u.ToDomain());
        }

        public async Task<User> AddAsync(User user)
        {
            var entity = user.ToEntity();
            await _users.InsertOneAsync(entity);
            return entity.ToDomain();
        }

        public async Task<User> UpdateAsync(User user)
        {
            var entity = user.ToEntity();
            entity.UpdatedAt = DateTime.UtcNow;

            var result = await _users.ReplaceOneAsync(
                u => u.Id == user.Id.ToString(),
                entity);

            return result.IsAcknowledged ? entity.ToDomain() : null;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var result = await _users.DeleteOneAsync(u => u.Id == id.ToString());
            return result.IsAcknowledged && result.DeletedCount > 0;
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