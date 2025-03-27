using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MongoDB.Driver;
using System.Net.Http;

namespace MicroSaaS.IntegrationTests;

public class SanityTests : IClassFixture<WebApplicationFactory<MicroSaaS.Backend.Program>>
{
    private readonly WebApplicationFactory<MicroSaaS.Backend.Program> _factory;

    public SanityTests(WebApplicationFactory<MicroSaaS.Backend.Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remover e substituir serviços problemáticos
                RemoveAndReplaceProblematicServices(services);
            });
        });
    }

    private void RemoveAndReplaceProblematicServices(IServiceCollection services)
    {
        // Remover os serviços existentes
        RemoveService<ISocialMediaAccountRepository>(services);
        RemoveService<ISocialMediaIntegrationService>(services);
        RemoveService<IContentPlanningService>(services);
        
        // Adicionar mocks para as dependências necessárias
        services.AddSingleton(new Mock<IMongoDatabase>().Object);
        services.AddSingleton(new HttpClient());
        
        // Adicionar mocks para os serviços
        services.AddScoped(_ => new Mock<ISocialMediaAccountRepository>().Object);
        services.AddScoped(_ => new Mock<ISocialMediaIntegrationService>().Object);
        services.AddScoped(_ => new Mock<IContentPlanningService>().Object);
    }
    
    private void RemoveService<T>(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
    }

    [Fact]
    public void SimpleTest_AlwaysPasses()
    {
        // Arrange
        const bool expected = true;

        // Act
        const bool actual = true;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task WebApplicationFactory_CanCreateClient()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act & Assert - Apenas verifica se o cliente foi criado sem erros
        Assert.NotNull(client);
    }
} 