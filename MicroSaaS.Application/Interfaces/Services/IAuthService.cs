using MicroSaaS.Domain.Entities;
using MicroSaaS.Application.Services;

namespace MicroSaaS.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthenticationResult> RegisterAsync(string username, string email, string password);
    Task<AuthenticationResult> LoginAsync(string username, string password);
    Task<string> GenerateJwtTokenAsync(User user);
    Task<bool> ValidateUserCredentialsAsync(string username, string password);
}
