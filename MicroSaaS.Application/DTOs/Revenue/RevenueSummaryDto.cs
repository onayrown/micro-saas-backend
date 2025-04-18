using System;
using System.Collections.Generic;

namespace MicroSaaS.Application.DTOs.Revenue
{
    public class RevenueSummaryDto
    {
        public Guid Id { get; set; }
        public Guid CreatorId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalRevenue { get; set; } // Propriedade TotalRevenue inferida
        public string Currency { get; set; } = "BRL"; // Propriedade Currency inferida
        public decimal AdSenseRevenue { get; set; } // Propriedade AdSenseRevenue inferida
        public decimal SponsorshipsRevenue { get; set; } // Propriedade SponsorshipsRevenue inferida
        public decimal AffiliateRevenue { get; set; } // Propriedade AffiliateRevenue inferida
        public List<PlatformRevenueDto> RevenueByPlatform { get; set; } = new(); // Propriedade RevenueByPlatform inferida
        public decimal PreviousPeriodRevenue { get; set; } // Propriedade PreviousPeriodRevenue inferida
        public decimal RevenueGrowth { get; set; } // Propriedade RevenueGrowth inferida
        public decimal ProjectedRevenue { get; set; } // Propriedade ProjectedRevenue inferida
        public DateTime GeneratedAt { get; set; }
    }
} 