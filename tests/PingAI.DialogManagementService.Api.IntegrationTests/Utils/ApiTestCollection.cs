using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Utils
{
    [CollectionDefinition(Name)]
    public class ApiTestCollection : ICollectionFixture<TestWebApplicationFactory>,
        ICollectionFixture<SharedDatabaseFixture>
    {
        public const string Name = "ApiTest";
    }
}