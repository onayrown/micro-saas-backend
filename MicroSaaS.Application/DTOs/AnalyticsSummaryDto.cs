namespace MicroSaaS.Application.DTOs;

/// <summary>
/// DTO para resumo de análise de desempenho em todas as plataformas
/// </summary>
public class AnalyticsSummaryDto
{
    /// <summary>
    /// Número total de seguidores em todas as plataformas
    /// </summary>
    public int TotalFollowers { get; set; }
    
    /// <summary>
    /// Crescimento de seguidores no período
    /// </summary>
    public int FollowersGrowth { get; set; }
    
    /// <summary>
    /// Percentual de crescimento de seguidores
    /// </summary>
    public double FollowersGrowthPercentage { get; set; }
    
    /// <summary>
    /// Taxa média de engajamento em todas as plataformas
    /// </summary>
    public double AverageEngagementRate { get; set; }
    
    /// <summary>
    /// Total de posts publicados no período
    /// </summary>
    public int TotalPosts { get; set; }
    
    /// <summary>
    /// Total de engajamentos (likes, comentários, compartilhamentos) no período
    /// </summary>
    public int TotalEngagements { get; set; }
    
    /// <summary>
    /// Detalhamento de estatísticas por plataforma
    /// </summary>
    public Dictionary<string, PlatformStatsDto> PlatformBreakdown { get; set; } = new();
    
    /// <summary>
    /// Plataforma com melhor desempenho no período
    /// </summary>
    public string BestPerformingPlatform { get; set; } = string.Empty;
    
    /// <summary>
    /// Plataforma com crescimento mais rápido no período
    /// </summary>
    public string FastestGrowingPlatform { get; set; } = string.Empty;
}

/// <summary>
/// DTO para estatísticas de uma plataforma específica
/// </summary>
public class PlatformStatsDto
{
    /// <summary>
    /// Número de seguidores na plataforma
    /// </summary>
    public int Followers { get; set; }
    
    /// <summary>
    /// Taxa de engajamento média na plataforma
    /// </summary>
    public double Engagement { get; set; }
    
    /// <summary>
    /// Número de posts publicados na plataforma no período
    /// </summary>
    public int Posts { get; set; }
} 