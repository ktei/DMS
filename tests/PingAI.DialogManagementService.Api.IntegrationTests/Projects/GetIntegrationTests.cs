using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models.Projects;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Projects
{
    public class GetIntegrationTests : ApiTestBase
    {
        public GetIntegrationTests(TestWebApplicationFactory factory, SharedDatabaseFixture fixture) : base(factory,
            fixture)
        {
        }

        [Fact]
        public async Task GetIntegration()
        {
            var context = Fixture.CreateContext();
            var project = await context.Projects.FirstAsync();
            await context.AddAsync(new SlackWorkspace(project.Id,
                "token", "webhookUrl", "teamId"));
            await context.SaveChangesAsync();
            var client = Factory.CreateUserAuthenticatedClient();

            var actual = await client.GetFromJsonAsync<IntegrationDto>(
                $"/dms/api/v1/projects/{project.Id}/integration"
            );

            actual.Should().NotBeNull();
            actual!.Connections.Should().HaveCountGreaterOrEqualTo(1);
        }
    }
}