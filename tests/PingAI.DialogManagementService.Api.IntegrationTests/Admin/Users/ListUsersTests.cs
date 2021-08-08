using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models.Users;
using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Admin.Users
{
    public class ListUsersTests : ApiTestBase
    {
        public ListUsersTests(TestWebApplicationFactory factory, SharedDatabaseFixture fixture) : base(factory, fixture)
        {
        }

        [Fact]
        public async Task ListAll()
        {
            var client = Factory.CreateAdminUserAuthenticatedClient();
            
            var response = await client.GetFromJsonAsync<List<UserListItemDto>>(
                "/dms/api/admin/v1/users");

            response.Should().NotBeNull();
            response.Should().NotBeEmpty();
        }
    }
}