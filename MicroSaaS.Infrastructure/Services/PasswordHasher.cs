using MicroSaaS.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace MicroSaaS.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        // Implementação simples usando SHA256
        // Em produção, considere usar BCrypt ou PBKDF2
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string providedPassword, string storedPassword)
    {
        // Criar hash da senha fornecida e comparar com a armazenada
        var hashedProvidedPassword = HashPassword(providedPassword);
        return hashedProvidedPassword == storedPassword;
    }
} 