using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
using PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Util;
using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Repositories
{
    public class EntityTypeRepositoryTests : RepositoryTestBase
    {
        public EntityTypeRepositoryTests(SharedDatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task ListByProjectId()
        {
            var context = Fixture.CreateContext();
            var sut = new EntityTypeRepository(context);
            var project = await context.Projects.Include(x => x.EntityTypes).FirstAsync();

            context.ChangeTracker.Clear();
            var actual = await sut.ListByProjectId(project.Id);

            actual.Should().HaveCountGreaterOrEqualTo(project.EntityTypes.Count);
        }
    }
}