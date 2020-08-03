using System;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Repositories
{
    public class UserRepositoryTests
    {
        private readonly DialogManagementContextFactory _dialogManagementContextFactory;
                
        public UserRepositoryTests()
        {
            _dialogManagementContextFactory = new DialogManagementContextFactory();
        }

        [Fact]
        public async Task GetUserByAuth0Id()
        {
            // Arrange
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[]{});
            var sut = new UserRepository(context);
            var organisation = new Organisation(Guid.NewGuid(), Guid.NewGuid().ToString(),
                "test", null);
            var user = new User(Guid.NewGuid(), "myuser", Guid.NewGuid().ToString());
            var orgUser = new OrganisationUser(Guid.NewGuid(), organisation.Id, user.Id);
            await context.AddAsync(organisation);
            await context.AddAsync(user);
            await context.AddAsync(orgUser);
            await context.SaveChangesAsync();

            // Act
            var actual = await sut.GetUserByAut0Id(user.Auth0Id);

            // Assert
            context.Remove(orgUser);
            context.Remove(user);
            context.Remove(organisation);
            await context.SaveChangesAsync();
            NotNull(actual);
            Equal(user.Id, actual!.Id);
            Equal(user.Auth0Id, actual.Auth0Id);
        }
    }
}