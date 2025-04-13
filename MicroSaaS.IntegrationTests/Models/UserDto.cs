using System;

namespace MicroSaaS.IntegrationTests.Models
{
    /// <summary>
    /// DTO de usuário específico para testes de integração
    /// Mantém compatibilidade com a entidade User do domínio, mas usa string para Id
    /// para facilitar a serialização/desserialização e evitar problemas de conversão.
    /// 
    /// Esta abordagem segue os princípios da Clean Architecture, onde:
    /// - DTOs são adaptados às necessidades de cada camada
    /// - A camada de domínio mantém sua integridade (usando Guid para Id)
    /// - A camada de apresentação/testes usa o formato mais conveniente (string)
    /// </summary>
    public class UserDto
    {
        // Propriedades conforme modelo da aplicação principal
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        
        // Propriedades adicionais usadas nos testes
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
} 