using MicroSaaS.Application.Interfaces.Services;
using BC = BCrypt.Net.BCrypt;

namespace MicroSaaS.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BC.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BC.Verify(password, hashedPassword);
    }
} 