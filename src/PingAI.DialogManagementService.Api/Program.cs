using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PingAI.DialogManagementService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
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
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
