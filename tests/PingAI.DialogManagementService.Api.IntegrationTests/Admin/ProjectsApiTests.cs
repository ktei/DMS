using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models.Projects;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Admin
{
    public class ProjectsApiTests : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private TestingFixture _testingFixture;
        private readonly TestWebApplicationFactory _factory;
        private Func<Task> _tearDownTestingFixture;

        public ProjectsApiTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateAdminUserAuthenticatedClient();
        }
        
        [Fact]
        public async Task ListProjects()
        {
            var response = await _client.GetFromJsonAsync<List<ProjectListItemDto>>(
                $"/dms/api/admin/v1/projects?organisationId={_testingFixture.Organisation.Id}");
            NotNull(response);
            True(response.Count > 0);
        }

        [Fact]
        public async Task GetRuntimeProjectByDesignTimeProjectId()
        {
            var response = await _client.GetFromJsonAsync<ProjectDto>(
                $"/dms/api/admin/v1/projects/{_testingFixture.Project.Id}/runtime");
            NotNull(response);
            Equal(_testingFixture.Project.Id, Guid.Parse(response.ProjectId));
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