using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AspNetCoreRateLimit;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace MicroSaaS.Backend.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RateLimitAttribute : ActionFilterAttribute
{
    private readonly int _limit;
    private readonly string _period;
    private IDistributedCache _cache;

    public RateLimitAttribute(int limit = 100, string period = "1m")
    {
        _limit = limit;
        _period = period;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            _cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();
            
            var clientId = context.HttpContext.Request.Headers["X-ClientId"].ToString();
            var endpoint = $"{context.HttpContext.Request.Method}:{context.HttpContext.Request.Path}";
            var key = $"rate_limit:{clientId}:{endpoint}";

            try
            {
                var currentCount = await _cache.GetStringAsync(key);
                var count = string.IsNullOrEmpty(currentCount) ? 0 : int.Parse(currentCount);

                if (count >= _limit)
                {
                    context.Result = new StatusCodeResult(429);
                    return;
                }

                await _cache.SetStringAsync(key, (count + 1).ToString(), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.Parse(_period)
                });
            }
            catch (Exception)
            {
                // Se ocorrer um erro no cache (Redis indisponível, etc.), 
                // permitiremos a solicitação mesmo assim para não bloquear o uso
                Console.WriteLine("Aviso: Não foi possível verificar a limitação de taxa. Redis/Cache indisponível.");
            }
        }
        catch (Exception ex)
        {
            // Se ocorrer qualquer erro, permitir a solicitação e registrar o problema
            Console.WriteLine($"Erro no RateLimitAttribute: {ex.Message}");
        }

        await next();
    }
} 