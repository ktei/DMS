using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models.Projects;
using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Projects
{
    public class GetProjectTests : ApiTestBase
    {
        public GetProjectTests(TestWebApplicationFactory factory, SharedDatabaseFixture fixture) : base(factory,
            fixture)
        {
        }

        [Fact]
        public async Task GetProject()
        {
            var context = Fixture.CreateContext();
            var project = await context.Projects.FirstAsync();
            var client = Factory.CreateUserAuthenticatedClient();

            var actual = await client.GetFromJsonAsync<ProjectDto>(
                $"/dms/api/v1/projects/{project.Id}"
            );

            actual.Should().NotBeNull();
            actual!.ProjectId.Should().Be(project.Id.ToString());
        }
    }
}