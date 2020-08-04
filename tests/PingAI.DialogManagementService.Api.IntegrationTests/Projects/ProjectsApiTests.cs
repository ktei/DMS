using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models.Projects;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Projects
{
    public class ProjectsApiTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly TestWebApplicationFactory _factory;

        public ProjectsApiTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateAuthenticatedClient();
        }

        [Fact]
        public async Task UpdateProject()
        {
            var httpResponse = await _client.PutAsJsonAsync(
                "/dms/api/v1/projects/3932f12d-ed9e-441a-8a13-8c4ca88b2e4c",
                new UpdateProjectRequest
                {
                    WidgetTitle = "title",
                    WidgetColor = "#ffffff",
                    WidgetDescription = "description",
                    GreetingMessage = "greeting",
                    FallbackMessage = "fallback",
                    Enquiries = new[] {"email"}
                });
            var response = await httpResponse.Content.ReadFromJsonAsync<UpdateProjectResponse>();
            NotNull(response);
        }
    }
}