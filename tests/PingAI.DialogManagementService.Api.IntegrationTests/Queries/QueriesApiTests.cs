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
using PingAI.DialogManagementService.Api.Models.Queries;
using PingAI.DialogManagementService.Domain.Model;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Queries
{
    public class QueriesApiTests : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private readonly TestWebApplicationFactory _factory;
        private TestingFixture _testingFixture;
        private Func<Task> _tearDownTestingFixture;
        
        public QueriesApiTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateUserAuthenticatedClient();
        }

        [Fact]
        public async Task ListQueries()
        {
            var projectId = _testingFixture.Project.Id;
            var q = await SetupQuery(projectId);
            
            var httpResponse = await _client.GetAsync(
                $"/dms/api/v1/queries?projectId={projectId}&queryType=faq"
            );
            await httpResponse.IsOk();
            var response = await httpResponse.Content.ReadFromJsonAsync<List<QueryListItemDto>>();
            
            // clean up
            await _factory.WithDbContext(async context =>
            {
                var query = await context.Queries
                    .Include(x => x.QueryIntents).ThenInclude(x => x.Intent)
                    .Include(x => x.QueryResponses).ThenInclude(x => x.Response)
                    .FirstOrDefaultAsync(resp => resp.Id == Guid.Parse(response.First().QueryId));
                context.RemoveRange(query.Intents);
                context.RemoveRange(query.Responses);
                context.Remove(query);
                await context.SaveChangesAsync();
            });

            var actual = response.FirstOrDefault(r => 
                r.Intent.Name == q.Intents.First().Name);
            NotNull(actual);
            Equal("Hello, World!", actual.ResponseText);
        }
        

        [Fact]
        public async Task GetQuery()
        {
            var projectId = _testingFixture.Project.Id;
            
            // insert a Query first
            Query query = await SetupQuery(projectId);
            
            var httpResponse = await _client.GetAsync(
                $"/dms/api/v1/queries/{query.Id}"
            );
            await httpResponse.IsOk();
            var response = await httpResponse.Content.ReadFromJsonAsync<QueryDto>();
            
            // clean up
            await _factory.WithDbContext(async context =>
            {
                query = await context.Queries
                    .Include(x => x.QueryIntents).ThenInclude(x => x.Intent)
                    .Include(x => x.QueryResponses).ThenInclude(x => x.Response)
                    .FirstOrDefaultAsync(resp => resp.Id == Guid.Parse(response.QueryId));
                context.RemoveRange(query.Intents);
                context.RemoveRange(query.Responses);
                context.Remove(query);
                await context.SaveChangesAsync();
            });
            
            NotNull(response);
            Single(response.Intents);
            Single(response.Responses); 
        }

        [Fact]
        public async Task CreateQuery()
        {
            var projectId = _testingFixture.Project.Id;
            var (entityName, entityType) = await SetupEntityNamesAndTypes(projectId);
            var newEntityName = TestingFixture.RandomString(15);
            var payload = new CreateQueryRequest
            {
                Name = "TEST_query",
                DisplayOrder = 0,
                ProjectId = projectId.ToString(),
                Tags = new[] {"t1", "t2"},
                Intent = new CreateIntentDto
                {
                    Name = "TEST_query_intent",
                    PhraseParts = new[]
                    {
                        new []
                        {
                            new CreatePhrasePartDto
                            {
                                Text = "Hello, ",
                                Type = PhrasePartType.TEXT.ToString()
                            },
                            new CreatePhrasePartDto
                            {
                                Text = "World!",
                                Type = PhrasePartType.ENTITY.ToString(),
                                EntityName = entityName.Name,
                                EntityTypeId = entityType.Id.ToString(),
                            }
                        },
                        new []
                        {
                            new CreatePhrasePartDto
                            {
                                Text = "Hello ",
                                Type = PhrasePartType.TEXT.ToString()
                            },
                            new CreatePhrasePartDto
                            {
                                Text = "World",
                                Type = PhrasePartType.ENTITY.ToString(),
                                EntityName = newEntityName,
                                EntityTypeId = entityType.Id.ToString(),
                            }
                        }
                    }
                },
                Response = new CreateResponseDto
                {
                    Order = 0,
                    RteText = "Greetings!",
                    Type = ResponseType.RTE.ToString()
                }
            };
            var httpResponse = await _client.PostAsJsonAsync(
                "/dms/api/v1/queries", payload
            );
            await httpResponse.IsOk();
            var response = await httpResponse.Content.ReadFromJsonAsync<QueryDto>();
            
            // clean up
            await _factory.WithDbContext(async context =>
            {
                var query = await context.Queries
                    .Include(x => x.QueryIntents).ThenInclude(x => x.Intent)
                    .Include(x => x.QueryResponses).ThenInclude(x => x.Response)
                    .FirstOrDefaultAsync(resp => resp.Id == Guid.Parse(response.QueryId));
                var newEntity = await context.EntityNames.FirstAsync(x => x.Name == newEntityName);
                context.RemoveRange(query.Intents);
                context.RemoveRange(query.Responses);
                context.Remove(query);
                context.AttachRange(entityName, entityType);
                context.RemoveRange(entityName, entityType);
                context.Remove(newEntity);
                await context.SaveChangesAsync();
            });
            
            NotNull(response);
            Single(response.Intents);
            Equal(4, response.Intents.First().PhraseParts.Length);
            Contains(response.Intents.First().PhraseParts, p => p.EntityName == newEntityName);
            Single(response.Responses);
        }
        
        [Fact]
        public async Task CreateQueryV1_1()
        {
            var projectId = _testingFixture.Project.Id;
            var (entityName, entityType) = await SetupEntityNamesAndTypes(projectId);
            var newEntityName = TestingFixture.RandomString(15);
            var payload = new CreateQueryRequestV1_1
            {
                Name = "TEST_query",
                DisplayOrder = 0,
                ProjectId = projectId.ToString(),
                Tags = new[] {"t1", "t2"},
                Intent = new CreateIntentDto
                {
                    Name = "TEST_query_intent",
                    PhraseParts = new[]
                    {
                        new[]
                        {
                            new CreatePhrasePartDto
                            {
                                Text = "Hello, ",
                                Type = PhrasePartType.TEXT.ToString()
                            },
                            new CreatePhrasePartDto
                            {
                                Text = "World!",
                                Type = PhrasePartType.ENTITY.ToString(),
                                EntityName = entityName.Name,
                                EntityTypeId = entityType.Id.ToString(),
                            }
                        },
                        new[]
                        {
                            new CreatePhrasePartDto
                            {
                                Text = "Hello ",
                                Type = PhrasePartType.TEXT.ToString()
                            },
                            new CreatePhrasePartDto
                            {
                                Text = "World",
                                Type = PhrasePartType.ENTITY.ToString(),
                                EntityName = newEntityName,
                                EntityTypeId = entityType.Id.ToString(),
                            }
                        }
                    }
                },
                Responses = new[]
                {
                    new CreateResponseDto
                    {
                        Order = 0,
                        RteText = "Greetings!",
                        Type = ResponseType.RTE.ToString()
                    },
                    new CreateResponseDto
                    {
                        Order = 1,
                        Type = ResponseType.HANDOVER.ToString()
                    },
                    new CreateResponseDto
                    {
                        Order = 2,
                        Type = ResponseType.FORM.ToString(),
                        Form = new CreateResponseFormDto
                        {
                            Fields = new []
                            {
                                new CreateResponseFormDto.Field
                                {
                                    DisplayName = "Name",
                                    Name = "NAME"
                                }
                            }
                        }
                    }
                }
            };
            var httpResponse = await _client.PostAsJsonAsync(
                "/dms/api/v1.1/queries", payload
            );
            await httpResponse.IsOk();
            var response = await httpResponse.Content.ReadFromJsonAsync<QueryDto>();
            
            // clean up
            await _factory.WithDbContext(async context =>
            {
                var query = await context.Queries
                    .Include(x => x.QueryIntents).ThenInclude(x => x.Intent)
                    .Include(x => x.QueryResponses).ThenInclude(x => x.Response)
                    .FirstOrDefaultAsync(resp => resp.Id == Guid.Parse(response.QueryId));
                var newEntity = await context.EntityNames.FirstAsync(x => x.Name == newEntityName);
                context.RemoveRange(query.Intents);
                context.RemoveRange(query.Responses);
                context.Remove(query);
                context.AttachRange(entityName, entityType);
                context.RemoveRange(entityName, entityType);
                context.Remove(newEntity);
                await context.SaveChangesAsync();
            });
            
            NotNull(response);
            Single(response.Intents);
            Equal(4, response.Intents.First().PhraseParts.Length);
            Contains(response.Intents.First().PhraseParts, p => p.EntityName == newEntityName);
            Equal(3, response.Responses.Length);
        }
        
        [Fact]
        public async Task UpdateQuery()
        {
            var projectId = _testingFixture.Project.Id;
            
            // insert a Query first
            Query query = await SetupQuery(projectId);

            var oldIntentIds = query.Intents.Select(x => x.Id).ToArray();
            var oldResponseIds = query.Responses.Select(x => x.Id).ToArray();

            
            // now try to update the query we inserted
            var payload = new UpdateQueryRequest
            {
                Name = "TEST_query",
                DisplayOrder = 0,
                Tags = new[] {"t1", "t2"},
                Intent = new CreateIntentDto
                {
                    Name = query.Intents.First().Name, // "TEST_query_intent",
                    PhraseParts = new[]
                    {
                        new[]
                        {
                            new CreatePhrasePartDto
                            {
                                Text = "Hello, ",
                                Type = PhrasePartType.TEXT.ToString()
                            },
                            new CreatePhrasePartDto
                            {
                                Text = "World!",
                                Type = PhrasePartType.TEXT.ToString()
                            }
                        }
                    }
                },
                Response = new CreateResponseDto
                {
                    Order = 0,
                    RteText = "Greetings!",
                    Type = ResponseType.RTE.ToString()
                }
            };
            var httpResponse = await _client.PutAsJsonAsync(
                $"/dms/api/v1/queries/{query.Id}", payload
            );
            await httpResponse.IsOk();
            var response = await httpResponse.Content.ReadFromJsonAsync<UpdateQueryResponse>();
            
            // clean up
            await _factory.WithDbContext(async context =>
            {
                query = await context.Queries
                    .Include(x => x.QueryIntents).ThenInclude(x => x.Intent)
                    .Include(x => x.QueryResponses).ThenInclude(x => x.Response)
                    .FirstOrDefaultAsync(resp => resp.Id == Guid.Parse(response.QueryId));
                context.RemoveRange(query.Intents);
                context.RemoveRange(query.Responses);

                var oldIntents = await context.Intents.Where(x => oldIntentIds.Contains(x.Id)).ToListAsync();
                var oldResponses = await context.Responses.Where(x => oldResponseIds.Contains(x.Id)).ToListAsync();
                context.RemoveRange(oldIntents);
                context.RemoveRange(oldResponses);
                
                context.Remove(query);
                await context.SaveChangesAsync();
            });
            
            NotNull(response);
            Single(response.Intents);
            Single(response.Responses);
        }
        
        [Fact]
        public async Task UpdateQueryV1_1()
        {
            var projectId = _testingFixture.Project.Id;
            
            // insert a Query first
            Query query = await SetupQuery(projectId);

            var oldIntentIds = query.Intents.Select(x => x.Id).ToArray();
            var oldResponseIds = query.Responses.Select(x => x.Id).ToArray();

            
            // now try to update the query we inserted
            var payload = new UpdateQueryRequestV1_1
            {
                Name = "TEST_query",
                DisplayOrder = 0,
                Tags = new[] {"t1", "t2"},
                Intent = new CreateIntentDto
                {
                    Name = query.Intents.First().Name, // "TEST_query_intent",
                    PhraseParts = new[]
                    {
                        new[]
                        {
                            new CreatePhrasePartDto
                            {
                                Text = "Hello, ",
                                Type = PhrasePartType.TEXT.ToString()
                            },
                            new CreatePhrasePartDto
                            {
                                Text = "World!",
                                Type = PhrasePartType.TEXT.ToString()
                            }
                        }
                    }
                },
                Responses = new[]
                {
                    new CreateResponseDto
                    {
                        Order = 0,
                        RteText = "Greetings!",
                        Type = ResponseType.RTE.ToString()
                    },
                    new CreateResponseDto
                    {
                        Order = 1,
                        Type = ResponseType.HANDOVER.ToString()
                    },
                    new CreateResponseDto
                    {
                        Order = 2,
                        Type = ResponseType.FORM.ToString(),
                        Form = new CreateResponseFormDto
                        {
                            Fields = new []
                            {
                                new CreateResponseFormDto.Field
                                {
                                    DisplayName = "Name",
                                    Name = "NAME"
                                }
                            }
                        }
                    }
                }
            };
            var httpResponse = await _client.PutAsJsonAsync(
                $"/dms/api/v1.1/queries/{query.Id}", payload
            );
            await httpResponse.IsOk();
            var response = await httpResponse.Content.ReadFromJsonAsync<UpdateQueryResponse>();
            
            // clean up
            await _factory.WithDbContext(async context =>
            {
                query = await context.Queries
                    .Include(x => x.QueryIntents).ThenInclude(x => x.Intent)
                    .Include(x => x.QueryResponses).ThenInclude(x => x.Response)
                    .FirstOrDefaultAsync(resp => resp.Id == Guid.Parse(response.QueryId));
                context.RemoveRange(query.Intents);
                context.RemoveRange(query.Responses);

                var oldIntents = await context.Intents.Where(x => oldIntentIds.Contains(x.Id)).ToListAsync();
                var oldResponses = await context.Responses.Where(x => oldResponseIds.Contains(x.Id)).ToListAsync();
                context.RemoveRange(oldIntents);
                context.RemoveRange(oldResponses);
                
                context.Remove(query);
                await context.SaveChangesAsync();
            });
            
            NotNull(response);
            Single(response.Intents);
            Equal(3, response.Responses.Length);
        }
        
        [Fact]
        public async Task DeleteQuery()
        {
            var projectId = _testingFixture.Project.Id;
            var query = await SetupQuery(projectId);
            
            var httpResponse = await _client.DeleteAsync($"/dms/api/v1/queries/{query.Id}");
            await httpResponse.IsOk();

            await _factory.WithDbContext(async context =>
            {
                var q = await context.Queries
                    .FirstOrDefaultAsync(resp => resp.Id == query.Id);

                // query should be deleted already
                Null(q);
            });
        }

        private async Task<Query> SetupQuery(Guid projectId)
        {
            Query? query = null;
            await _factory.WithDbContext(async context =>
            {
                query = new Query(Guid.NewGuid().ToString(),
                    projectId, new Expression[0], Guid.NewGuid().ToString(),
                    new[]{"t1", "t2"}, 0);
                var resp = new Response(projectId, ResponseType.RTE, 0);
                resp.SetRteText("Hello, World!", new Dictionary<string, EntityName>(0));
                query.AddResponse(resp);
                var intent = new Intent(TestingFixture.RandomString(15), projectId, IntentType.STANDARD, new PhrasePart[0]);
                query.AddIntent(intent);
                query = (await context.AddAsync(query)).Entity;
                await context.SaveChangesAsync();
            });
            Debug.Assert(query != null);
            return query;
        }
        
        private async Task<(EntityName entityName, EntityType entityType)> SetupEntityNamesAndTypes(Guid projectId)
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

        public Task DisposeAsync() => _tearDownTestingFixture.Invoke();
    }
}