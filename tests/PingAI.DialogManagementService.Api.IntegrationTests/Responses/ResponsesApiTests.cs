using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models.Responses;
using PingAI.DialogManagementService.Domain.Model;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Responses
{
    public class ResponsesApiTests: IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private readonly TestWebApplicationFactory _factory;
        private TestingFixture _testingFixture;
        private Func<Task> _tearDownTestingFixture;
        
        public ResponsesApiTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateAuthenticatedClient();
        }

        [Fact]
        public async Task CreateResponse()
        {
            var fixture = _testingFixture;
            var httpResponse = await _client.PostAsJsonAsync(
                "/dms/api/v1/responses", new CreateResponseRequest
                {
                    ProjectId = fixture.Project.Id.ToString(),
                    RteText = $"Test, ${{{fixture.EntityName.Name}}}!",
                    Type = ResponseType.RTE.ToString()
                });
            await httpResponse.IsOk();
            var response = await httpResponse.Content.ReadFromJsonAsync<CreateResponseResponse>();
            
            // clean up
            await _factory.WithDbContext(async context =>
            {
                var r = await context.Responses.FirstOrDefaultAsync(
                    resp => resp.Id == Guid.Parse(response.ResponseId));
                context.Responses.Remove(r);
                await context.SaveChangesAsync();
            });
            
            NotNull(response);
            Contains(response.Resolution, p => p.EntityNameId == fixture.EntityName.Id.ToString());
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