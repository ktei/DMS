using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models.Users;
using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Admin.Users
{
    public class CreateUserTests : ApiTestBase
    {
        public CreateUserTests(TestWebApplicationFactory factory, SharedDatabaseFixture fixture) : base(factory,
            fixture)
        {
        }

        [Fact]
        public async Task CreateUser()
        {
            var context = Fixture.CreateContext();
            var client = Factory.CreateAdminUserAuthenticatedClient();
            var userName = Guid.NewGuid().ToString();
            
            var response = await client.PostAsJsonAsync("/dms/api/admin/v1/users", new CreateUserRequest
            {
                Name = userName,
                Auth0Id = Guid.NewGuid().ToString()
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var userSaved = await context.Users.FirstAsync(x => x.Name == userName);
            userSaved.Should().NotBeNull();
        }
    }
}