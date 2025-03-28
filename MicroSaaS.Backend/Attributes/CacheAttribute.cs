using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MicroSaaS.Application.Interfaces.Services;

namespace MicroSaaS.Backend.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class CacheAttribute : ActionFilterAttribute
{
    private readonly string _key;
    private readonly TimeSpan _expiration;
    private ICacheService _cacheService;

    public CacheAttribute(string key, int minutes = 30)
    {
        _key = key;
        _expiration = TimeSpan.FromMinutes(minutes);
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        _cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

        // Não usar cache para métodos POST, PUT, DELETE
        if (context.HttpContext.Request.Method != "GET")
        {
            await next();
            return;
        }

        var cachedResult = await _cacheService.GetAsync<object>(_key);
        if (cachedResult != null)
        {
            context.Result = new OkObjectResult(cachedResult);
            return;
        }

        var executedContext = await next();
        if (executedContext.Result is OkObjectResult okResult)
        {
            await _cacheService.SetAsync(_key, okResult.Value, _expiration);
        }
    }
} 