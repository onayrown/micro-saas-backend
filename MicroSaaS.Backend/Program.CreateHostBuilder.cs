using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MicroSaaS.Backend;

public partial class Program
{
    // Este método é chamado pelo WebApplicationFactory nos testes
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<TestWebAppStartup>();
            });
} 