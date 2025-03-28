using System;

namespace MicroSaaS.Application.Interfaces.Services
{
    public interface ILoggingService
    {
        void LogInformation(string message, params object[] propertyValues);
        void LogWarning(string message, params object[] propertyValues);
        void LogError(Exception ex, string message, params object[] propertyValues);
        void LogDebug(string message, params object[] propertyValues);
        void LogCritical(Exception ex, string message, params object[] propertyValues);
    }
} 