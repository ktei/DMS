using System;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
using PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Helpers;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Repositories
{
    public class IntentRepositoryTests : IAsyncLifetime
    {
        private readonly DialogManagementContextFactory _dialogManagementContextFactory;
        private readonly TestDataFactory _testDataFactory;
        
        public IntentRepositoryTests()
        {
            _dialogManagementContextFactory = new DialogManagementContextFactory();
            _testDataFactory = new TestDataFactory(_dialogManagementContextFactory.CreateDbContext(new string[] { }));
        }

        [Fact]
        public async Task GetIntentsByProjectId()
        {
            // Arrange
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[] { });
            var project = _testDataFactory.Project;
            var intent1 = new Intent(Guid.NewGuid(), "generic", project.Id, IntentType.GENERIC);
            var intent2 = new Intent(Guid.NewGuid(), "standard", project.Id, IntentType.STANDARD);
            await context.AddRangeAsync(intent1, intent2);
            await context.SaveChangesAsync();
            var sut = new IntentRepository(context);
            
            // Act
            var actual = await sut.GetIntentsByProjectId(project.Id);

            // Assert
            
            // clean up
            context.Intents.RemoveRange(intent1, intent2);
            await context.SaveChangesAsync();
            
            Equal(2, actual.Count);
        }

        [Fact]
        public async Task AddIntentWithPhraseParts()
        {
            // Arrange

            
            // Act
            
            // Assert
            // False(true);
            
            // clean up
        }

        public Task InitializeAsync() => _testDataFactory.Setup();

        public Task DisposeAsync() => _testDataFactory.Cleanup();
    }
}