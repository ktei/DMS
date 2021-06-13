using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Repositories
{
    public class ProjectVersionRepositoryTests : RepositoryTestBase
    {
        public ProjectVersionRepositoryTests(SharedDatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task FindLatestByProjectId()
        {
            var context = Fixture.CreateContext();
            var project = await context.Projects.Include(p => p.ProjectVersion).FirstAsync();
            var projectV2 = Project.CreateWithDefaults(project.OrganisationId);
            await context.AddAsync(projectV2);
            var projectVersion = await context.AddAsync(new ProjectVersion(projectV2.Id,
                projectV2.OrganisationId, project.Id, project.ProjectVersion!.Version.Next()));
            await context.SaveChangesAsync();
            var sut = new ProjectVersionRepository(context);

            var actual = await sut.FindLatestByProjectId(project.Id);

            actual.Should().NotBeNull();
            actual!.Id.Should().Be(projectVersion.Entity.Id);
        }
    }
}