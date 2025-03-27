namespace MicroSaaS.Infrastructure.Settings;

public class RevenueSettings
{
    public AdSenseSettings AdSense { get; set; } = new();
}

public class AdSenseSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
} 