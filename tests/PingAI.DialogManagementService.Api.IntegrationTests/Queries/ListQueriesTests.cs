using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models.Queries;
using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Queries
{
    public class ListQueriesTests : ApiTestBase
    {
        public ListQueriesTests(TestWebApplicationFactory factory, SharedDatabaseFixture fixture) : base(factory,
            fixture)
        {
        }

        [Fact]
        public async Task ListQueries()
        {
            var context = Fixture.CreateContext();
            var project = await context.Projects.FirstAsync();
            var client = Factory.CreateUserAuthenticatedClient();

            var actual = await client.GetFromJsonAsync<QueryListItemDto[]>(
                $"/dms/api/v1.1/queries?projectId={project.Id}"
            );

            actual.Should().NotBeNull();
            actual!.Should().HaveCountGreaterOrEqualTo(1);
        }
    }
}