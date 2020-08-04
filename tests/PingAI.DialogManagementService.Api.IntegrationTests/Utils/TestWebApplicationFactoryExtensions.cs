using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Utils
{
    public static class TestWebApplicationFactoryExtensions
    {
        public static HttpClient CreateAuthenticatedClient(this TestWebApplicationFactory factory) =>
            factory.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddAuthentication(options =>
                            {
                                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                options.DefaultAuthenticateScheme = "Test";
                            })
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                                "Test", options => { });
                    });
                })
                .CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });
    }
}
