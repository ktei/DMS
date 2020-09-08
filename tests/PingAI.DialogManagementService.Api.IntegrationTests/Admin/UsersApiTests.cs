using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models.Users;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Admin
{
    public class UsersApiTests : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private TestingFixture _testingFixture;
        private readonly TestWebApplicationFactory _factory;
        private Func<Task> _tearDownTestingFixture;

        public UsersApiTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateAdminAuthenticatedClient();
        }

        [Fact]
        public async Task ListUsers()
        {
            var response = await _client.GetFromJsonAsync<List<UserListItemDto>>(
                "/dms/api/admin/v1/users");
            True(response.Count > 1);
            Contains(response, u => 
                string.CompareOrdinal(u.UserId, _testingFixture.UserId.ToString()) == 0);
        }

        [Fact]
        public async Task CreateUser()
        {
            var userName = Guid.NewGuid().ToString();
            var response = await _client.PostAsJsonAsync("/dms/api/admin/v1/users", new CreateUserRequest
            {
                Name = userName,
                Auth0Id = Guid.NewGuid().ToString()
            });
            await response.IsOk();
            var userCreated = await response.Content.ReadFromJsonAsync<UserDto>();
            
            Equal(userName, userCreated.Name);
            
            // cleanup
            await _factory.WithDbContext(async context =>
            {
                var user = await context.Users
                    .FirstAsync(x => x.Id == Guid.Parse(userCreated.UserId));
                context.Remove(user);
                await context.SaveChangesAsync();
            });
        }

        public async Task InitializeAsync()
        {
            (_testingFixture, _tearDownTestingFixture) = await _factory.SetupTestingFixture();
        }

        public async Task DisposeAsync()
        {
            await _tearDownTestingFixture.Invoke();
        }
    }
}