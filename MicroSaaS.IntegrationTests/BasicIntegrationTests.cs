using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Xunit;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using MicroSaaS.Application.Interfaces.Services;
using FluentAssertions;

namespace MicroSaaS.IntegrationTests
{
    public class BasicIntegrationTests : IClassFixture<SimplifiedTestFactory>
    {
        private readonly HttpClient _client;
        private readonly SimplifiedTestFactory _factory;

        public BasicIntegrationTests(SimplifiedTestFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            
            // Configurar token para os testes
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0ZUBtaWNyb3NhYXMuY29tIiwianRpIjoiYWJjMTIzIiwiaWF0IjoxNTE2MjM5MDIyLCJuYmYiOjE1MTYyMzkwMjIsImV4cCI6MjUxNjIzOTAyMiwiYXVkIjoibWljcm9zYWFzLmNvbSIsImlzcyI6Im1pY3Jvc2Fhcy5jb20ifQ.VY6JUOK9gH3AQJl0KEhHYQ5MURKVc18WA5qmVxpWHaE";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
            // Arrange & Act
            var client = _factory.CreateClient();

            // Assert
            Assert.NotNull(client);
        }

        [Fact]
        public async Task HealthCheck_ReturnsOk()
        {
            // Arrange
            var expected = HttpStatusCode.OK;

            // Act
            var response = await _client.GetAsync("/api/health");

            // Assert
            response.StatusCode.Should().Be(expected);
        }
    }
} 