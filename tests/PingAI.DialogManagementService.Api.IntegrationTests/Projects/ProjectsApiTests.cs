using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
            _client = _factory.CreateUserAuthenticatedClient();
        }

        [Fact]
        public async Task ListProjects()
        {
            var response = await _client.GetFromJsonAsync<List<ProjectListItemDto>>(
                $"/dms/api/v1/projects?organisationId={_testingFixture.Organisation.Id}");
            NotNull(response);
            True(response.Count > 0);
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
                    WidgetColor = "#2196f3",
                    WidgetDescription = "description",
                    GreetingMessage = "greeting",
                    FallbackMessage = "fallback",
                });
            await httpResponse.IsOk();
            var response = await httpResponse.Content.ReadFromJsonAsync<UpdateProjectResponse>();
            NotNull(response);
            Equal(fixture.Project.Id.ToString(), response.ProjectId);
        }

        [Fact]
        public async Task UpdateEnquiries()
        {
            var fixture = _testingFixture;
            var httpResponse = await _client.PutAsJsonAsync(
                $"/dms/api/v1/projects/{fixture.Project.Id}/enquiries",
                new UpdateEnquiriesRequest
                {
                    Enquiries = new[]{"e1", "e2", "e3"}
                }); 
            await httpResponse.IsOk();
            var response = await httpResponse.Content.ReadFromJsonAsync<UpdateEnquiriesResponse>();
            NotNull(response);
            Equal(3, response.Enquiries.Length);
        }

        [Fact]
        public async Task PublishProject()
        {
            var fixture = _testingFixture;
            var httpResponse = await _client.SendAsync(
                new HttpRequestMessage(HttpMethod.Post,
                    $"/dms/api/v1/projects/{fixture.Project.Id}/publish"));
            await httpResponse.IsOk();
            var response = await httpResponse.Content.ReadFromJsonAsync<Guid>();
            
            // cleanup
            await _factory.WithDbContext(async context =>
            {
                var project = await context.Projects
                    .Include(p => p.EntityNames)
                    .Include(p => p.EntityTypes)
                    .Include(p => p.Intents)
                    .Include(p => p.Responses)
                    .Include(p => p.Queries)
                    .SingleAsync(p => p.Id == response);
                var projectVersion = await context.ProjectVersions.FirstAsync(x => x.ProjectId == project.Id);
                context.RemoveRange(projectVersion);
                context.RemoveRange(project.EntityNames);
                context.RemoveRange(project.EntityTypes);
                context.RemoveRange(project.Responses);
                context.RemoveRange(project.Intents);
                context.RemoveRange(project.Queries);
                context.Remove(project);
                await context.SaveChangesAsync();
            });
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