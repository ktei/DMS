using System;
using System.Collections.Generic;
using System.Diagnostics;
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

                var createdIntents = await context.Intents.AsNoTracking().Where(x => x.Name == "i1" || x.Name == "t2")
                    .ToListAsync();
                
                // clean up
                context.RemoveRange(intent1, intent2);
                await context.SaveChangesAsync();

                True(actual.Count >= 2);
                Equal(2, createdIntents.Count);
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
                    default(Guid?), null);
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
            }); 
        }

        [Fact]
        public async Task CreateIntent()
        {
            // Arrange
            var (entityName, entityType) = await SetupFixture(_testingFixture.Project.Id);
            
            // Act
            var httpResponse = await _client.PostAsJsonAsync(
                "/dms/api/v1/intents", new CreateIntentRequest
                {
                    Name = "test intent",
                    ProjectId = _testingFixture.Project.Id.ToString(),
                    Type = IntentType.STANDARD.ToString(),
                    Phrases = new[]
                    {
                        new[]
                        {
                            new CreatePhrasePartDto
                            {
                                Text = "My favourite city is Kyoto",
                                Type = PhrasePartType.TEXT.ToString(),
                            },
                            new CreatePhrasePartDto
                            {
                                EntityName = entityName.Name,
                                EntityTypeId = entityType.Id.ToString(),
                                Value = "Kyoto",
                                Type = PhrasePartType.CONSTANT_ENTITY.ToString(),
                            },
                        },
                        new[]
                        {
                            new CreatePhrasePartDto
                            {
                                Text = "My hometown is ",
                                Type = PhrasePartType.TEXT.ToString(),
                            },
                            new CreatePhrasePartDto
                            {
                                Text = "Qingdao",
                                EntityName = "hometown",
                                EntityTypeId = entityType.Id.ToString(),
                                Type = PhrasePartType.ENTITY.ToString(),
                            }
                        }
                    }
                });
            await httpResponse.IsOk();
            var response = await httpResponse.Content.ReadFromJsonAsync<CreateIntentResponse>();
            EntityName? entityNameHometown = null;

            // clean up
            await _factory.WithDbContext(async context =>
            {
                entityNameHometown = await context.EntityNames.FirstOrDefaultAsync(e => e.Name == "hometown");
                context.AttachRange(entityName, entityType);
                context.RemoveRange(entityName, entityType, entityNameHometown);
                context.Remove(context.Intents.Single(i => i.Id == Guid.Parse(response.IntentId)));
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
            var (entityName, entityType) = await SetupFixture(_testingFixture.Project.Id);
            
            var intent = new Intent("HelloWorld", _testingFixture.Project.Id,
                IntentType.STANDARD);
            var phraseId = Guid.NewGuid();
            var phrasePart1 = new PhrasePart(intent.Id,
                phraseId, 0, "hello world", null, PhrasePartType.TEXT,
                default(Guid?), null);
            var phrasePart2 = new PhrasePart(intent.Id,
                phraseId, 0, null,"Beijing", PhrasePartType.CONSTANT_ENTITY,
                entityName.Id, entityType.Id);
            intent.UpdatePhrases(new[]
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
            var httpResponse = await _client.PutAsJsonAsync(
                $"/dms/api/v1/intents/{intent.Id}",
                new UpdateIntentRequest
                {
                    Name = "helloWorld",
                    Phrases = new[]
                    {
                        new[]
                        {
                            new CreatePhrasePartDto
                            {
                                Text = "Hello World!",
                                Type = PhrasePartType.TEXT.ToString()
                            },
                            new CreatePhrasePartDto
                            {
                                Value = "Shanghai",
                                Type = PhrasePartType.CONSTANT_ENTITY.ToString(),
                                EntityName = entityName.Name,
                                EntityTypeId = entityType.Id.ToString()
                            }
                        },
                        new[]
                        {
                            new CreatePhrasePartDto
                            {
                                Text = "My flight departs from ",
                                Type = PhrasePartType.TEXT.ToString()
                            },
                            new CreatePhrasePartDto
                            {
                                Text = "Melbourne",
                                Type = PhrasePartType.ENTITY.ToString(),
                                EntityName = "TEST_departureCity",
                                EntityTypeId = entityType.Id.ToString()
                            }
                        }
                    }
                });

            await _factory.WithDbContext(async context =>
            {
                // clean up
                context.AttachRange(entityName, entityType);
                intent = await context.Intents
                    .Include(x => x.PhraseParts)
                    .FirstAsync(x => x.Id == intent.Id);
                // context.RemoveRange(intent.PhraseParts);
                var departureCity = await context.EntityNames.FirstOrDefaultAsync(e => e.Name == "TEST_departureCity");
                context.Remove(intent);
                context.RemoveRange(entityName, entityType, departureCity);
                await context.SaveChangesAsync();

                // Assert
                await httpResponse.IsOk();
                var response = await httpResponse.Content.ReadFromJsonAsync<UpdateIntentResponse>();
                Equal(4, response.PhraseParts.Length);
                Equal(intent.Id.ToString(), response.IntentId);
                Equal("helloWorld", response.Name);
                Contains(response.PhraseParts, p => 
                    p.EntityName == entityName.Name && p.Value == "Shanghai");
                Contains(response.PhraseParts, p => 
                    p.EntityName == "TEST_departureCity" && p.Text == "Melbourne");
            });
        }
        
        private async Task<(EntityName entityName, EntityType entityType)> SetupFixture(Guid projectId)
        {
            EntityName? entityName = null;
            EntityType? entityType = null;
            await _factory.WithDbContext(async context =>
            {
                entityName =
                    (await context.AddAsync(new EntityName(TestingFixture.RandomString(15), projectId, true)))
                    .Entity;
                entityType =
                    (await context.AddAsync(new EntityType(TestingFixture.RandomString(15), projectId, "test", null)))
                    .Entity;
                await context.SaveChangesAsync();
            });
            Debug.Assert(entityName != null);
            Debug.Assert(entityType != null);

            return (entityName, entityType);
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