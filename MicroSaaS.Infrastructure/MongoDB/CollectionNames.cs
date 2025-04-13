namespace MicroSaaS.Infrastructure.MongoDB
{
    /// <summary>
    /// Classe que define os nomes padrão de todas as coleções no MongoDB.
    /// Todas as coleções seguem o padrão PascalCase (primeira letra maiúscula).
    /// </summary>
    public static class CollectionNames
    {
        public const string Users = "Users";
        public const string ContentCreators = "ContentCreators";
        public const string ContentPosts = "ContentPosts";
        public const string SocialMediaAccounts = "SocialMediaAccounts";
        public const string ContentPerformances = "ContentPerformances"; // Nome para PerformanceMetrics
        public const string ContentRecommendations = "ContentRecommendations";
        public const string ContentChecklists = "ContentChecklists";
        public const string Analytics = "Analytics"; // Usado em algum lugar?
        public const string DashboardInsights = "DashboardInsights";
        public const string Schedules = "Schedules"; // Usado em algum lugar?
        // Adicionar se não existir:
        public const string DailyRevenues = "DailyRevenues"; 
    }
} 