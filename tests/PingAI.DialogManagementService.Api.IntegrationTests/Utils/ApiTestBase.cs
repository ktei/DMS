using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Utils
{
    [Collection(ApiTestCollection.Name)]
    public abstract class ApiTestBase
    {
        protected ApiTestBase(TestWebApplicationFactory factory, SharedDatabaseFixture fixture)
        {
            Factory = factory;
            Fixture = fixture;
        }

        protected TestWebApplicationFactory Factory { get; }
        protected SharedDatabaseFixture Fixture { get; }
    }
}
