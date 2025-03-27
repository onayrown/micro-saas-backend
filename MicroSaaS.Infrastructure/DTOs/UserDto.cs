using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.DTOs
{
    public class UserDto(Guid id, string username, string email)
    {
        public Guid Id { get; set; } = id;
        public string Username { get; set; } = username;
        public string Email { get; set; } = email;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
