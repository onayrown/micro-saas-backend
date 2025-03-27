namespace MicroSaaS.Shared.Models;

public class RevenueSummary
{
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRecurringRevenue { get; set; }
    public decimal AnnualRecurringRevenue { get; set; }
    public int TotalSubscribers { get; set; }
    public decimal AverageRevenuePerUser { get; set; }
}

public class DailyRevenue
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
}

public class PlatformRevenue
{
    public string Platform { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public int Subscribers { get; set; }
} 