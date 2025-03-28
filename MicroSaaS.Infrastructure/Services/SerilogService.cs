using System;
using Microsoft.Extensions.Logging;
using MicroSaaS.Application.Interfaces.Services;
using Serilog;

namespace MicroSaaS.Infrastructure.Services
{
    public class SerilogService : ILoggingService
    {
        private readonly ILogger<SerilogService> _logger;

        public SerilogService(ILogger<SerilogService> logger)
        {
            _logger = logger;
        }

        public void LogInformation(string message, params object[] propertyValues)
        {
            _logger.LogInformation(message, propertyValues);
        }

        public void LogWarning(string message, params object[] propertyValues)
        {
            _logger.LogWarning(message, propertyValues);
        }

        public void LogError(Exception ex, string message, params object[] propertyValues)
        {
            _logger.LogError(ex, message, propertyValues);
        }

        public void LogDebug(string message, params object[] propertyValues)
        {
            _logger.LogDebug(message, propertyValues);
        }

        public void LogCritical(Exception ex, string message, params object[] propertyValues)
        {
            _logger.LogCritical(ex, message, propertyValues);
        }
    }
} 