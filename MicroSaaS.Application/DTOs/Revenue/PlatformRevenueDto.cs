using MicroSaaS.Shared.Enums; // Assumindo que SocialMediaPlatform está aqui
using System;

namespace MicroSaaS.Application.DTOs.Revenue
{
    public class PlatformRevenueDto
    {
        public Guid Id { get; set; }
        public Guid CreatorId { get; set; }
        public SocialMediaPlatform Platform { get; set; }
        public decimal Amount { get; set; } // Propriedade Amount esperada
        public string Currency { get; set; } = "BRL";
        public DateTime CalculationDate { get; set; }
    }
} 