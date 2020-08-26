using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models.Projects;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Projects
{
    public class AdminProjectsApiTests : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private TestingFixture _testingFixture;
        private readonly TestWebApplicationFactory _factory;
        private Func<Task> _tearDownTestingFixture;

        public AdminProjectsApiTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateAdminAuthenticatedClient();
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
        public async Task NonAdminShouldBeRejected()
        {
            var client = _factory.CreateUserAuthenticatedClient();
            var response = await client.SendAsync(
                new HttpRequestMessage(HttpMethod.Get,
                    $"/dms/api/admin/v1/projects?organisationId={_testingFixture.Organisation.Id}"));
            Equal(HttpStatusCode.Forbidden, response.StatusCode);
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