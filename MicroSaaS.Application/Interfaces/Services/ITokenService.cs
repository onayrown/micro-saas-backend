using MicroSaaS.Domain.Entities;

namespace MicroSaaS.Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}
