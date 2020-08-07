using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
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

        private static readonly Guid EntityNameId = Guid.NewGuid();
        private static readonly Guid EntityTypeId = Guid.NewGuid();
        
        public static async Task<(TestingFixture TestingFixture, Func<Task> TearDownTestingFixture)>
            SetupTestingFixture(this TestWebApplicationFactory factory)
        {
            var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DialogManagementContext>();
            var organisation =
                await context.Organisations.FindAsync(Guid.Parse("6b95c285-363c-4b2a-a322-54fe3cad9698"));
            var project = await context.Projects.FindAsync(Guid.Parse("3932f12d-ed9e-441a-8a13-8c4ca88b2e4c"));
            var organisationUser = new OrganisationUser(Guid.NewGuid(), organisation.Id,
                Guid.Parse("3ec1b42a-aada-4487-8ac1-ee2c5ef4cc7f"));
            var entityType = await context.EntityTypes.FirstOrDefaultAsync(e => e.Name == "TEST_city");
            var entityName = await context.EntityNames.FirstOrDefaultAsync(e => e.Name == "TEST_favouriteCity");
            entityType ??= (await context.AddAsync(new EntityType(EntityTypeId, 
                    "TEST_city", project.Id, "city name", null))).Entity;
            entityName ??= (await context.AddAsync(new EntityName(EntityNameId,
                    "TEST_favouriteCity", project.Id, true))).Entity;
            try
            {
                await context.SaveChangesAsync();
            }
            catch
            {
                // TODO: this is a bad way to fix concurrency issue
                // We only want to add these entityName, entityType
                // once, but tests run concurrently so we may encounter
                // issues such as entityName with same name already exists
            }

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
                // we don't h ave anything to dispose of yet
                scope.Dispose();
            });
        }

        public static Task WithDbContext(this TestWebApplicationFactory factory,
            Func<DialogManagementContext, Task> useDbContext)
            => factory.WithServiceProvider(sp => 
                useDbContext(sp.GetRequiredService<DialogManagementContext>()));

        public static async Task WithServiceProvider(this TestWebApplicationFactory factory,
            Func<IServiceProvider, Task> useServiceProvider)
        {
            using var scope = factory.Services.CreateScope();
            var sp = scope.ServiceProvider;
            await useServiceProvider(sp);
        }
    }
}
