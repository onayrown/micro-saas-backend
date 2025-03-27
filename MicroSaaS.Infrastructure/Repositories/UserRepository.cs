using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Data;
using MicroSaaS.Infrastructure.Entities;
using MicroSaaS.Infrastructure.Mappers;
using MongoDB.Driver;

namespace MicroSaaS.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<UserEntity> _users;

    public UserRepository(MongoDbContext context)
    {
        _users = context.Users;
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        var userEntity = await _users
            .Find(u => u.Username == username)
            .FirstOrDefaultAsync();

        return userEntity?.ToDomain();
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        var userEntity = await _users
            .Find(u => u.Email == email)
            .FirstOrDefaultAsync();

        return userEntity?.ToDomain();
    }

    public async Task<User> CreateAsync(User user)
    {
        var userEntity = user.ToEntity();
        await _users.InsertOneAsync(userEntity);
        return userEntity.ToDomain();
    }

    public async Task UpdateAsync(User user)
    {
        var userEntity = user.ToEntity();
        await _users.ReplaceOneAsync(
            u => u.Id == userEntity.Id,
            userEntity,
            new ReplaceOptions { IsUpsert = true }
        );
    }

    public async Task<bool> CheckUsernameExistsAsync(string username)
    {
        var count = await _users
            .CountDocumentsAsync(u => u.Username == username);
        return count > 0;
    }

    public async Task<bool> CheckEmailExistsAsync(string email)
    {
        var count = await _users
            .CountDocumentsAsync(u => u.Email == email);
        return count > 0;
    }
}