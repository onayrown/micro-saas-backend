namespace MicroSaaS.Domain.Interfaces;

// Interface para hashers de senha na camada de aplicação/infraestrutura
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string providedPassword, string storedPassword);
}
