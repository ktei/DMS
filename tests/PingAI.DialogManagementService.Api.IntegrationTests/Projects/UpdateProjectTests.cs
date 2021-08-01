using System.Net;
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
    public class UpdateProjectTests : ApiTestBase
    {
        public UpdateProjectTests(TestWebApplicationFactory factory, SharedDatabaseFixture fixture) : base(factory,
            fixture)
        {
        }

        [Fact]
        public async Task UpdateProject()
        {
            var context = Fixture.CreateContext();
            var project = await context.Projects.FirstAsync();
            var client = Factory.CreateUserAuthenticatedClient();
            var updateRequest = new UpdateProjectRequest
            {
                WidgetTitle = "title",
                WidgetColor = "#2196f3",
                WidgetDescription = "description",
                GreetingMessage = "greeting",
                QuickReplies = new[] {"hello", "world"},
                FallbackMessage = "fallback",
                BusinessEmail = "support@iiiknow.com",
                BusinessTimeStart = "2020-09-19T09:30:00",
                BusinessTimeEnd = "2020-09-19T17:00:00",
                Domains = new[] {"https://test.com"}
            };
            
            var httpResponse = await client.PutAsJsonAsync(
                $"/dms/api/v1/projects/{project.Id}",
                updateRequest);

            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            context.ChangeTracker.Clear();
            var actual = await context.Projects
                .Include(x => x.GreetingResponses)
                .ThenInclude(gr => gr.Response)
                .SingleAsync(x => x.Id == project.Id);
            actual.WidgetTitle.Should().Be(updateRequest.WidgetTitle);
            actual.WidgetColor.Should().Be(updateRequest.WidgetColor);
            actual.WidgetDescription.Should().Be(updateRequest.WidgetDescription);
            actual.GreetingResponses.Should().Contain(gr => gr.Response!.Type == ResponseType.RTE &&
                                                            gr.Response!.Resolution.Parts![0].Text == "greeting");
            actual.GreetingResponses.Should().Contain(gr => gr.Response!.Type == ResponseType.QUICK_REPLY &&
                                                            gr.Response!.Resolution.Parts![0].Text == "hello");
            actual.GreetingResponses.Should().Contain(gr => gr.Response!.Type == ResponseType.QUICK_REPLY &&
                                                            gr.Response!.Resolution.Parts![0].Text == "world");
            actual.FallbackMessage.Should().Be(updateRequest.FallbackMessage);
            actual.BusinessEmail.Should().Be(updateRequest.BusinessEmail);
            actual.Domains.Should().BeEquivalentTo(updateRequest.Domains);
        }
    }
}