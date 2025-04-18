using System;

namespace MicroSaaS.Application.DTOs.Revenue
{
    public class DailyRevenueDto
    {
        public Guid Id { get; set; }
        public Guid CreatorId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; } // Propriedade Amount esperada
        public string Currency { get; set; } = "BRL"; // Moeda padrão
        public string Source { get; set; } = string.Empty; // Origem (opcional)
    }
} 