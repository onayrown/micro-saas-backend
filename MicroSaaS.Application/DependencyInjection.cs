using Microsoft.Extensions.DependencyInjection;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Application.Services;
using MicroSaaS.Application.Services.Recommendation;
using MicroSaaS.Application.Services.Scheduler;
using MicroSaaS.Application.Services.Dashboard;
using MicroSaaS.Shared.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Adicionar ContentAnalysisService que agora está na camada correta (Application)
        services.AddScoped<IContentAnalysisService, ContentAnalysisService>();

        // Adicionar ContentPlanningService que agora está na camada correta (Application)
        services.AddScoped<IContentPlanningService, ContentPlanningService>();

        // Adicionar RecommendationService que agora está na camada correta (Application)
        services.AddScoped<IRecommendationService, RecommendationService>();

        // Adicionar SchedulerService que agora está na camada correta (Application)
        services.AddSingleton<ISchedulerService, SchedulerService>();

        // Adicionar DashboardService que agora está na camada correta (Application)
        services.AddScoped<IDashboardService, DashboardService>();

        // Adicionar registros para serviços cujas implementações estão na Application
        services.AddScoped<IDashboardInsightsService, DashboardInsightsService>();
        services.AddScoped<IPerformanceAnalysisService, PerformanceAnalysisService>();
        services.AddScoped<IAuthService, AuthService>();

        // Adicionar outros serviços específicos da Application aqui, se houver.

        return services;
    }
} 