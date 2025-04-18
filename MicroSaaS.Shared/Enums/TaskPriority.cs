namespace MicroSaaS.Shared.Enums;

/// <summary>
/// Representa os níveis de prioridade para tarefas e itens de checklist
/// </summary>
public enum TaskPriority
{
    /// <summary>
    /// Prioridade baixa
    /// </summary>
    Low = 0,
    
    /// <summary>
    /// Prioridade média (padrão)
    /// </summary>
    Medium = 1,
    
    /// <summary>
    /// Prioridade alta
    /// </summary>
    High = 2,
    
    /// <summary>
    /// Prioridade urgente
    /// </summary>
    Urgent = 3
}
