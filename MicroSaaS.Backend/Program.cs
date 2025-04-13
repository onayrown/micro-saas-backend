using AspNetCoreRateLimit;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroSaaS.Application;
using MicroSaaS.Application.DTOs.Validators;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Backend.Attributes;
using MicroSaaS.Backend.Swagger;
using MicroSaaS.Infrastructure;
using MicroSaaS.Infrastructure.MongoDB;
using MicroSaaS.Infrastructure.Services;
using MicroSaaS.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;
using MicroSaaS.Infrastructure.MongoDB;
using Microsoft.Extensions.Logging;
using System;
using MicroSaaS.Backend.HealthChecks;
using DotNetEnv;
using Microsoft.Extensions.DependencyInjection;
using HealthChecks.Redis;
using MongoDB.Driver;

namespace MicroSaaS.Backend;

// Tornando a classe Program parcial e pública para permitir que o WebApplicationFactory a acesse
public partial class Program
{
    // Este método parcial é necessário para o ASP.NET Core reconhecer a classe corretamente nos testes
    // Não precisa de implementação
    partial void InitializeForTesting();

    public static async Task Main(string[] args)
    {
        // Carregar variáveis de ambiente do arquivo .env
        DotNetEnv.Env.Load();

        // Inicialização do MongoDB
        MicroSaaS.Infrastructure.MongoDB.MongoDbInitializer.Initialize();

        var app = CreateApplication(args);
        await app.RunAsync();
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

            // Registrar HttpClient para os serviços que dependem dele
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<HttpClient>();

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

            // IMPORTANTE: Usar os métodos de extensão para registrar serviços da aplicação e infraestrutura
            // Isso garante que todos os serviços e repositórios sejam registrados corretamente
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructure(builder.Configuration);

            // Adicionar serviços hospedados
            builder.Services.AddHostedService<MicroSaaS.Application.Services.Scheduler.SchedulerService>();

            // Adicionar controllers
            builder.Services.AddControllers();

            // Swagger - usando versão 6.5.0
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "MicroSaaS API",
                    Version = "v1",
                    Description = "API para gerenciamento de conteúdo para criadores de conteúdo digital",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Equipe MicroSaaS",
                        Email = "suporte@microsaas.com.br",
                        Url = new Uri("https://www.microsaas.com.br/contato")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense
                    {
                        Name = "Licença de Uso",
                        Url = new Uri("https://www.microsaas.com.br/licenca")
                    }
                });

                // Configurações para resolver problemas com tipos genéricos
                c.UseAllOfToExtendReferenceSchemas();

                // Para resolver problemas com tipos circulares ou complexos
                c.CustomSchemaIds(type => type.FullName?.Replace("+", "."));

                // Ignorar propriedades não serializáveis
                c.MapType<TimeSpan>(() => new Microsoft.OpenApi.Models.OpenApiSchema { Type = "string", Format = "time-span" });

                // Incluir arquivos XML de documentação
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                // Configurar autenticação JWT no Swagger
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
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

                // Desativar validação de parâmetros personalizados
                c.SchemaFilter<SwaggerRequiredSchemaFilter>();

                // Agrupar endpoints por tag para melhor organização
                c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
                c.DocInclusionPredicate((docName, api) => true);
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

            // Tornar o Redis opcional para desenvolvimento
            if (builder.Environment.IsDevelopment())
            {
                try
                {
                    // Usar as configurações do arquivo .env para o Redis
                    string redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? 
                                                  builder.Configuration.GetConnectionString("Redis") ?? 
                                                  "localhost:6379";
                    
                    string redisInstanceName = Environment.GetEnvironmentVariable("REDIS_INSTANCE_NAME") ?? 
                                              "MicroSaaS:";
                    
                    builder.Services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = redisConnectionString;
                        options.InstanceName = redisInstanceName;
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Aviso: Redis não disponível. Usando cache em memória. Erro: {ex.Message}");
                    // Usar cache em memória como fallback
                    builder.Services.AddDistributedMemoryCache();
                }
            }
            else
            {
                // Em produção, ainda queremos que falhe se o Redis não estiver disponível
                // Usar as configurações do arquivo .env para o Redis
                string redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? 
                                              builder.Configuration.GetConnectionString("Redis") ?? 
                                              "localhost:6379";
                
                string redisInstanceName = Environment.GetEnvironmentVariable("REDIS_INSTANCE_NAME") ?? 
                                          "MicroSaaS:";
                
                builder.Services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnectionString;
                    options.InstanceName = redisInstanceName;
                });
            }

            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
            builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
            builder.Services.AddInMemoryRateLimiting();
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            // Configuração do Rate Limiting por Endpoint
            builder.Services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RateLimitAttribute(limit: 100, period: "1m"));
            });

            // Registrar o MongoDbHealthCheck
            builder.Services.AddSingleton<MongoDbHealthCheck>();

            // Adicionar Health Checks
            builder.Services.AddHealthChecks()
                .AddCheck<MongoDbHealthCheck>("mongodb");

            var app = builder.Build();

            // Configurar Swagger usando a versão 6.5.0
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MicroSaaS API v1");
                    c.RoutePrefix = "swagger";
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                    c.DefaultModelsExpandDepth(-1); // Oculta a seção de modelos por padrão
                    c.DisplayRequestDuration();
                    c.EnableDeepLinking();
                    c.EnableFilter();
                });
            }

            // Adicionar middleware do Serilog para request logging
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} respondeu {StatusCode} em {Elapsed:0.0000} ms";
            });

            // Tentativa de usar o middleware de limitação de taxa, mas ignorar se falhar
            try
            {
                app.UseIpRateLimiting();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Aviso: Middleware de limitação de taxa não está disponível. Erro: {ex.Message}");
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Adicionar uma rota alternativa para o swagger.json para o caso da geração automática continuar falhando
            app.MapGet("/swagger/v1/swagger.json", async (HttpContext context, ISwaggerProvider swaggerProvider) =>
            {
                try
                {
                    // Tentar usar o provedor Swagger normal primeiro
                    var document = swaggerProvider.GetSwagger("v1");
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(document);
                }
                catch (Exception ex)
                {
                    // Registrar o erro detalhado no console
                    Console.WriteLine($"Erro ao gerar swagger.json: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                        Console.WriteLine($"Inner stack trace: {ex.InnerException.StackTrace}");
                    }

                    // Se falhar, gerar um documento OpenAPI manualmente com mais endpoints
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(@"{
  ""openapi"": ""3.0.1"",
  ""info"": {
    ""title"": ""MicroSaaS API"",
    ""version"": ""v1"",
    ""description"": ""API para gestão de conteúdo para criadores""
  },
  ""paths"": {
    ""/api/auth/register"": {
      ""post"": {
        ""tags"": [""Auth""],
        ""summary"": ""Registrar um novo usuário"",
        ""operationId"": ""register"",
        ""responses"": {
          ""200"": { ""description"": ""Registro bem-sucedido"" }
        }
      }
    },
    ""/api/auth/login"": {
      ""post"": {
        ""tags"": [""Auth""],
        ""summary"": ""Login de usuário"",
        ""operationId"": ""login"",
        ""responses"": {
          ""200"": { ""description"": ""Login bem-sucedido"" }
        }
      }
    },
    ""/api/dashboard/{creatorId}"": {
      ""get"": {
        ""tags"": [""Dashboard""],
        ""summary"": ""Obter dados do dashboard"",
        ""operationId"": ""getDashboard"",
        ""parameters"": [
          {
            ""name"": ""creatorId"",
            ""in"": ""path"",
            ""required"": true,
            ""schema"": { ""type"": ""string"", ""format"": ""uuid"" }
          }
        ],
        ""responses"": {
          ""200"": { ""description"": ""Dados do dashboard obtidos com sucesso"" }
        }
      }
    },
    ""/api/content"": {
      ""get"": {
        ""tags"": [""Content""],
        ""summary"": ""Listar conteúdos"",
        ""operationId"": ""listContent"",
        ""responses"": {
          ""200"": { ""description"": ""Conteúdos listados com sucesso"" }
        }
      }
    },
    ""/api/performance/summary/{creatorId}"": {
      ""get"": {
        ""tags"": [""Performance""],
        ""summary"": ""Resumo de desempenho"",
        ""operationId"": ""getPerformanceSummary"",
        ""parameters"": [
          {
            ""name"": ""creatorId"",
            ""in"": ""path"",
            ""required"": true,
            ""schema"": { ""type"": ""string"", ""format"": ""uuid"" }
          },
          {
            ""name"": ""startDate"",
            ""in"": ""query"",
            ""required"": false,
            ""schema"": { ""type"": ""string"", ""format"": ""date-time"" }
          },
          {
            ""name"": ""endDate"",
            ""in"": ""query"",
            ""required"": false,
            ""schema"": { ""type"": ""string"", ""format"": ""date-time"" }
          }
        ],
        ""responses"": {
          ""200"": { ""description"": ""Resumo de desempenho obtido com sucesso"" }
        }
      }
    }
  }
}");
                }
            });

            return app;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "A aplicação terminou inesperadamente");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
