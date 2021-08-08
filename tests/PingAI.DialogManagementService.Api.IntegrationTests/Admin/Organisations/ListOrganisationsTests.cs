using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models.Organisations;
using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Admin.Organisations
{
    public class ListOrganisationsTests : ApiTestBase
    {
        public ListOrganisationsTests(TestWebApplicationFactory factory, SharedDatabaseFixture fixture) : base(factory,
            fixture)
        {
        }

        [Fact]
        public async Task ListAll()
        {
            var client = Factory.CreateAdminUserAuthenticatedClient();
            
            var response = await client.GetFromJsonAsync<List<OrganisationListItemDto>>(
                "/dms/api/admin/v1/organisations");
            
            response.Should().NotBeNull();
            response.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ListByAuthUser0Id()
        {
            var context = Fixture.CreateContext();
            var user = await context.Users.FirstAsync();
            var client = Factory.CreateAdminUserAuthenticatedClient();

            var response = await client.GetFromJsonAsync<List<OrganisationListItemDto>>(
                $"/dms/api/admin/v1/organisations?auth0UserId={user.Auth0Id}");

            response.Should().NotBeNull();
            response.Should().NotBeEmpty();
        }
    }
}
