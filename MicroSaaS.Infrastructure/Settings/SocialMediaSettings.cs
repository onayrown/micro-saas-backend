namespace MicroSaaS.Infrastructure.Settings;

public class SocialMediaSettings
{
    public InstagramSettings Instagram { get; set; } = new();
    public YouTubeSettings YouTube { get; set; } = new();
    public TikTokSettings TikTok { get; set; } = new();
}

public class InstagramSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

public class YouTubeSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

public class TikTokSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
} 