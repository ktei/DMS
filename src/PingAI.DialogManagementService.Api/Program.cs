using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using PingAI.DialogManagementService.Infrastructure.Persistence;

namespace PingAI.DialogManagementService.Api
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            await using var conn = (NpgsqlConnection) scope.ServiceProvider.GetService<DialogManagementContext>()
                .Database.GetDbConnection();
            await conn.OpenAsync();
            conn.ReloadTypes();
            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var env = context.HostingEnvironment;
                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: !env.IsProduction())
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true,
                            reloadOnChange: !env.IsProduction());
                    if (!env.IsProduction())
                    {
                        builder.AddJsonFile("appsettings.Local.json", true, true);
                    }
                    
                    builder.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    // TODO: use Serilog
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel((context, options) =>
                    {
                        if (!int.TryParse(context.Configuration["PORT"], out var portNumber))
                        {
                            portNumber = 5000;
                        }

                        // TODO: in future we may want to e2e encryption
                        // so we need to listen on https in production
                        options.Listen(IPAddress.Any, portNumber);
                        Console.WriteLine($"Listening on http://*:{portNumber}");
                        Console.Title = $"DialogManagementService: {portNumber}";
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
