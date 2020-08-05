using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
using PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Helpers;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Repositories
{
    public class EntityTypeRepositoryTests : IAsyncLifetime
    {
        private readonly DialogManagementContextFactory _dialogManagementContextFactory;
        private readonly TestDataFactory _testDataFactory;
        
        public EntityTypeRepositoryTests()
        {
            _dialogManagementContextFactory = new DialogManagementContextFactory();
            _testDataFactory = new TestDataFactory(_dialogManagementContextFactory.CreateDbContext(new string[] { }));
        }

        [Fact]
        public async Task GetEntityTypesByProjectId()
        {
            // Arrange
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[]{});
            var entityType = new EntityType(Guid.NewGuid(), "test", _testDataFactory.Project.Id,
                "description", new []{"t1", "t2"});
            await context.AddAsync(entityType);
            await context.SaveChangesAsync();
            var sut = new EntityTypeRepository(context);

            // Act
            var actual = await sut.GetEntityTypesByProjectId(_testDataFactory.Project.Id);

            // Assert
            
            // clean up
            context.Remove(entityType);
            await context.SaveChangesAsync();

            Single(actual);
            Equal(entityType.Id, actual[0].Id);
        }

        [Fact]
        public async Task AddEntityTypeWithValues()
        {
            // Arrange
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[]{});
            var entityType = new EntityType(Guid.NewGuid(), "test", _testDataFactory.Project.Id,
                "description", new []{"t1", "t2"});
            entityType.UpdateValues(new[] {new EntityValue(Guid.NewGuid(), "Sydney", entityType.Id, new[] {"SYD"})});
            var sut = new EntityTypeRepository(context);
            await sut.AddEntityType(entityType);
            await context.SaveChangesAsync();
            
            // Act
            var actual = await context.EntityTypes.SingleAsync(x => x.Id == entityType.Id);
            
            // Assert
            context.EntityValues.RemoveRange(context.EntityValues.Where(e => e.EntityTypeId == entityType.Id));
            context.Remove(entityType);
            await context.SaveChangesAsync();

            Equal(entityType.Id, actual.Id);
        }
        
        public Task InitializeAsync() => _testDataFactory.Setup();

        public Task DisposeAsync() => _testDataFactory.Cleanup();
    }
}