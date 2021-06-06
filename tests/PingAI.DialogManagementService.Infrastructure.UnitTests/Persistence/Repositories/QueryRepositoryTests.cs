// using System;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using PingAI.DialogManagementService.Domain.Model;
// using PingAI.DialogManagementService.Infrastructure.Persistence;
// using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
// using PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Helpers;
// using Xunit;
// using static Xunit.Assert;
//
// namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Repositories
// {
//     public class QueryRepositoryTests : IAsyncLifetime
//     {
//         private readonly DialogManagementContextFactory _dialogManagementContextFactory;
//         private readonly TestDataFactory _testDataFactory;
//         
//         public QueryRepositoryTests()
//         {
//             _dialogManagementContextFactory = new DialogManagementContextFactory();
//             _testDataFactory = new TestDataFactory(_dialogManagementContextFactory.CreateDbContext(new string[] { }));
//         }
//
//         [Fact]
//         public async Task AddQueryWithIntent()
//         {
//             // Arrange
//             await using var context = _dialogManagementContextFactory.CreateDbContext(new string[] { });
//             var project = _testDataFactory.Project;
//             var sut = new QueryRepository(context);
//             var intent = new Intent("TEST_welcome", project.Id, IntentType.STANDARD);
//             var phraseParts = new []{
//                 new PhrasePart(intent.Id,
//                     Guid.NewGuid(), 0, "Welcome", null, 
//                     PhrasePartType.TEXT, default(Guid?), null, 1),
//             };
//             intent.UpdatePhrases(phraseParts);
//             await context.AddAsync(intent);
//             await context.SaveChangesAsync();
//             
//             // Act
//             var query = new Query("TEST_query", project.Id, new Expression[0],
//                 "TEST_query_description", new []{"t1"}, 0);
//             query.AddIntent(intent);
//             query = await sut.AddQuery(query);
//             await context.SaveChangesAsync();
//             var actual = await context.Queries.AsNoTracking()
//                 .Include(x => x.QueryIntents).ThenInclude(qi => qi.Intent)
//                 .FirstOrDefaultAsync(x => x.Id == query.Id); 
//
//             // Assert
//
//             // clean up
//             context.Remove(intent);
//             context.Remove(query);
//             await context.SaveChangesAsync();
//             
//             NotNull(actual);
//             Single(actual.QueryIntents);
//             Equal("TEST_welcome", actual.QueryIntents[0]!.Intent!.Name);
//         }
//         
//
//         [Fact]
//         public async Task AddQueryWithResponse()
//         {
//             // Arrange
//             await using var context = _dialogManagementContextFactory.CreateDbContext(new string[] { });
//             var project = _testDataFactory.Project;
//             var sut = new QueryRepository(context);
//             var response = new Response(new Resolution(new[]
//             {
//                 new ResolutionPart("Hello, ", null, ResolutionPartType.RTE),
//                 new ResolutionPart("World!", null, ResolutionPartType.RTE)
//             }), project.Id, ResponseType.RTE, 0);
//             await context.AddAsync(response);
//             await context.SaveChangesAsync();
//             
//             // Act
//             var query = new Query("TEST_query", project.Id, new Expression[0],
//                 "TEST_query_description", new []{"t1"}, 0);
//             query.AddResponse(response);
//             query = await sut.AddQuery(query);
//             await context.SaveChangesAsync();
//             var actual = await context.Queries.AsNoTracking()
//                 .Include(x => x.QueryResponses).ThenInclude(qr => qr.Response)
//                 .FirstOrDefaultAsync(x => x.Id == query.Id);
//
//             // Assert
//
//             // clean up
//             context.RemoveRange(query.QueryResponses);
//             context.RemoveRange(response, query);
//             await context.SaveChangesAsync();
//             
//             NotNull(actual);
//             Single(actual.QueryResponses);
//             Equal(2, actual.QueryResponses[0]!.Response!.Resolution!.Parts!.Length);
//         }
//         
//         public Task InitializeAsync() => _testDataFactory.Setup();
//
//         public Task DisposeAsync() => _testDataFactory.Cleanup();        
//     }
// }