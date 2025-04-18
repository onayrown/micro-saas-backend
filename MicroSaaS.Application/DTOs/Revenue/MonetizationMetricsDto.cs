using System;
using System.Collections.Generic;

namespace MicroSaaS.Application.DTOs.Revenue
{
    // DTO para métricas de monetização - Propriedades baseadas no uso anterior
    public class MonetizationMetricsDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal EstimatedMonthlyRevenue { get; set; }
        public decimal EstimatedAnnualRevenue { get; set; }
        public Dictionary<string, decimal> RevenueByPlatform { get; set; } = new();
        public decimal RevenuePerView { get; set; }
        public AdSenseMetricsDto? AdSenseMetrics { get; set; } // Referencia outro DTO
        // Adicionar outras propriedades se necessário
    }

    // DTO para métricas do AdSense - Propriedades baseadas no uso anterior
    public class AdSenseMetricsDto
    {
        public decimal EstimatedMonthlyRevenue { get; set; }
        public int TotalClicks { get; set; }
        public int TotalImpressions { get; set; }
        public decimal Ctr { get; set; } // Click-through rate
        public decimal Rpm { get; set; } // Revenue per mille (thousand impressions)
        public DateTime LastUpdated { get; set; }
    }
} 