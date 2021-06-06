using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Repositories
{
    public class ProjectRepositoryTests : RepositoryTestBase
    {
        public ProjectRepositoryTests(SharedDatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task FindByIdWithJoins()
        {
            var context = Fixture.CreateContext();
            var sut = new ProjectRepository(context);
            var project = await context.Projects.FirstAsync();

            context.ChangeTracker.Clear();
            var actual = await sut.FindByIdWithJoins(project.Id);

            actual.Should().NotBeNull();
            actual!.Intents.Should().HaveCountGreaterOrEqualTo(1);
            actual.Responses.Should().HaveCountGreaterOrEqualTo(1);
            actual.GreetingResponses.Should().HaveCountGreaterOrEqualTo(1);
        }
        
        [Fact]
        public async Task FindById()
        {
            var context = Fixture.CreateContext();
            var sut = new ProjectRepository(context);
            var project = await context.Projects.FirstAsync();

            context.ChangeTracker.Clear();
            var actual = await sut.FindById(project.Id);

            actual.Should().NotBeNull();
            actual!.GreetingResponses.Should().HaveCountGreaterOrEqualTo(1);
            actual.CreatedAt.Kind.Should().Be(DateTimeKind.Utc);
        }
    }
}
