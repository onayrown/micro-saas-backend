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

namespace MicroSaaS.Backend;

// Tornando a classe Program parcial e pública para permitir que o WebApplicationFactory a acesse
public partial class Program
{
    // Este método parcial é necessário para o ASP.NET Core reconhecer a classe corretamente nos testes
    // Não precisa de implementação
    partial void InitializeForTesting();

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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

        // Registrar dependências do repositório
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IContentCreatorRepository, ContentCreatorRepository>();
        builder.Services.AddScoped<IContentPostRepository, ContentPostRepository>();
        builder.Services.AddScoped<IContentChecklistRepository, ContentChecklistRepository>();
        builder.Services.AddScoped<ISocialMediaAccountRepository, SocialMediaAccountRepository>();

        // Registrar serviços
        builder.Services.AddScoped<IAuthService, MicroSaaS.Infrastructure.Services.AuthService>();
        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<ISocialMediaIntegrationService, SocialMediaIntegrationService>();
        builder.Services.AddScoped<IRevenueService, RevenueService>();
        builder.Services.AddScoped<IContentPlanningService, ContentPlanningService>();

        // Adicionar controllers
        builder.Services.AddControllers();

        // Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

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

        var app = builder.Build();

        // Configurar Swagger
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
