using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Repositories
{
    public class IntentRepositoryTests : RepositoryTestBase
    {
        public IntentRepositoryTests(SharedDatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task ListByProjectId()
        {
            var context = Fixture.CreateContext();
            var sut = new IntentRepository(context);
            var project = await context.Projects.FirstAsync();

            context.ChangeTracker.Clear();
            var actual = await sut.ListByProjectId(project.Id);

            actual.Should().HaveCount(1);
        }

        [Fact]
        public async Task FindById()
        {
            var context = Fixture.CreateContext();
            var sut = new IntentRepository(context);
            var intent = await context.Intents.FirstAsync();

            context.ChangeTracker.Clear();
            var actual = await sut.FindById(intent.Id);

            actual.Should().NotBeNull();
        }

        [Fact]
        public async Task Add()
        {
            var context = Fixture.CreateContext();
            var sut = new IntentRepository(context);
            var project = await context.Projects.FirstAsync();
            var entityName = await context.EntityNames.FirstAsync();
            var intent = new Intent(project.Id, "TEST_INTENT", IntentType.STANDARD);
            intent.AddPhrase(new Phrase(0)
                .AppendText("Hello, this is my ")
                .AppendEntity("entity", entityName));

            await sut.Add(intent);
            
            context.ChangeTracker.Clear();
            var actual = context.Intents.FirstOrDefaultAsync(x => x.Id == intent.Id);
            actual.Should().NotBeNull();
        }

        [Fact]
        public async Task Remove()
        {
            var context = Fixture.CreateContext();
            var sut = new IntentRepository(context);
            var project = await context.Projects.FirstAsync();
            var intent = new Intent(project.Id, "TEST_INTENT", IntentType.STANDARD);
            await context.AddAsync(intent);
            await context.SaveChangesAsync();

            sut.Remove(intent);
            await context.SaveChangesAsync();
            
            context.ChangeTracker.Clear();
            var actual = await context.Intents.FirstOrDefaultAsync(x => x.Id == intent.Id);
            actual.Should().BeNull();
        }
    }
}
