using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PingAI.DialogManagementService.Infrastructure.Persistence;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Utils
{
    public class TestWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // get rid of the application db context
                // we want to use the db for testing
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<DialogManagementContext>));
                services.Remove(descriptor);

                // change connection string to your local DB's connection string
                services.AddDbContext<DialogManagementContext>(optionsBuilder =>
                    optionsBuilder.UseNpgsql(
                        "Host=localhost;Database=postgres;Username=postgres;Password=admin"));
            });
        }
    }
}