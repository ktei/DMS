using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models.Organisations;
using PingAI.DialogManagementService.Domain.Model;
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
        public async Task ListOrganisationsWithUserId()
        {
            var response = await _client.GetFromJsonAsync<List<OrganisationListItemDto>>(
                "/dms/api/admin/v1/organisations?userId=3ec1b42a-aada-4487-8ac1-ee2c5ef4cc7f");
            True(response.Count > 0);
        }
        
        [Fact]
        public async Task CreateOrganisation()
        {
            var response = await _client.PostAsJsonAsync(
                $"/dms/api/admin/v1/organisations",
                new CreateOrganisationRequest
                {
                    Auth0UserId = _testingFixture.Auth0UserId,
                    Name = Guid.NewGuid().ToString()
                });
            await response.IsOk();

            var organisationCreated = await response.Content.ReadFromJsonAsync<OrganisationDto>();
            await _factory.WithDbContext(async context =>
            {
                var defaultProject = await context.Projects
                    .Include(p => p.EntityNames)
                    .FirstOrDefaultAsync(p => p.OrganisationId == Guid.Parse(organisationCreated.OrganisationId));
                // assert a default project exists for this new organisation
                NotNull(defaultProject);
                Equal(Defaults.EnquiryEntityNames.Length, defaultProject.EntityNames.Count);
            });

            // cleanup
            await _factory.WithDbContext(async context =>
            {
                var organisation = await context.Organisations
                    .Include(o => o.Projects)
                    .ThenInclude(p => p.EntityNames)
                    .FirstAsync(x => x.Id == Guid.Parse(organisationCreated.OrganisationId));
                context.RemoveRange(organisation.Projects.SelectMany(p => p.EntityNames));
                context.RemoveRange(organisation.Projects);
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