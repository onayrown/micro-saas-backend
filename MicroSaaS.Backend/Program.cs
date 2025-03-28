using FluentValidation;
using FluentValidation.AspNetCore;
using MicroSaaS.Application.DTOs.Validators;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Application.Services;
using MicroSaaS.Infrastructure.Data;
using MicroSaaS.Infrastructure.Repositories;
using MicroSaaS.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MicroSaaS.Infrastructure.Persistence;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.DTOs.Auth;
using MicroSaaS.Infrastructure.Settings;
using MicroSaaS.Domain.Interfaces;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using MicroSaaS.Backend.Attributes;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace MicroSaaS.Backend;

// Tornando a classe Program parcial e pública para permitir que o WebApplicationFactory a acesse
public partial class Program
{
    // Este método parcial é necessário para o ASP.NET Core reconhecer a classe corretamente nos testes
    // Não precisa de implementação
    partial void InitializeForTesting();

    public static void Main(string[] args)
    {
        var app = CreateApplication(args);
        app.Run();
    }

    // Método para criar a aplicação, usado para testes
    public static WebApplication CreateApplication(string[] args)
    {
        // Configuração do Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
                theme: AnsiConsoleTheme.Code)
            .WriteTo.File(
                path: "logs/microsaas-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .CreateLogger();

        try
        {
            Log.Information("Iniciando aplicação MicroSaaS");
            
            var builder = WebApplication.CreateBuilder(args);
            
            // Adicionar Serilog ao Host
            builder.Host.UseSerilog();

            // Configuração FluentValidation
            builder.Services.AddValidatorsFromAssemblyContaining<ContentCreatorValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<ContentPostValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

            // Adiciona FluentValidation manualmente ao MVC (versão 11.3.0)
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddMvc();

            // Configuração do MongoDB
            builder.Services.Configure<MicroSaaS.Infrastructure.Settings.MongoDbSettings>(
                builder.Configuration.GetSection("MongoDbSettings"));
            builder.Services.AddScoped<MongoDbContext>();

            // Configuração do JWT
            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("JwtSettings"));
            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

            // Configuração do JWT
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings!.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                    };
                });

            // Configuração das redes sociais
            builder.Services.Configure<SocialMediaSettings>(
                builder.Configuration.GetSection("SocialMedia"));

            // Configuração da receita
            builder.Services.Configure<RevenueSettings>(
                builder.Configuration.GetSection("Revenue"));

            // Registrar dependências do repositório do domínio
            builder.Services.AddScoped<MicroSaaS.Domain.Repositories.IUserRepository, MicroSaaS.Infrastructure.Repositories.UserRepository>();
            builder.Services.AddScoped<MicroSaaS.Domain.Repositories.IContentCreatorRepository, MicroSaaS.Infrastructure.Repositories.ContentCreatorRepository>();
            builder.Services.AddScoped<MicroSaaS.Domain.Repositories.IContentPostRepository, MicroSaaS.Infrastructure.Repositories.ContentPostRepository>();
            builder.Services.AddScoped<MicroSaaS.Domain.Repositories.ISocialMediaAccountRepository, MicroSaaS.Infrastructure.Repositories.SocialMediaAccountRepository>();

            // Registrar adaptadores para conectar interfaces da aplicação às implementações do domínio
            builder.Services.AddScoped<MicroSaaS.Application.Interfaces.Repositories.IUserRepository, MicroSaaS.Infrastructure.AdapterRepositories.UserRepositoryAdapter>();
            builder.Services.AddScoped<MicroSaaS.Application.Interfaces.Repositories.IContentCreatorRepository, MicroSaaS.Infrastructure.AdapterRepositories.ContentCreatorRepositoryAdapter>();
            builder.Services.AddScoped<MicroSaaS.Application.Interfaces.Repositories.IContentPostRepository, MicroSaaS.Infrastructure.AdapterRepositories.ContentPostRepositoryAdapter>();
            builder.Services.AddScoped<MicroSaaS.Application.Interfaces.Repositories.ISocialMediaAccountRepository, MicroSaaS.Infrastructure.AdapterRepositories.SocialMediaAccountRepositoryAdapter>();

            // Registrar outros repositórios
            builder.Services.AddScoped<IContentChecklistRepository, ContentChecklistRepository>();
            builder.Services.AddScoped<MicroSaaS.Application.Interfaces.Repositories.IPerformanceMetricsRepository, PerformanceMetricsRepository>();
            builder.Services.AddScoped<MicroSaaS.Application.Interfaces.Repositories.IDashboardInsightsRepository, DashboardInsightsRepository>();
            builder.Services.AddScoped<MicroSaaS.Application.Interfaces.Repositories.IContentPerformanceRepository, ContentPerformanceRepository>();

            // Registrar serviços
            builder.Services.AddScoped<IAuthService, MicroSaaS.Infrastructure.Services.AuthService>();
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ISocialMediaIntegrationService, SocialMediaIntegrationService>();
            builder.Services.AddScoped<IRevenueService, RevenueService>();
            builder.Services.AddScoped<IContentPlanningService, ContentPlanningService>();
            builder.Services.AddScoped<IPerformanceAnalysisService, MicroSaaS.Infrastructure.Services.PerformanceAnalysisService>();
            builder.Services.AddScoped<ILoggingService, SerilogService>();
            builder.Services.AddScoped<ICacheService, RedisCacheService>();
            builder.Services.AddScoped<IDashboardInsightsService, DashboardInsightsService>();
            builder.Services.AddScoped<ISchedulerService, SchedulerService>();
            builder.Services.AddScoped<IRecommendationService, RecommendationService>();

            // Adicionar serviços hospedados
            builder.Services.AddHostedService<SchedulerService>();

            // Adicionar controllers
            builder.Services.AddControllers();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                // Configuração para incluir documentação XML
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "MicroSaaS API",
                    Version = "v1",
                    Description = "API para gestão de conteúdo para criadores",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Equipe de Desenvolvimento",
                        Email = "devteam@microsaas.com.br"
                    }
                });
                
                // Configuração do JWT para Swagger
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Token JWT de autenticação"
                });
                
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            
            // Configuração de versionamento da API
            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });
            
            builder.Services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            // Configuração do Rate Limiting
            builder.Services.AddMemoryCache();
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
                options.InstanceName = "MicroSaaS:";
            });
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
            builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
            builder.Services.AddInMemoryRateLimiting();
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            // Configuração do Rate Limiting por Endpoint
            builder.Services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RateLimitAttribute(limit: 100, period: "1m"));
            });

            var app = builder.Build();

            // Configurar Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => 
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MicroSaaS API v1");
                    c.RoutePrefix = string.Empty;
                    c.DocExpansion(DocExpansion.List);
                    c.EnableFilter();
                    c.DisplayRequestDuration();
                });
            }

            // Adicionar middleware do Serilog para request logging
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} respondeu {StatusCode} em {Elapsed:0.0000} ms";
            });

            app.UseIpRateLimiting();
            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "A aplicação terminou inesperadamente");
            throw;
        }
    }
}
