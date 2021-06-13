using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Queries
{
    public class DeleteQueryTests : ApiTestBase
    {
        public DeleteQueryTests(TestWebApplicationFactory factory, SharedDatabaseFixture fixture) : base(factory,
            fixture)
        {
        }

        [Fact]
        public async Task DeleteQuery()
        {
            var context = Fixture.CreateContext();
            var project = await context.Projects.FirstAsync();
            var query = await context.Queries.FirstAsync(x => x.ProjectId == project.Id);
            var client = Factory.CreateUserAuthenticatedClient();

            var httpResponse = await client.DeleteAsync(
                $"/dms/api/v1/queries/{query.Id}"
            );

            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
