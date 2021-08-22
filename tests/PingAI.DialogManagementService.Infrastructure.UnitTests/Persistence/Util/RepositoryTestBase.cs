using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Util
{
    [Collection(RepositoryTestCollection.Name)]
    public abstract class RepositoryTestBase
    {
        protected RepositoryTestBase(SharedDatabaseFixture fixture) => Fixture = fixture;

        protected SharedDatabaseFixture Fixture { get; }
    }
}
