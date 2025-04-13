using MicroSaaS.Application.Interfaces.Services;
// using BC = BCrypt.Net.BCrypt; // Remover referência BCrypt
using Microsoft.AspNetCore.Identity; // Adicionar referência Identity
using MicroSaaS.Domain.Entities; // Precisamos de User para PasswordHasher<User>

namespace MicroSaaS.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    // Instanciar o hasher do Identity
    private readonly PasswordHasher<User> _identityHasher = new PasswordHasher<User>();

    public string HashPassword(string password)
    {
        // return BC.HashPassword(password); // Remover implementação BCrypt
        // Gerar hash usando Identity. O "null" representa o User, 
        // que não é estritamente necessário para a versão padrão do hasher.
        return _identityHasher.HashPassword(null!, password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        // return BC.Verify(password, hashedPassword); // Remover implementação BCrypt
        // Verificar hash usando Identity
        var result = _identityHasher.VerifyHashedPassword(null!, hashedPassword, password);
        // Verificar se o resultado não é Failure
        return result != PasswordVerificationResult.Failed;
    }
} 