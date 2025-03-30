using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using Xunit;
using FluentAssertions;
using Xunit.Abstractions;

namespace MicroSaaS.IntegrationTests
{
    public class SocialMediaAccountTest_Fixed : IClassFixture<SimplifiedTestFactory>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        private readonly string _validToken = "valid-token";
        private readonly Guid _existingCreatorId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        public SocialMediaAccountTest_Fixed(SimplifiedTestFactory factory, ITestOutputHelper output)
        {
            _client = factory.CreateClient();
            _output = output;
        }

        [Fact]
        public async Task HealthCheck_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/health");
            _output.WriteLine($"Status code: {response.StatusCode}");
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAccounts_WithValidCreator_ShouldReturnAccounts()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);

            // Act
            var response = await _client.GetAsync($"/api/SocialMediaAccount/{_existingCreatorId}");
            _output.WriteLine($"Status code: {response.StatusCode}");
            
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                _output.WriteLine($"Error content: {content}");
            }
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
} 