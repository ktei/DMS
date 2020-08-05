using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence;

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

        public static async Task<(TestingFixture TestingFixture, Func<Task> TearDownTestingFixture)>
            SetupTestingFixture(this TestWebApplicationFactory factory)
        {
            var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DialogManagementContext>();
            var organisation =
                new Organisation(Guid.NewGuid(), "test organisation", "test description", null);
            var project = new Project(Guid.NewGuid(), "test project", organisation.Id,
                "test widget title", "#ffffff",
                "test widget description", "test fallback message",
                "test greeting message", new string[] { });
            var organisationUser = new OrganisationUser(Guid.NewGuid(), organisation.Id,
                Guid.Parse("3ec1b42a-aada-4487-8ac1-ee2c5ef4cc7f"));
            var entityType = new EntityType(Guid.NewGuid(), "city", project.Id, "city name", null);
            var entityName = new EntityName(Guid.NewGuid(), "favouriteCity", project.Id, true);
            await context.AddRangeAsync(organisation, organisationUser,
                project, entityType, entityName);
            await context.SaveChangesAsync();
            var testingFixture = new TestingFixture
            {
                Organisation = organisation,
                Project = project,
                UserId = organisationUser.UserId,
                EntityType = entityType, 
                EntityName = entityName
            };

            return (testingFixture, async () =>
            {
                context.RemoveRange(project, organisationUser,
                    organisation, entityType, entityName);
                await context.SaveChangesAsync();
                scope.Dispose();
            });
        }

        public static Task WithDbContext(this TestWebApplicationFactory factory,
            Func<DialogManagementContext, Task> useDbContext)
            => factory.WithServiceProvider(sp => useDbContext(sp.GetRequiredService<DialogManagementContext>()));

        public static async Task WithServiceProvider(this TestWebApplicationFactory factory,
            Func<IServiceProvider, Task> useServiceProvider)
        {
            using var scope = factory.Services.CreateScope();
            var sp = scope.ServiceProvider;
            await useServiceProvider(sp);
        }
    }
}
