using MicroSaaS.Domain.Entities;

namespace MicroSaaS.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User> GetByUsernameAsync(string username);
    Task<User> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> CheckUsernameExistsAsync(string username);
    Task<bool> CheckEmailExistsAsync(string email);
}
