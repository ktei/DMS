using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Util
{
    [CollectionDefinition(Name)]
    public class RepositoryTestCollection : ICollectionFixture<SharedDatabaseFixture>
    {
        public const string Name = "RepositoryTest";
    }
}