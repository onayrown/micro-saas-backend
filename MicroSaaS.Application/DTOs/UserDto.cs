using System;

namespace MicroSaaS.Application.DTOs
{
    // DTO para retornar informações básicas do usuário após login/registro
    public class UserDto
    {
        public string Id { get; set; } = string.Empty; // Id deve ser string (ObjectId)
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
