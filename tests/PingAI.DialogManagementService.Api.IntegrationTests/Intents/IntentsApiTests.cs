using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models;
using PingAI.DialogManagementService.Api.Models.Intents;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Intents
{
    public class IntentsApiTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly TestWebApplicationFactory _factory;
        
        public IntentsApiTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateAuthenticatedClient();
        }

        [Fact]
        public async Task CreateIntent()
        {
            await _factory.WithTestingFixture(async fixture =>
            {
                var httpResponse = await _client.PostAsJsonAsync(
                    "/dms/api/v1/intents", new CreateIntentRequest
                    {
                        Name = "test intent",
                        ProjectId = fixture.Project.Id.ToString(),
                        Type = IntentType.STANDARD.ToString()
                    });
                await httpResponse.IsOk();
                var response = await httpResponse.Content.ReadFromJsonAsync<CreateIntentResponse>();
                
                // clean up
                await _factory.WithDbContext(async context =>
                {
                    context.Intents.Remove(context.Intents.Single(i => i.Id == Guid.Parse(response.IntentId)));
                    await context.SaveChangesAsync();
                });

                NotNull(response);
                Equal(fixture.Project.Id.ToString(), response.ProjectId);
                Equal("test intent", response.Name);
            });
        }
    }
}