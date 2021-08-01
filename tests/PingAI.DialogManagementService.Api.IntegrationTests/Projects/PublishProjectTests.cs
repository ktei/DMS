using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Projects
{
    public class PublishProjectTests : ApiTestBase
    {
        public PublishProjectTests(TestWebApplicationFactory factory, SharedDatabaseFixture fixture) : base(factory,
            fixture)
        {
        }

        [Fact]
        public async Task PublishProject()
        {
            var context = Fixture.CreateContext();
            var project = await context.Projects.FirstAsync();
            var client = Factory.CreateUserAuthenticatedClient();

            var httpResponse = await client.SendAsync(
                new HttpRequestMessage(HttpMethod.Post,
                    $"/dms/api/v1/projects/{project.Id}/publish"));
            var responseContent = httpResponse.Content.ReadAsStringAsync();

            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var publishedProjectId = await httpResponse.Content.ReadFromJsonAsync<Guid>();
            context.ChangeTracker.Clear();
            var actual = await context.Projects.FirstOrDefaultAsync(x => x.Id == publishedProjectId);
            actual.Should().NotBeNull();
        }
    }
}