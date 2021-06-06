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
//     public class ResponseRepositoryTests : IAsyncLifetime
//     {
//         private readonly DialogManagementContextFactory _dialogManagementContextFactory;
//         private readonly TestDataFactory _testDataFactory;
//         
//         public ResponseRepositoryTests()
//         {
//             _dialogManagementContextFactory = new DialogManagementContextFactory();
//             _testDataFactory = new TestDataFactory(_dialogManagementContextFactory.CreateDbContext(new string[] { }));
//         }
//
//         [Fact]
//         public async Task AddResponse()
//         {
//             // Arrange
//             await using var context = _dialogManagementContextFactory.CreateDbContext(new string[] { });
//             var project = _testDataFactory.Project;
//             var sut = new ResponseRepository(context);
//             var response = new Response(new Resolution(new[]
//             {
//                 new ResolutionPart("Hello, ", null, ResolutionPartType.RTE),
//                 new ResolutionPart("World!", null, ResolutionPartType.RTE)
//             }), project.Id, ResponseType.RTE, 0);
//             
//             // Act
//             await sut.AddResponse(response);
//             await context.SaveChangesAsync();
//             var actual = await context.Responses.AsNoTracking()
//                 .FirstOrDefaultAsync(x => x.Id == response.Id);
//             
//             // Assert
//             
//             // clean up
//             context.Remove(response);
//             await context.SaveChangesAsync();
//             
//             NotNull(actual);
//             Equal(response.Id, actual.Id);
//             Equal(2, actual.Resolution!.Parts!.Length);
//         }
//         
//         public Task InitializeAsync() => _testDataFactory.Setup();
//
//         public Task DisposeAsync() => _testDataFactory.Cleanup();
//     }
// }