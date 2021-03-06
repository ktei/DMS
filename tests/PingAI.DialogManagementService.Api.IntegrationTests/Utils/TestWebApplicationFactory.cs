using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PingAI.DialogManagementService.Application.Interfaces.Services.Nlu;
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
                         typeof(DialogManagementContext));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // change connection string to your local DB's connection string
                services.AddDbContextPool<DialogManagementContext>(optionsBuilder =>
                    optionsBuilder.UseNpgsql(
                        "Host=localhost;Database=postgres;Username=postgres;Password=admin"));

                // uncomment this if you want to test the integration with NLU service
                // but you need to run local NLU service at port 5678
                // services.AddHttpClient<INluService, NluService>(client =>
                // {
                //     client.BaseAddress = new Uri("http://localhost:5678");
                // });

                // uncomment this if you want to mock NluService
                services.AddTransient(_ => MockNluService());

                services.AddStackExchangeRedisCache(options => { options.Configuration = "localhost:6379"; });
            });
        }

        private static INluService MockNluService()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});
            return fixture.Create<INluService>();
        }
    }
}

