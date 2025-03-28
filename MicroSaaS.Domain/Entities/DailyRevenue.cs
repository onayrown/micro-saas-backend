using System;

namespace MicroSaaS.Domain.Entities;

public class DailyRevenue
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
} 