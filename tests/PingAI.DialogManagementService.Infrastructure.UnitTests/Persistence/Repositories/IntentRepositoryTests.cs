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
            var intent1 = new Intent("generic", project.Id, IntentType.GENERIC);
            var intent2 = new Intent("standard", project.Id, IntentType.STANDARD);
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
        public async Task GetIntent()
        {
            // TODO
        }

        [Fact]
        public async Task AddIntentWithPhraseParts()
        {
            // Arrange
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[] { });
            var project = _testDataFactory.Project;
            var intent = new Intent("welcome",
                project.Id, IntentType.STANDARD);
            var sut = new IntentRepository(context);

            // Act
            var phraseId1 = Guid.NewGuid();
            var phraseId2 = Guid.NewGuid();
            var phraseId3 = Guid.NewGuid();
            intent.UpdatePhrases(new[]
            {
                new PhrasePart(intent.Id, phraseId1, 0, "Hello, World!",
                    null, PhrasePartType.TEXT, default(Guid?), default),
                
                new PhrasePart(intent.Id, phraseId2, 0, "The city is ",
                    null, PhrasePartType.TEXT, default(Guid?), default),
                new PhrasePart(intent.Id, phraseId2, 1, "Melbourne",
                    null, PhrasePartType.ENTITY, _testDataFactory.EntityName.Id,
                    _testDataFactory.EntityType.Id),
                
                new PhrasePart(intent.Id, phraseId3, 0, "My city is Kyoto",
                    null, PhrasePartType.TEXT, default(Guid?), default),
                new PhrasePart(intent.Id, phraseId3, null, null,
                    "Kyoto", PhrasePartType.CONSTANT_ENTITY, _testDataFactory.EntityName.Id,
                    _testDataFactory.EntityType.Id)
            });
            await sut.AddIntent(intent);
            await context.SaveChangesAsync();
            var actual = await context.Intents
                .Include(x => x.PhraseParts)
                .SingleAsync(x => x.Id == intent.Id);

            // clean up
            context.RemoveRange(intent.PhraseParts);
            context.Remove(intent);
            await context.SaveChangesAsync();

            // Assert
            Equal(intent.Id, actual.Id);
            Equal(5, actual.PhraseParts.Count);
        }

        public Task InitializeAsync() => _testDataFactory.Setup();

        public Task DisposeAsync() => _testDataFactory.Cleanup();
    }
}