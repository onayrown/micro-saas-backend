using System;
using System.Collections.Generic;

namespace MicroSaaS.Application.DTOs.ContentAnalysis
{
    public class ContentAnalysisDto
    {
        public Guid ContentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public double EngagementScore { get; set; }
        public double ReachScore { get; set; }
        public List<string> StrengthPoints { get; set; } = new();
        public List<string> ImprovementSuggestions { get; set; } = new();
        public DateTime AnalysisDate { get; set; }
        // Adicionar outras propriedades conforme necessário
    }
} 