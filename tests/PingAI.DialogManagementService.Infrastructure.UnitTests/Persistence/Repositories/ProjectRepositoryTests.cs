using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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
        public async Task GetProjectsByIds()
        {
            // Arrange
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[] { });
            var organisation =
                new Organisation(Guid.NewGuid().ToString(), "test", null);
            var project = new Project( "test", organisation.Id,  "title", Defaults.WidgetColor,
                "description", "fallback message", "greeting message", new string[] { },
                ApiKey.Empty, null, Defaults.BusinessTimezone, Defaults.BusinessTimeStartUtc,
                Defaults.BusinessTimeEndUtc, null);
            organisation.AddProject(project);
            await context.AddAsync(organisation);
            await context.SaveChangesAsync();
            var sut = new ProjectRepository(context); 
            
            // Act
            var actual = await sut.GetProjectsByIds(new []{project.Id});

            // Assert

            // clean up
            context.Remove(organisation);
            await context.SaveChangesAsync();

            Single(actual);
        }

        [Fact]
        public async Task GetProjectById()
        {
            // Arrange
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[] { });
            var organisation =
                new Organisation(Guid.NewGuid().ToString(), "test", null);
            var project = new Project("test", organisation.Id, "title", Defaults.WidgetColor,
                "description", "fallback message",
                "greeting message", new string[] { }, ApiKey.Empty, null, Defaults.BusinessTimezone,
                Defaults.BusinessTimeStartUtc,
                Defaults.BusinessTimeEndUtc, null);
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
                new Organisation(Guid.NewGuid().ToString(), "test", null);
            await context.AddAsync(organisation);
            await context.SaveChangesAsync();
            var sut = new ProjectRepository(context);
            var project = new Project(Guid.NewGuid().ToString(), organisation.Id, 
                null, Defaults.WidgetColor,
                null, null, null, null,
                ApiKey.Empty, null, Defaults.BusinessTimezone, Defaults.BusinessTimeStartUtc,
                Defaults.BusinessTimeEndUtc, null);

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

        [Fact]
        public async Task ExportProject()
        {
            // Arrange
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[] { });
            var organisation =
                new Organisation(RandomString(10), "test", null);
            var project = new Project(RandomString(10), organisation.Id,
                null, Defaults.WidgetColor,
                null, null, null, null,
                ApiKey.Empty, new[] {"https://google.com"}, Defaults.BusinessTimezone, Defaults.BusinessTimeStartUtc,
                Defaults.BusinessTimeEndUtc, null);
            var entityType = new EntityType(
                RandomString(10), project.Id, RandomString(15), new []{"t1"},
                new []{new EntityValue("v", Guid.Empty, default)});
            var entityName = new EntityName(RandomString(10), project.Id, true);
            project.AddEntityType(entityType);
            project.AddEntityName(entityName);
            var intent = new Intent(RandomString(15), project.Id, IntentType.STANDARD,
                new[]
                {
                    new PhrasePart(Guid.Empty, Guid.NewGuid(), 0, 
                        "Hello, World", null,
                        PhrasePartType.TEXT, default(Guid?), default),
                    new PhrasePart(Guid.Empty, Guid.NewGuid(), 0, "test", null,
                        PhrasePartType.ENTITY, entityName, entityType)
                });
            var response = new Response(new Resolution(new[]
            {
                new ResolutionPart("Test response", null, ResolutionPartType.RTE), 
            }), project.Id, ResponseType.RTE, 0);
            project.AddIntent(intent);
            project.AddResponse(response);
            var query = new Query(RandomString(10), project.Id, new Expression[0],
                RandomString(15), new []{"t1"}, 0);
            query.AddIntent(intent);
            query.AddResponse(response);
            project.AddQuery(query);
            organisation.AddProject(project);
            await context.AddAsync(organisation);
            await context.SaveChangesAsync();
            var sut = new ProjectRepository(context);
            
            // Act
            var exportedProject = project.Export();
            exportedProject = await sut.AddProject(exportedProject);
            await context.SaveChangesAsync();

            // Assert
            var actual = await context.Projects.AsNoTracking()
                .Include(p => p.EntityNames)
                .Include(p => p.EntityTypes).ThenInclude(x => x.Values)
                .Include(p => p.Intents)
                .ThenInclude(i => i.PhraseParts)
                .Include(p => p.Responses)
                .Include(p => p.Queries)
                .ThenInclude(q => q.QueryIntents)
                .Include(p => p.Queries)
                .ThenInclude(q => q.QueryResponses)
                
                .SingleAsync(x => x.Id == exportedProject.Id);

            try
            {
                actual.EntityNames.Should().HaveSameCount(exportedProject.EntityNames);
                actual.EntityTypes.Should().HaveSameCount(exportedProject.EntityTypes);
                actual.Queries.Should().HaveSameCount(exportedProject.Queries);
                actual.Responses.Should().HaveSameCount(exportedProject.Responses);
                actual.Intents.Should().HaveSameCount(exportedProject.Intents);
            }
            finally
            {
                // clean up
                context.RemoveRange(entityType.Values);
                context.RemoveRange(entityName, entityType, intent, response,
                    query, project);
                context.RemoveRange(exportedProject.EntityNames);
                context.RemoveRange(exportedProject.EntityTypes);
                context.RemoveRange(exportedProject.Queries);
                context.RemoveRange(exportedProject.Intents);
                context.RemoveRange(exportedProject.Responses);
                context.RemoveRange(project, exportedProject);
                context.Remove(organisation);
                await context.SaveChangesAsync();
            }
        }
        
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        private static readonly Random Random = new Random();
    }
}