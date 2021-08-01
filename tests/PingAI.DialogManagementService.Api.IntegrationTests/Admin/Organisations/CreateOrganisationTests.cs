using System;
using System.Net;
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
    public class CreateOrganisationTests : ApiTestBase
    {
        public CreateOrganisationTests(TestWebApplicationFactory factory, SharedDatabaseFixture fixture) : base(factory,
            fixture)
        {
        }

        [Fact]
        public async Task CreateOrganisation()
        {
            var context = Fixture.CreateContext();
            var user = await context.Users.FirstAsync();
            var client = Factory.CreateAdminUserAuthenticatedClient();
            var request = new CreateOrganisationRequest
            {
                Auth0UserId = user.Auth0Id,
                Name = Guid.NewGuid().ToString()
            };

            var httpResponse = await client.PostAsJsonAsync(
                $"/dms/api/admin/v1/organisations", request);

            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var actual = await httpResponse.Content.ReadFromJsonAsync<OrganisationDto>();
            actual!.Name.Should().Be(request.Name);
        }
    }
}