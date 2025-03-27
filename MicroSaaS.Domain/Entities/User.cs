using MicroSaaS.Domain.Interfaces;

namespace MicroSaaS.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }

    // Método para validar senha (sem depender de frameworks externos)
    public bool ValidatePassword(string password, IPasswordHasher passwordHasher)
    {
        return passwordHasher.VerifyPassword(password, this.PasswordHash);
    }
}
