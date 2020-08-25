using System;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Repositories
{
    public class ProjectVersionRepositoryTests
    {
        private readonly DialogManagementContextFactory _dialogManagementContextFactory;

        public ProjectVersionRepositoryTests()
        {
            _dialogManagementContextFactory = new DialogManagementContextFactory();
        }

        [Fact]
        public async Task GetDesignTimeVersionsByOrganisationId()
        {
            // Arrange
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[] { });
            var organisation =
                new Organisation(Guid.NewGuid().ToString(), "test", null);
            var project = new Project( "test", organisation.Id,  "title", "#ffffff",
                "description", "fallback message", "greeting message", new string[] { });
            organisation.AddProject(project);
            var projectVersion = new ProjectVersion(project, organisation.Id,
                Guid.NewGuid(), Ver.NewDesignTime());
            organisation.AddProjectVersion(projectVersion);
            await context.AddAsync(organisation);
            await context.SaveChangesAsync();
            var sut = new ProjectVersionRepository(context);
            
            // Act
            var actual = await sut.GetDesignTimeVersionsByOrganisationId(organisation.Id);
            
            // cleanup
            context.RemoveRange(organisation, project, projectVersion);
            await context.SaveChangesAsync();
            
            // Assert
            Single(actual);
            Equal(actual[0].ProjectId, project.Id);
            Equal(Ver.DesignTime, actual[0].Version);
        }
    }
}