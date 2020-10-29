using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Admin
{
    public class AdminPermissionTests : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
    {
        private TestingFixture _testingFixture;
        private readonly TestWebApplicationFactory _factory;
        private Func<Task> _tearDownTestingFixture;
        
        public AdminPermissionTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
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

        [Fact]
        public async Task AdminClientShouldBePermitted()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("AdminPortalClientId", TestAdminClientAuthHandler.AdminPortalClientId)
                }).Build();
            var client = _factory.CreateAdminClientAuthenticatedClient(services =>
                services.AddSingleton<IConfiguration>(config));
            var response = await client.SendAsync(
                new HttpRequestMessage(HttpMethod.Get,
                    $"/dms/api/admin/v1/projects?organisationId={_testingFixture.Organisation.Id}"));
            Equal(HttpStatusCode.OK, response.StatusCode);
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
