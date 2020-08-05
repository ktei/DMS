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
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[] { });
            var project = _testDataFactory.Project;
            var intent = new Intent(Guid.NewGuid(), "welcome",
                project.Id, IntentType.STANDARD);
            var sut = new IntentRepository(context);

            // Act
            var phraseId1 = Guid.NewGuid();
            var phraseId2 = Guid.NewGuid();
            intent.UpdatePhrases(new PhrasePart[]
            {
                new PhrasePart(Guid.NewGuid(), intent.Id, phraseId1, 0, "Hello, World!",
                    null, PhrasePartType.TEXT, null, null),
                
                // new PhrasePart(Guid.NewGuid(), intent.Id, phraseId2, 0, "Hello, my name is ",
                //     null, PhrasePartType.TEXT, null, null),
                // new PhrasePart(Guid.NewGuid(), intent.Id, phraseId2, 1, "Rui",
                //     null, PhrasePartType.ENTITY, null, null),

            });
            await sut.AddIntent(intent);
            await context.SaveChangesAsync();
            var actual = await context.Intents.SingleAsync(x => x.Id == intent.Id);

            // clean up
            context.RemoveRange(intent.PhraseParts);
            context.Remove(intent);
            await context.SaveChangesAsync();

            // Assert
            Equal(intent.Id, actual.Id);
            Single(actual.PhraseParts);
            Equal(phraseId1, actual.PhraseParts[0].PhraseId);
        }

        public Task InitializeAsync() => _testDataFactory.Setup();

        public Task DisposeAsync() => _testDataFactory.Cleanup();
    }
}