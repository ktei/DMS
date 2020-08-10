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
            _client = _factory.CreateAuthenticatedClient();
        }

        [Fact]
        public async Task ListQueries()
        {
            var projectId = _testingFixture.Project.Id;
            
            // insert a Query first
            Query? query = null;
            await _factory.WithDbContext(async context =>
            {
                query = new Query(Guid.NewGuid().ToString(),
                    projectId, new Expression[0], Guid.NewGuid().ToString(),
                    new[]{"t1", "t2"}, 0);
                var resp = new Response(projectId, ResponseType.RTE, 0);
                resp.SetRteText("Hello, World!", new Dictionary<string, EntityName>(0));
                query.AddResponse(resp);
                var intent = new Intent(Guid.NewGuid().ToString(), projectId, IntentType.STANDARD);
                query.AddIntent(intent);
                query = (await context.AddAsync(query)).Entity;
                await context.SaveChangesAsync();
            });
            Debug.Assert(query != null); 
            
            var httpResponse = await _client.GetAsync(
                $"/dms/api/v1/queries?projectId={projectId}"
            );
            await httpResponse.IsOk();
            var response = await httpResponse.Content.ReadFromJsonAsync<List<QueryListItemDto>>();
            
            // clean up
            await _factory.WithDbContext(async context =>
            {
                query = await context.Queries
                    .Include(x => x.QueryIntents).ThenInclude(x => x.Intent)
                    .Include(x => x.QueryResponses).ThenInclude(x => x.Response)
                    .FirstOrDefaultAsync(resp => resp.Id == Guid.Parse(response.First().QueryId));
                context.RemoveRange(query.Intents);
                context.RemoveRange(query.Responses);
                context.Remove(query);
                await context.SaveChangesAsync();
            });

            Single(response);
            Equal("Hello, World!", response[0].ResponseText);
        }
        

        [Fact]
        public async Task GetQuery()
        {
            var projectId = _testingFixture.Project.Id;
            
            // insert a Query first
            Query? query = null;
            await _factory.WithDbContext(async context =>
            {
                query = new Query(Guid.NewGuid().ToString(),
                    projectId, new Expression[0], Guid.NewGuid().ToString(),
                    new string[]{"t1", "t2"}, 0);
                var resp = new Response(projectId, ResponseType.RTE, 0);
                resp.SetRteText("Hello, World!", new Dictionary<string, EntityName>(0));
                query.AddResponse(resp);
                var intent = new Intent(Guid.NewGuid().ToString(), projectId, IntentType.STANDARD);
                query.AddIntent(intent);
                query = (await context.AddAsync(query)).Entity;
                await context.SaveChangesAsync();
            });
            Debug.Assert(query != null);
            
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
            var phraseId1 = Guid.NewGuid();
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
                        new CreatePhrasePartDto
                        {
                            Text = "Hello, ",
                            PhraseId = phraseId1.ToString(),
                            Type = PhrasePartType.TEXT.ToString()
                        },
                        new CreatePhrasePartDto
                        {
                            Text = "World!",
                            PhraseId = phraseId1.ToString(),
                            Type = PhrasePartType.TEXT.ToString()
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
            var response = await httpResponse.Content.ReadFromJsonAsync<CreateQueryResponse>();
            
            // clean up
            await _factory.WithDbContext(async context =>
            {
                var query = await context.Queries
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
        public async Task UpdateQuery()
        {
            var projectId = _testingFixture.Project.Id;
            var phraseId1 = Guid.NewGuid();
            
            // insert a Query first
            Query? query = null;
            await _factory.WithDbContext(async context =>
            {
                query = new Query(Guid.NewGuid().ToString(),
                    projectId, new Expression[0], Guid.NewGuid().ToString(),
                    new string[]{"t1", "t2"}, 0);
                var resp = new Response(projectId, ResponseType.RTE, 0);
                resp.SetRteText("Hello, World!", new Dictionary<string, EntityName>(0));
                query.AddResponse(resp);
                var intent = new Intent(Guid.NewGuid().ToString(), projectId, IntentType.STANDARD);
                query.AddIntent(intent);
                query = (await context.AddAsync(query)).Entity;
                await context.SaveChangesAsync();
            });
            Debug.Assert(query != null);

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
                    Name = "TEST_query_intent",
                    PhraseParts = new[]
                    {
                        new CreatePhrasePartDto
                        {
                            Text = "Hello, ",
                            PhraseId = phraseId1.ToString(),
                            Type = PhrasePartType.TEXT.ToString()
                        },
                        new CreatePhrasePartDto
                        {
                            Text = "World!",
                            PhraseId = phraseId1.ToString(),
                            Type = PhrasePartType.TEXT.ToString()
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

        public async Task InitializeAsync()
        {
            (_testingFixture, _tearDownTestingFixture) = await _factory.SetupTestingFixture();
        }

        public Task DisposeAsync() => _tearDownTestingFixture.Invoke();
    }
}