using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
using PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Helpers;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Repositories
{
    public class QueryRepositoryTests : IAsyncLifetime
    {
        private readonly DialogManagementContextFactory _dialogManagementContextFactory;
        private readonly TestDataFactory _testDataFactory;
        
        public QueryRepositoryTests()
        {
            _dialogManagementContextFactory = new DialogManagementContextFactory();
            _testDataFactory = new TestDataFactory(_dialogManagementContextFactory.CreateDbContext(new string[] { }));
        }

        [Fact]
        public async Task AddQueryWithIntent()
        {
            // Arrange
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[] { });
            var project = _testDataFactory.Project;
            var sut = new QueryRepository(context);
            var intent = new Intent("TEST_welcome", project.Id, IntentType.STANDARD);
            object[] phraseParts = {
                new PhrasePart(intent.Id,
                    Guid.NewGuid(), 0, "Welcome", null, 
                    PhrasePartType.TEXT, null, null),
            };
            await context.AddRangeAsync(intent);
            await context.AddRangeAsync(phraseParts);
            await context.SaveChangesAsync();
            
            // Act
            var query = new Query("TEST_query", project.Id, new Expression[0],
                "TEST_query_description", new []{"t1"}, 0);
            query.AddIntent(intent);
            query = await sut.AddQuery(query);
            await context.SaveChangesAsync();
            var actual = await context.Queries.AsNoTracking()
                .Include(x => x.QueryIntents).ThenInclude(qi => qi.Intent)
                .FirstOrDefaultAsync(x => x.Id == query.Id); 

            // Assert

            // clean up
            context.RemoveRange(query.QueryIntents);
            context.RemoveRange(phraseParts);
            context.RemoveRange(intent, query);
            await context.SaveChangesAsync();
            
            NotNull(actual);
            Single(actual.QueryIntents);
            Equal("TEST_welcome", actual.QueryIntents[0]!.Intent!.Name);
        }
        

        [Fact]
        public async Task AddQueryWithResponse()
        {
            // Arrange
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[] { });
            var project = _testDataFactory.Project;
            var sut = new QueryRepository(context);
            var response = new Response(new[]
            {
                new ResolutionPart("Hello, ", null, ResolutionPartType.TEXT),
                new ResolutionPart("World!", null, ResolutionPartType.TEXT)
            }, project.Id, ResponseType.RTE, 0);
            await context.AddRangeAsync(response);
            await context.SaveChangesAsync();
            
            // Act
            var query = new Query("TEST_query", project.Id, new Expression[0],
                "TEST_query_description", new []{"t1"}, 0);
            query.AddResponse(response);
            query = await sut.AddQuery(query);
            await context.SaveChangesAsync();
            var actual = await context.Queries.AsNoTracking()
                .Include(x => x.QueryResponses).ThenInclude(qr => qr.Response)
                .FirstOrDefaultAsync(x => x.Id == query.Id);

            // Assert

            // clean up
            context.RemoveRange(query.QueryResponses);
            context.RemoveRange(response, query);
            await context.SaveChangesAsync();
            
            NotNull(actual);
            Single(actual.QueryResponses);
            Equal(2, actual.QueryResponses[0]!.Response!.Resolution.Length);
        }
        
        public Task InitializeAsync() => _testDataFactory.Setup();

        public Task DisposeAsync() => _testDataFactory.Cleanup();        
    }
}