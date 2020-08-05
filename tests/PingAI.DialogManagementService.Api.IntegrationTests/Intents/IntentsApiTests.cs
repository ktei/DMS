using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models.Intents;
using PingAI.DialogManagementService.Domain.Model;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Intents
{
    public class IntentsApiTests : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private readonly TestWebApplicationFactory _factory;
        private TestingFixture _testingFixture;
        private Func<Task> _tearDownTestingFixture;
        
        public IntentsApiTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateAuthenticatedClient();
        }

        [Fact]
        public async Task CreateIntent()
        {
            var httpResponse = await _client.PostAsJsonAsync(
                "/dms/api/v1/intents", new CreateIntentRequest
                {
                    Name = "test intent",
                    ProjectId = _testingFixture.Project.Id.ToString(),
                    Type = IntentType.STANDARD.ToString(),
                    // PhraseParts = new[]
                    // {
                    //     new CreatePhrasePartDto
                    //     {
                    //         Text = "Hello, World!",
                    //         PhraseId = Guid.NewGuid().ToString(),
                    //         
                    //     }
                    // }
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
            Equal(_testingFixture.Project.Id.ToString(), response.ProjectId);
            Equal("test intent", response.Name);
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