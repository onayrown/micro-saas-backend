namespace MicroSaaS.Shared.Enums;

/// <summary>
/// Prioridade da recomendação para o criador de conteúdo
/// </summary>
public enum RecommendationPriority
{
    /// <summary>
    /// Baixa prioridade, sugestão opcional
    /// </summary>
    Low = 0,
    
    /// <summary>
    /// Média prioridade, sugestão recomendada
    /// </summary>
    Medium = 1,
    
    /// <summary>
    /// Alta prioridade, sugestão muito importante
    /// </summary>
    High = 2,
    
    /// <summary>
    /// Prioridade crítica, ação imediata recomendada
    /// </summary>
    Critical = 3
} 