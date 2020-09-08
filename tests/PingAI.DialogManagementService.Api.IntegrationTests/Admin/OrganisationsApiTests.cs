using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models.Organisations;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Admin
{
    public class OrganisationsApiTests : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private TestingFixture _testingFixture;
        private readonly TestWebApplicationFactory _factory;
        private Func<Task> _tearDownTestingFixture;

        public OrganisationsApiTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateAdminAuthenticatedClient();
        }

        [Fact]
        public async Task ListOrganisations()
        {
            var response = await _client.GetFromJsonAsync<List<OrganisationListItemDto>>(
                "/dms/api/admin/v1/organisations");
            True(response.Count > 1);
        }
        
        [Fact]
        public async Task CreateOrganisation()
        {
            var response = await _client.PostAsJsonAsync(
                $"/dms/api/admin/v1/organisations",
                new CreateOrganisationRequest
                {
                    AdminUserId = _testingFixture.UserId,
                    Name = Guid.NewGuid().ToString()
                });
            await response.IsOk();

            var organisationCreated = await response.Content.ReadFromJsonAsync<OrganisationDto>();

            // cleanup
            await _factory.WithDbContext(async context =>
            {
                var organisation = await context.Organisations
                    .FirstAsync(x => x.Id == Guid.Parse(organisationCreated.OrganisationId));
                context.Remove(organisation);
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