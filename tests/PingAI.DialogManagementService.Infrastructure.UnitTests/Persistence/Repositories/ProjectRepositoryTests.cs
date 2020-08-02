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
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[]{});
            var organisation =
                new Organisation(Guid.NewGuid(), "test", "test", null);
            var project = new Project(Guid.NewGuid(), "test", organisation.Id, "title", "#ffffff",
                "description", "fallback message", "greeting message");
            await context.AddAsync(organisation);
            await context.AddAsync(project);
            await context.SaveChangesAsync();
            var sut = new ProjectRepository(context);
            
            // Act
            var actual = await sut.GetProjectById(project.Id);

            // Assert
            
            // clean up
            context.Projects.Remove(project);
            context.Organisations.Remove(organisation);
            await context.SaveChangesAsync();
            
            NotNull(project);
            Equal(project.Id, actual!.Id);
        }

        [Fact]
        public async Task SaveProject()
        {
            // Arrange
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[]{});
            var organisation =
                new Organisation(Guid.NewGuid(), "test", "test", null);
            await context.AddAsync(organisation);
            await context.SaveChangesAsync();
            var sut = new ProjectRepository(context);
            var project = new Project(Guid.NewGuid(), "test", organisation.Id, "title", "#ffffff",
                "description", "fallback message", "greeting message");
            
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