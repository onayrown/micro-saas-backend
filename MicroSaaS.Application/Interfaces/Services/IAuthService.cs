using MicroSaaS.Domain.Entities;

namespace MicroSaaS.Application.Interfaces.Services;

public interface IAuthService
{
    Task<string> GenerateJwtTokenAsync(User user);
    Task<bool> ValidateUserCredentialsAsync(string username, string password);
}
