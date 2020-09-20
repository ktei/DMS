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
    public class OrganisationRepositoryTests
    {
        private readonly DialogManagementContextFactory _dialogManagementContextFactory;
        
        public OrganisationRepositoryTests()
        {
            _dialogManagementContextFactory = new DialogManagementContextFactory();
        }

        [Fact]
        public async Task AddProjectToOrganisation()
        {
            // Arrange
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[]{});
            var sut = new OrganisationRepository(context);
            var organisation = new Organisation(Guid.NewGuid().ToString(),
                "test", null);
            var project = new Project("test project", organisation.Id, 
                "widget title", Defaults.WidgetColor, "widget description",
                "fallback message", "greeting message", new string[]{},
                ApiKey.Empty, null, Defaults.BusinessTimezone, Defaults.BusinessTimeStartUtc,
                Defaults.BusinessTimeEndUtc, null);
            organisation.AddProject(project);
            
            // Act
            await sut.AddOrganisation(organisation);
            await context.SaveChangesAsync();
            
            // Assert
            var actualOrganisation = await context.Organisations.AsNoTracking()
                .Include(o => o.Projects)
                .FirstAsync(o => o.Id == organisation.Id);
            
            // clean up
            context.Projects.Remove(project);
            context.Organisations.Remove(organisation);
            await context.SaveChangesAsync();
            
            Equal(1, actualOrganisation.Projects.Count);
            Equal(project.Id, actualOrganisation.Projects[0].Id);
        }

        [Fact]
        public async Task GetOrganisationsByIds()
        {
            // Arrange
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[]{});
            var sut = new OrganisationRepository(context);
            var organisation = new Organisation(Guid.NewGuid().ToString(),
                "test", null);
            var project = new Project("test project", organisation.Id, 
                "widget title", Defaults.WidgetColor, "widget description",
                "fallback message", "greeting message", new string[]{},
                ApiKey.Empty, null, Defaults.BusinessTimezone, Defaults.BusinessTimeStartUtc,
                Defaults.BusinessTimeEndUtc, null);
            organisation.AddProject(project);
            
            // Act
            await context.AddAsync(organisation);
            await context.SaveChangesAsync();
            
            // Assert
            var actualOrganisation = await sut.GetOrganisationsByIds(new[]{organisation.Id});
            
            // clean up
            context.Projects.Remove(project);
            context.Organisations.Remove(organisation);
            await context.SaveChangesAsync();
            
            NotNull(actualOrganisation);
            Single(actualOrganisation!);
            Equal(1, actualOrganisation[0].Projects.Count);
            Equal(project.Id, actualOrganisation[0].Projects[0].Id); 
        }
        

        [Fact]
        public async Task SaveOrganisation()
        {
            // Arrange
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[]{});
            var sut = new OrganisationRepository(context);
            var organisation = new Organisation(Guid.NewGuid().ToString(),
                "test", new[] {"tag1", "tag2"});

            // Act
            await sut.AddOrganisation(organisation);
            await context.SaveChangesAsync();
            var actual = await context.Organisations.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == organisation.Id);

            // Assert
            
            // clean up
            context.Organisations.Remove(organisation);
            await context.SaveChangesAsync();
            
            NotNull(actual);
            Equal(organisation.Name, actual!.Name);
        }
    }
}
