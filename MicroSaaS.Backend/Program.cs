using FluentValidation;
using FluentValidation.AspNetCore;
using MicroSaaS.Application.DTOs.Validators;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Application.Services;
using MicroSaaS.Domain.Interfaces;
using MicroSaaS.Infrastructure.Configuration;
using MicroSaaS.Infrastructure.Data;
using MicroSaaS.Infrastructure.Repositories;
using MicroSaaS.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MicroSaaS.Backend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configuração FluentValidation
        builder.Services.AddValidatorsFromAssemblyContaining<ContentCreatorValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<ContentPostValidator>();

        // Adiciona FluentValidation manualmente ao MVC (versão 11.3.0)
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddMvc();

        // Configuração do MongoDB
        builder.Services.Configure<MongoDbSettings>(
            builder.Configuration.GetSection("MongoDbSettings"));

        // Obter a chave JWT das configurações
        var jwtKey = builder.Configuration["JwtSettings:SecretKey"] ?? 
            throw new InvalidOperationException("JWT Secret Key não está configurada no appsettings.json");

        // Configuração do JWT
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        // Registrar dependências do repositório
        builder.Services.AddSingleton<MongoDbContext>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IContentCreatorRepository, ContentCreatorRepository>();
        builder.Services.AddScoped<IContentPostRepository, ContentPostRepository>();

        // Registrar serviços
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

        // Adicionar controllers
        builder.Services.AddControllers();

        // Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configurar Swagger
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
