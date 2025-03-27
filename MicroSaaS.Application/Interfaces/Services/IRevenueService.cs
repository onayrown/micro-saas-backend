using MicroSaaS.Domain.Entities;

namespace MicroSaaS.Application.Interfaces.Services;

public interface IRevenueService
{
    Task<decimal> GetEstimatedRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate);
    Task<bool> IntegrateGoogleAdSenseAsync(ContentCreator creator, string accessToken);
}
