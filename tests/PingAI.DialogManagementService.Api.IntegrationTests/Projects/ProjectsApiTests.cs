using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models.Projects;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Projects
{
    public class ProjectsApiTests : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private readonly TestWebApplicationFactory _factory;
        private TestingFixture _testingFixture;
        private Func<Task> _tearDownTestingFixture;

        public ProjectsApiTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateAuthenticatedClient();
        }

        [Fact]
        public async Task GetProject()
        {
            var fixture = _testingFixture;
            var projectId = fixture.Project.Id;
            var response = await _client.GetFromJsonAsync<GetProjectResponse>(
                $"/dms/api/v1/projects/{projectId}");
            NotNull(response);
            Equal(projectId.ToString(), response.ProjectId);

        }

        [Fact]
        public async Task UpdateProject()
        {
            var fixture = _testingFixture;
            var httpResponse = await _client.PutAsJsonAsync(
                $"/dms/api/v1/projects/{fixture.Project.Id}",
                new UpdateProjectRequest
                {
                    WidgetTitle = "title",
                    WidgetColor = "#ffffff",
                    WidgetDescription = "description",
                    GreetingMessage = "greeting",
                    FallbackMessage = "fallback",
                    Enquiries = new[] {"email"}
                });
            await httpResponse.IsOk();
            var response = await httpResponse.Content.ReadFromJsonAsync<UpdateProjectResponse>();
            NotNull(response);
            Equal(fixture.Project.Id.ToString(), response.ProjectId);
        }

        public async Task InitializeAsync()
        {
            (_testingFixture, _tearDownTestingFixture) = await _factory.SetupTestingFixture();
        }

        public async Task DisposeAsync()
        {
            await _tearDownTestingFixture.Invoke();
        }
    }
}