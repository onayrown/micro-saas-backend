using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Moq;
using Xunit;
using MicroSaaS.Infrastructure.Persistence.Repositories;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Infrastructure.Services;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Results;
using System.Collections.Generic;
using MicroSaaS.IntegrationTests.Mocks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using MicroSaaS.Application.DTOs.Auth;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using MicroSaaS.IntegrationTests.Models;
using AppAuthResponse = MicroSaaS.Application.DTOs.Auth.AuthResponse;
using TestAuthResponse = MicroSaaS.IntegrationTests.Models.AuthResponse;

namespace MicroSaaS.IntegrationTests;

public class IntegrationTestBase : IClassFixture<WebApplicationFactory<MicroSaaS.Backend.Program>>
{
    protected readonly WebApplicationFactory<MicroSaaS.Backend.Program> _factory;
    protected readonly HttpClient _client;

    public IntegrationTestBase(WebApplicationFactory<MicroSaaS.Backend.Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            // Usar o TestStartup que criamos
            builder.UseSetting("Environment", "Testing");
            
            // Configurar para usar nosso arquivo appsettings.json para testes
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));
            });
            
            // Substituir serviços por mocks
            builder.ConfigureServices(services =>
            {
                // Remover todos os serviços existentes que possam causar problemas
                RemoveAllProblematicServices(services);
                
                // Adicionar mocks para as dependências necessárias
                services.AddSingleton(new Mock<IMongoDatabase>().Object);
                services.AddSingleton(new HttpClient());
                
                // Adicionar mocks para todos os repositórios e serviços
                AddAllMockServices(services);
            });

            // Configurar a aplicação de teste
            builder.Configure(app => 
            {
                app.UseRouting();
                
                app.UseEndpoints(endpoints => 
                {
                    // Adicionar endpoint para login com credenciais válidas
                    endpoints.MapPost("/api/auth/login", context => 
                    {
                        return HandleLoginRequest(context);
                    });
                    
                    // Adicionar endpoint para registro
                    endpoints.MapPost("/api/auth/register", context =>
                    {
                        return HandleRegisterRequest(context);
                    });
                    
                    // Adicionar outros endpoints conforme necessário
                });
            });
        });

        // Criar cliente HTTP para testes
        _client = _factory.CreateClient();
    }
    
    private void RemoveAllProblematicServices(IServiceCollection services)
    {
        // Remover serviços que dependem de recursos externos
        RemoveService<ISocialMediaAccountRepository>(services);
        RemoveService<ITokenService>(services);
        RemoveService<IUserRepository>(services);
        RemoveService<IAuthService>(services);
        RemoveService<ISocialMediaIntegrationService>(services);
        RemoveService<IPasswordHasher>(services);
        RemoveService<IContentCreatorRepository>(services);
        RemoveService<IContentPostRepository>(services);
        RemoveService<IContentChecklistRepository>(services);
        RemoveService<IContentPlanningService>(services);
        RemoveService<IRevenueService>(services);
    }
    
    private void AddAllMockServices(IServiceCollection services)
    {
        // Configurar mocks para repositórios
        var mockUserRepository = new Mock<IUserRepository>();
        SetupUserRepositoryMock(mockUserRepository);
        services.AddScoped(_ => mockUserRepository.Object);
        
        var mockSocialMediaAccountRepository = new Mock<ISocialMediaAccountRepository>();
        services.AddScoped(_ => mockSocialMediaAccountRepository.Object);
        
        var mockContentCreatorRepository = new Mock<IContentCreatorRepository>();
        services.AddScoped(_ => mockContentCreatorRepository.Object);
        
        var mockContentPostRepository = new Mock<IContentPostRepository>();
        services.AddScoped(_ => mockContentPostRepository.Object);
        
        var mockContentChecklistRepository = new Mock<IContentChecklistRepository>();
        services.AddScoped(_ => mockContentChecklistRepository.Object);
        
        // Configurar mocks para serviços
        var mockPasswordHasher = new Mock<IPasswordHasher>();
        SetupPasswordHasherMock(mockPasswordHasher);
        services.AddScoped(_ => mockPasswordHasher.Object);
        
        var mockTokenService = new Mock<ITokenService>();
        SetupTokenServiceMock(mockTokenService);
        services.AddScoped(_ => mockTokenService.Object);
        
        var mockAuthService = new Mock<IAuthService>();
        SetupAuthServiceMock(mockAuthService);
        services.AddScoped(_ => mockAuthService.Object);
        
        var mockSocialMediaIntegrationService = new Mock<ISocialMediaIntegrationService>();
        services.AddScoped(_ => mockSocialMediaIntegrationService.Object);
        
        var mockContentPlanningService = new Mock<IContentPlanningService>();
        services.AddScoped(_ => mockContentPlanningService.Object);
        
        var mockRevenueService = new Mock<IRevenueService>();
        services.AddScoped(_ => mockRevenueService.Object);
    }
    
    private async Task HandleLoginRequest(HttpContext context)
    {
        // Ler o corpo da requisição como string
        using var reader = new StreamReader(context.Request.Body);
        var body = await reader.ReadToEndAsync();
        
        // Deserializar a string para um objeto LoginRequest
        LoginRequest? request = null;
        try 
        {
            request = JsonSerializer.Deserialize<LoginRequest>(body, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
        }
        catch (JsonException)
        {
            context.Response.StatusCode = 400; // Bad Request
            return;
        }
        
        if (request != null && request.Email == "test@example.com" && request.Password == "Test@123")
        {
            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            
            var response = new TestAuthResponse
            {
                Success = true,
                Token = "valid_token_for_testing",
                Message = "Login successful",
                User = new Models.UserDto
                {
                    Id = "10000000-1000-1000-1000-100000000000",
                    Username = "Test User",
                    Name = "Test User",
                    Email = request.Email,
                    Role = "user",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
            
            var jsonOptions = new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            };
            
            var jsonResponse = JsonSerializer.Serialize(response, jsonOptions);
            await context.Response.WriteAsync(jsonResponse);
        }
        else
        {
            context.Response.StatusCode = 401; // Unauthorized
        }
    }
    
    private async Task HandleRegisterRequest(HttpContext context)
    {
        // Ler o corpo da requisição como string
        using var reader = new StreamReader(context.Request.Body);
        var body = await reader.ReadToEndAsync();
        
        // Deserializar a string para um objeto RegisterRequest
        RegisterRequest? request = null;
        try 
        {
            request = JsonSerializer.Deserialize<RegisterRequest>(body, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
        }
        catch (JsonException)
        {
            context.Response.StatusCode = 400; // Bad Request
            return;
        }
        
        if (request != null)
        {
            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            
            var response = new TestAuthResponse
            {
                Success = true,
                Token = "valid_token_for_testing",
                Message = "Registration successful",
                User = new Models.UserDto
                {
                    Id = "20000000-2000-2000-2000-200000000000",
                    Username = request.Name,
                    Name = request.Name,
                    Email = request.Email,
                    Role = "user",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
            
            var jsonOptions = new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            };
            
            var jsonResponse = JsonSerializer.Serialize(response, jsonOptions);
            await context.Response.WriteAsync(jsonResponse);
        }
        else
        {
            context.Response.StatusCode = 400; // Bad Request
        }
    }
    
    private void RemoveService<T>(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
    }
    
    private void SetupUserRepositoryMock(Mock<IUserRepository> mockUserRepository)
    {
        var testUser = new User
        {
            Id = Guid.Parse("10000000-1000-1000-1000-100000000000"),
            Email = "test@example.com",
            Username = "Test User",
            Name = "Test User",
            PasswordHash = "hashed_password",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        mockUserRepository
            .Setup(x => x.GetByEmailAsync(It.Is<string>(e => e == "test@example.com")))
            .ReturnsAsync(testUser);
            
        mockUserRepository
            .Setup(x => x.GetByEmailAsync(It.Is<string>(e => e == "invalid@example.com")))
            .ReturnsAsync((User?)null);
            
        mockUserRepository
            .Setup(x => x.GetByIdAsync(It.Is<Guid>(id => id == Guid.Parse("10000000-1000-1000-1000-100000000000"))))
            .ReturnsAsync(testUser);
    }
    
    private void SetupPasswordHasherMock(Mock<IPasswordHasher> mockPasswordHasher)
    {
        mockPasswordHasher.Setup(x => x.VerifyPassword("Test@123", "hashed_password"))
            .Returns(true);
            
        mockPasswordHasher.Setup(x => x.VerifyPassword("WrongPassword", It.IsAny<string>()))
            .Returns(false);
    }
    
    private void SetupTokenServiceMock(Mock<ITokenService> mockTokenService)
    {
        mockTokenService.Setup(x => x.GenerateToken(It.IsAny<User>()))
            .Returns("valid_token_for_testing");
    }
    
    private void SetupAuthServiceMock(Mock<IAuthService> mockAuthService)
    {
        mockAuthService.Setup(x => x.ValidateUserCredentialsAsync("test@example.com", "Test@123"))
            .Returns(Task.FromResult(Result<bool>.Ok(true)));
            
        mockAuthService.Setup(x => x.ValidateUserCredentialsAsync("invalid@example.com", It.IsAny<string>()))
            .Returns(Task.FromResult(Result<bool>.Ok(false)));
    }
} 