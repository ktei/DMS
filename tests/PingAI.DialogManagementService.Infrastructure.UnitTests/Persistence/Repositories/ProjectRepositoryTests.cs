using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Repositories
{
    public class ProjectRepositoryTests
    {
        private readonly DialogManagementContextFactory _dialogManagementContextFactory;

        public ProjectRepositoryTests()
        {
            _dialogManagementContextFactory = new DialogManagementContextFactory();
        }

        [Fact]
        public async Task GetProjectById()
        {
            // Arrange
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[] { });
            var organisation =
                new Organisation("test", "test", null);
            var project = new Project( "test", organisation.Id, "title", "#ffffff",
                "description", "fallback message", "greeting message", new string[] { });
            organisation.AddProject(project);
            await context.AddAsync(organisation);
            await context.SaveChangesAsync();
            var sut = new ProjectRepository(context);

            // Act
            var actual = await sut.GetProjectById(project.Id);

            // Assert

            // clean up
            context.Remove(organisation);
            await context.SaveChangesAsync();

            NotNull(project);
            Equal(project.Id, actual!.Id);
        }

        [Fact]
        public async Task SaveProject()
        {
            // Arrange
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[] { });
            var organisation =
                new Organisation("test", "test", null);
            await context.AddAsync(organisation);
            await context.SaveChangesAsync();
            var sut = new ProjectRepository(context);
            var project = new Project("test", organisation.Id, null, "#ffffff",
                null, null, null, null);

            // Act
            await sut.AddProject(project);
            await context.SaveChangesAsync();
            var actual = await context.Projects
                .AsNoTracking()
                .FirstAsync(x => x.Id == project.Id);

            // Assert

            // clean up
            context.Projects.Remove(project);
            context.Organisations.Remove(organisation);
            await context.SaveChangesAsync();

            NotNull(project);
        }
    }
}