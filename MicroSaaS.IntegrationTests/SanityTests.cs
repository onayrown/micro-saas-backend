using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Xunit;

namespace MicroSaaS.IntegrationTests;

public class SanityTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public SanityTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
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
    public void WebApplicationFactory_CanCreateClient()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act & Assert - Apenas verifica se o cliente foi criado sem erros
        Assert.NotNull(client);
    }
} 