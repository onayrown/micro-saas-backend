using MicroSaaS.IntegrationTests.Mocks.Repositories;
using MicroSaaS.IntegrationTests.Mocks.Services;

namespace MicroSaaS.IntegrationTests.Mocks
{
    /// <summary>
    /// Classe estática que fornece acesso a todos os serviços mock
    /// </summary>
    public static class MockServices
    {
        // Repositórios
        public static MockUserRepository CreateUserRepository() => new MockUserRepository();
        public static MockSocialMediaAccountRepository CreateSocialMediaAccountRepository() => new MockSocialMediaAccountRepository();
        
        // Serviços
        public static MockTokenService CreateTokenService() => new MockTokenService();
        public static MockAuthService CreateAuthService() => new MockAuthService();
        public static MockLoggingService CreateLoggingService() => new MockLoggingService();
        public static MockDashboardService CreateDashboardService() => new MockDashboardService();
        public static MockRecommendationService CreateRecommendationService() => new MockRecommendationService();
    }
} 