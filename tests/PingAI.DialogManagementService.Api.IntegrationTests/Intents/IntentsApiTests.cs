using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        public async Task ListIntents()
        {
            // Arrange
            await _factory.WithDbContext(async context =>
            {
                var intent1 = new Intent( "i1", _testingFixture.Project.Id, IntentType.STANDARD);
                var intent2 = new Intent( "t2", _testingFixture.Project.Id, IntentType.STANDARD);
                await context.AddRangeAsync(intent1, intent2);
                await context.SaveChangesAsync();
                var actual = await _client.GetFromJsonAsync<List<IntentListItemDto>>(
                    $"/dms/api/v1/intents?projectId={_testingFixture.Project.Id}");
                
                // clean up
                context.RemoveRange(intent1, intent2);
                await context.SaveChangesAsync();
                
                Equal(2, actual.Count);
            });
        }

        [Fact]
        public async Task GetIntent()
        {
            // Arrange
            await _factory.WithDbContext(async context =>
            {
                var intent = new Intent("i1", _testingFixture.Project.Id, IntentType.STANDARD);
                var phrasePart = new PhrasePart(intent.Id,
                    Guid.NewGuid(), 0, "Hello, World!", null, PhrasePartType.TEXT,
                    _testingFixture.EntityName.Id, _testingFixture.EntityType.Id);
                intent.UpdatePhrases(new []{phrasePart});
                await context.AddRangeAsync(intent);
                await context.SaveChangesAsync();
                var actual = await _client.GetFromJsonAsync<IntentDto>(
                    $"/dms/api/v1/intents/{intent.Id}");
                
                // clean up
                context.RemoveRange(intent, phrasePart);
                await context.SaveChangesAsync();
                
                Equal(intent.Id.ToString(), actual.IntentId);
                Single(actual.PhraseParts);
                Equal(_testingFixture.EntityName.Name, actual.PhraseParts[0].EntityName);
                Equal(_testingFixture.EntityType.Id.ToString(), actual.PhraseParts[0].EntityTypeId);
            }); 
        }

        [Fact]
        public async Task CreateIntent()
        {
            // Arrange
            var phraseId1 = Guid.NewGuid().ToString();
            var phraseId2 = Guid.NewGuid().ToString();
            
            // Act
            var httpResponse = await _client.PostAsJsonAsync(
                "/dms/api/v1/intents", new CreateIntentRequest
                {
                    Name = "test intent",
                    ProjectId = _testingFixture.Project.Id.ToString(),
                    Type = IntentType.STANDARD.ToString(),
                    PhraseParts = new[]
                    {
                        new CreatePhrasePartDto
                        {
                            Text = "My favourite city is Kyoto",
                            PhraseId = phraseId1,
                            Type = PhrasePartType.TEXT.ToString(),
                        },
                        new CreatePhrasePartDto
                        {
                            EntityName = _testingFixture.EntityName.Name,
                            EntityTypeId = _testingFixture.EntityType.Id.ToString(),
                            Value = "Kyoto",
                            Type = PhrasePartType.CONSTANT_ENTITY.ToString(),
                            PhraseId = phraseId1
                        },
                        new CreatePhrasePartDto
                        {
                            Text = "My hometown is ",
                            PhraseId = phraseId2,
                            Type = PhrasePartType.TEXT.ToString(),
                        },
                        new CreatePhrasePartDto
                        {
                            Text = "Qingdao",
                            EntityName = "hometown",
                            EntityTypeId = _testingFixture.EntityType.Id.ToString(),
                            Type = PhrasePartType.ENTITY.ToString(),
                            PhraseId = phraseId2
                        }
                    }
                });
            await httpResponse.IsOk();
            var response = await httpResponse.Content.ReadFromJsonAsync<CreateIntentResponse>();
            EntityName? entityNameHometown = null;

            // clean up
            await _factory.WithDbContext(async context =>
            {
                entityNameHometown = context.EntityNames.First(e => e.Name == "hometown");
                context.RemoveRange(context.Intents
                    .Include(i => i.PhraseParts)
                    .Single(i => i.Id == Guid.Parse(response.IntentId)).PhraseParts);
                context.Intents.Remove(context.Intents.Single(i => i.Id == Guid.Parse(response.IntentId)));
                if (entityNameHometown != null)
                {
                    context.Remove(entityNameHometown);
                }
                await context.SaveChangesAsync();
            });
            
            // Assert
            NotNull(entityNameHometown);
            NotNull(response);
            Equal(_testingFixture.Project.Id.ToString(), response.ProjectId);
            Equal("test intent", response.Name);
            Equal(4, response.PhraseParts.Length);
        }

        [Fact]
        public async Task UpdateIntent()
        {
            // Arrange
            var intent = new Intent("HelloWorld", _testingFixture.Project.Id,
                IntentType.STANDARD);
            var phraseId = Guid.NewGuid();
            var phrasePart1 = new PhrasePart(intent.Id,
                phraseId, 0, "hello world", null, PhrasePartType.TEXT,
                null, null);
            var phrasePart2 = new PhrasePart(intent.Id,
                phraseId, 0, null,"Beijing", PhrasePartType.CONSTANT_ENTITY,
                _testingFixture.EntityName.Id, _testingFixture.EntityType.Id);
            intent.UpdatePhrases(new PhrasePart[]
            {
                phrasePart1,
                phrasePart2
            });
            
            await _factory.WithDbContext(async context =>
            {
                await context.AddAsync(intent);
                await context.SaveChangesAsync();
            });

            // Act
            var phraseId2 = Guid.NewGuid();
            var httpResponse = await _client.PutAsJsonAsync(
                $"/dms/api/v1/intents/{intent.Id}",
                new UpdateIntentRequest
                {
                    Name = "helloWorld",
                    PhraseParts = new[]
                    {
                        new CreatePhrasePartDto
                        {
                            Text = "Hello World!",
                            PhraseId = phraseId.ToString(),
                            Type = PhrasePartType.TEXT.ToString()
                        },
                        new CreatePhrasePartDto
                        {
                            Value = "Shanghai",
                            PhraseId = phraseId.ToString(),
                            Type = PhrasePartType.CONSTANT_ENTITY.ToString(),
                            EntityName = _testingFixture.EntityName.Name,
                            EntityTypeId = _testingFixture.EntityType.Id.ToString()
                        },
                        new CreatePhrasePartDto
                        {
                            Text = "My flight departs from ",
                            PhraseId = phraseId2.ToString(),
                            Type = PhrasePartType.TEXT.ToString()
                        },
                        new CreatePhrasePartDto
                        {
                            Text = "Melbourne",
                            PhraseId = phraseId2.ToString(),
                            Type = PhrasePartType.ENTITY.ToString(),
                            EntityName = "TEST_departureCity",
                            EntityTypeId = _testingFixture.EntityType.Id.ToString()
                        }
                    }
                });

            await _factory.WithDbContext(async context =>
            {
                // clean up
                intent = await context.Intents
                    .Include(x => x.PhraseParts)
                    .FirstAsync(x => x.Id == intent.Id);
                context.RemoveRange(intent.PhraseParts);
                context.Remove(intent);
                await context.SaveChangesAsync();

                // Assert
                await httpResponse.IsOk();
                var response = await httpResponse.Content.ReadFromJsonAsync<UpdateIntentResponse>();
                Equal(4, response.PhraseParts.Length);
                Equal(intent.Id.ToString(), response.IntentId);
                Equal("helloWorld", response.Name);
                Contains(response.PhraseParts, p => 
                    p.EntityName == _testingFixture.EntityName.Name && p.Value == "Shanghai");
                Contains(response.PhraseParts, p => 
                    p.EntityName == "TEST_departureCity" && p.Text == "Melbourne");
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