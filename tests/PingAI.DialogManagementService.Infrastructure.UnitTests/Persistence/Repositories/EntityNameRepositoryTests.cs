using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
using PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Util;
using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Repositories
{
    public class EntityNameRepositoryTests : RepositoryTestBase
    {
        public EntityNameRepositoryTests(SharedDatabaseFixture fixture) : base(fixture)
        {
        }
        
        [Fact]
        public async Task ListByProjectId()
        {
            var context = Fixture.CreateContext();
            var sut = new EntityNameRepository(context);
            var project = await context.Projects.FirstAsync();

            context.ChangeTracker.Clear();
            var actual = await sut.ListByProjectId(project.Id);

            actual.Should().HaveCountGreaterOrEqualTo(2);
        }

        [Fact]
        public async Task Add()
        {
            var context = Fixture.CreateContext();
            var sut = new EntityNameRepository(context);
            var project = await context.Projects.FirstAsync();

            var entityName = await sut.Add(new EntityName(project.Id, "TEST_ENTITY", true));
            
            context.ChangeTracker.Clear();
            var actual = context.EntityNames.SingleOrDefaultAsync(x => x.Id == entityName.Id);
            actual.Should().NotBeNull();
        }
    }
}
