using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Repositories
{
    public class UserRepositoryTests : RepositoryTestBase
    {
        public UserRepositoryTests(SharedDatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task GetUserByAuth0Id()
        {
            var context = Fixture.CreateContext();
            var sut = new UserRepository(context);
            var user = await context.Users.Include(x => x.Organisations).FirstAsync();
            
            context.ChangeTracker.Clear();
            var actual = await sut.GetUserByAuth0Id(user.Auth0Id);
            
            actual.Should().NotBeNull();
            actual!.Organisations.Should().HaveCount(user.Organisations.Count);
        }
    }
}
