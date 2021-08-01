using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Domain.Model;
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
        public async Task ListByOrganisationId()
        {
            var context = Fixture.CreateContext();
            var sut = new ProjectRepository(context);
            var organisation = await context.Organisations.FirstAsync();
            
            context.ChangeTracker.Clear();
            var actual = await sut.ListByOrganisationId(organisation.Id);

            actual.Should().HaveCountGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task ListAll()
        {
            var context = Fixture.CreateContext();
            var sut = new ProjectRepository(context);

            var actual = await sut.ListAll();

            actual.Should().HaveCountGreaterOrEqualTo(1);
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

        [Fact]
        public async Task FindLatestVersion()
        {
            var context = Fixture.CreateContext();
            var sut = new ProjectRepository(context);
            var project = await context.Projects
                .FirstAsync(x => x.ProjectVersion!.Version == ProjectVersionNumber.DesignTime);
            var latestVersion = await context.ProjectVersions
                .Where(x => x.VersionGroupId == project.Id)
                .OrderBy("version DESC")
                .FirstAsync();
            var publishedProject = Project.CreateWithDefaults(project.OrganisationId, Guid.NewGuid().ToString());
            await context.AddRangeAsync(publishedProject, new ProjectVersion(publishedProject.Id,
                project.OrganisationId, project.Id, latestVersion.Version.Next()));
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();
            var actual = await sut.FindLatestVersion(project.Id);

            actual.Should().NotBeNull();
            actual!.Id.Should().Be(publishedProject.Id);
        }

        [Fact]
        public async Task Add()
        {
            var context = Fixture.CreateContext();
            var sut = new ProjectRepository(context);
            var organisation = await context.Organisations.FirstAsync();
            var project = Project.CreateWithDefaults(organisation.Id, Guid.NewGuid().ToString());

            await sut.Add(project);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();
            var actual = context.Projects.FirstOrDefaultAsync(x => x.Id == project.Id);
            actual.Should().NotBeNull();
        }

        [Fact]
        public async Task ImportFromAnotherProjectAndSaveIt()
        {
            var context = Fixture.CreateContext();
            var sut = new ProjectRepository(context);
            var firstProject = await context.Projects.FirstAsync(x => x.Name == SharedDatabaseFixture.DefaultProjectName);
            var project = await sut.FindByIdWithJoins(firstProject.Id);
            var project2 = Project.CreateWithDefaults(project!.OrganisationId, Guid.NewGuid().ToString());
            project2.Import(project);
            await sut.Add(project2);

            await context.SaveChangesAsync();
            
            context.ChangeTracker.Clear();
            var actual = await sut.FindByIdWithJoins(project2.Id);
            actual.Should().NotBeNull();
            actual!.Intents.Should().HaveCount(project.Intents.Count);
            actual.Responses.Should().HaveCount(project.Responses.Count);
            actual.Queries.Should().HaveCount(project.Queries.Count);
            actual.GreetingResponses.Should().HaveCount(project.GreetingResponses.Count);
            actual.EntityNames.Should().HaveCount(project.EntityNames.Count);
            actual.EntityTypes.Should().HaveCount(project.EntityTypes.Count);
        }
    }
}
