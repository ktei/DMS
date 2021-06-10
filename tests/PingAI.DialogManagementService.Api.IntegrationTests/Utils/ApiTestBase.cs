using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Utils
{
    public abstract class ApiTestBase : IClassFixture<TestWebApplicationFactory>,
        IClassFixture<SharedDatabaseFixture>
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
