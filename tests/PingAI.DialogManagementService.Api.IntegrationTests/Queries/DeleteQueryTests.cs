using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Domain.Model;
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
            var query = new Query(project.Id, Guid.NewGuid().ToString(), new Expression[0],
                "desc", null, 1);
            query.AddIntent(new Intent(query.ProjectId, DateTime.UtcNow.Ticks.ToString(), IntentType.STANDARD));
            await context.AddAsync(query);
            await context.SaveChangesAsync();
            var client = Factory.CreateUserAuthenticatedClient();

            var httpResponse = await client.DeleteAsync(
                $"/dms/api/v1/queries/{query.Id}"
            );

            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
