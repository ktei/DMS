using System;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
using PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Helpers;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Repositories
{
    public class UserRepositoryTests : IAsyncLifetime
    {
        private readonly DialogManagementContextFactory _dialogManagementContextFactory;
        private readonly TestDataFactory _testDataFactory;
                
        public UserRepositoryTests()
        {
            _dialogManagementContextFactory = new DialogManagementContextFactory();
            _testDataFactory = new TestDataFactory(_dialogManagementContextFactory.CreateDbContext(new string[] { }));
        }

        [Fact]
        public async Task GetUserByAuth0Id()
        {
            // Arrange
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[]{});
            var sut = new UserRepository(context);
            var organisation = _testDataFactory.Organisation;
            context.Attach(organisation);
            var user = new User("myuser", Guid.NewGuid().ToString());
            organisation.AddUser(user);
            await context.SaveChangesAsync();

            // Act
            var actual = await sut.GetUserByAut0Id(user.Auth0Id);

            // Assert
            
            // clean up
            organisation.RemoveUser(user);
            context.Remove(user);
            await context.SaveChangesAsync();
            
            NotNull(actual);
            Equal(user.Id, actual!.Id);
            Equal(user.Auth0Id, actual.Auth0Id);
        }

        public Task InitializeAsync() => _testDataFactory.Setup();

        public Task DisposeAsync() => _testDataFactory.Cleanup();
    }
}