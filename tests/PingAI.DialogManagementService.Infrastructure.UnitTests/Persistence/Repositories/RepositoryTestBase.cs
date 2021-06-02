using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Repositories
{
    public abstract class RepositoryTestBase : IClassFixture<SharedDatabaseFixture>
    {
        protected RepositoryTestBase(SharedDatabaseFixture fixture) => Fixture = fixture;

        protected SharedDatabaseFixture Fixture { get; }
    }
}
