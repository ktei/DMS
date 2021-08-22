using System;
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
    public class ProjectVersionRepositoryTests : RepositoryTestBase
    {
        public ProjectVersionRepositoryTests(SharedDatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task FindLatestByProjectId()
        {
            var context = Fixture.CreateContext();
            var organisation = await context.Organisations.FirstAsync();
            var project = Project.CreateWithDefaults(organisation.Id, "TEST_PROJECT_VERSION");
            await context.AddAsync(project);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            var projectV2 = Project.CreateWithDefaults(project.OrganisationId, Guid.NewGuid().ToString());
            var projectVersion = new ProjectVersion(projectV2.Id,
                projectV2.OrganisationId, project.Id, project.ProjectVersion!.Version.Next());
            await context.AddRangeAsync(projectV2, projectVersion);
            await context.SaveChangesAsync();
            var sut = new ProjectVersionRepository(context);

            context.ChangeTracker.Clear();
            var actual = await sut.FindLatestByProjectId(project.Id);

            actual.Should().NotBeNull();
            actual!.Id.Should().Be(projectVersion.Id);
        }
    }
}