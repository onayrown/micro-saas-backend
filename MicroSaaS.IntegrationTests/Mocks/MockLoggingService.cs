using MicroSaaS.Application.Interfaces.Services;
using System;

namespace MicroSaaS.IntegrationTests.Mocks
{
    public class MockLoggingService : ILoggingService
    {
        // Simula log de informação (imprime no console para visibilidade no teste)
        public void LogInformation(string message, params object[] args)
        {
            Console.WriteLine($"[INFO] {string.Format(message, args)}");
        }

        // Simula log de aviso
        public void LogWarning(string message, params object[] args)
        {
            Console.WriteLine($"[WARN] {string.Format(message, args)}");
        }

        // Simula log de erro
        public void LogError(Exception ex, string message, params object[] args)
        {
            Console.WriteLine($"[ERROR] {string.Format(message, args)} Exception: {ex.Message}");
            // Poderia logar o stack trace também, se necessário para depuração
            // Console.WriteLine(ex.StackTrace);
        }

        // Simula log de depuração (pode ser condicional)
        public void LogDebug(string message, params object[] args)
        {
#if DEBUG // Exibe logs de debug apenas em modo Debug
            Console.WriteLine($"[DEBUG] {string.Format(message, args)}");
#endif
        }

        // Simula log crítico
        public void LogCritical(Exception ex, string message, params object[] args)
        {
            Console.WriteLine($"[CRITICAL] {string.Format(message, args)} Exception: {ex.Message}");
             // Console.WriteLine(ex.StackTrace);
        }
    }
} 