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
    public class ListProjectsTests : ApiTestBase
    {
        public ListProjectsTests(TestWebApplicationFactory factory, SharedDatabaseFixture fixture) : base(factory,
            fixture)
        {
        }

        [Fact]
        public async Task ListProjects()
        {
            var context = Fixture.CreateContext();
            var organisation = await context.Organisations.FirstAsync();
            var client = Factory.CreateUserAuthenticatedClient();

            var actual = await client.GetFromJsonAsync<ProjectListItemDto[]>(
                $"/dms/api/v1/projects?organisationId={organisation.Id}"
            );
            
            actual.Should().NotBeNull();
            actual!.Should().HaveCountGreaterOrEqualTo(1);
        }
    }
}