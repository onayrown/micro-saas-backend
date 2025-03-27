using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Models;

namespace MicroSaaS.Application.Interfaces.Services;

public interface IRevenueService
{
    // Conexão com AdSense
    Task<string> GetAdSenseAuthUrlAsync(Guid creatorId, string callbackUrl);
    Task<bool> ConnectAdSenseAsync(Guid creatorId, string authorizationCode);
    Task<bool> IntegrateGoogleAdSenseAsync(ContentCreator creator, string accessToken);
    
    // Relatórios de Receita
    Task<decimal> GetEstimatedRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate);
    Task<RevenueSummary> GetRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate);
    Task<List<PlatformRevenue>> GetRevenueByPlatformAsync(Guid creatorId, DateTime startDate, DateTime endDate);
    Task<List<DailyRevenue>> GetRevenueByDayAsync(Guid creatorId, DateTime startDate, DateTime endDate);

    Task<RevenueSummary> GetRevenueSummaryAsync(Guid creatorId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<DailyRevenue>> GetDailyRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<PlatformRevenue>> GetPlatformRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate);
    Task<decimal> GetTotalRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate);
}

public class RevenueSummary
{
    public decimal TotalRevenue { get; set; }
    public decimal EstimatedMonthlyRevenue { get; set; }
    public decimal AverageRevenuePerView { get; set; }
}

public class PlatformRevenue
{
    public required string Platform { get; set; }
    public decimal Revenue { get; set; }
    public long Views { get; set; }
}

public class DailyRevenue
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public long Views { get; set; }
}
