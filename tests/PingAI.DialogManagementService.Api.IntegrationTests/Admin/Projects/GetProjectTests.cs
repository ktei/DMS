using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models.Projects;
using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Admin.Projects
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
            var client = Factory.CreateAdminUserAuthenticatedClient();

            var response = await client.GetFromJsonAsync<ProjectDto>(
                $"/dms/api/admin/v1/projects/{project.Id}");

            response.Should().NotBeNull();
        }

        [Fact]
        public async Task GetRuntimeByDesignTimeProjectId()
        {
            var context = Fixture.CreateContext();
            var project = await context.Projects.FirstAsync();
            var client = Factory.CreateAdminUserAuthenticatedClient();
            
            var response = await client.GetFromJsonAsync<ProjectDto>(
                $"/dms/api/admin/v1/projects/{project.Id}/runtime");

            response.Should().NotBeNull();
        }
    }
}