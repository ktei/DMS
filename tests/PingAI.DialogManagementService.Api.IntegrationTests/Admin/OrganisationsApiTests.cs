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
            _client = _factory.CreateAdminUserAuthenticatedClient();
        }

        [Fact]
        public async Task ListOrganisations()
        {
            var response = await _client.GetFromJsonAsync<List<OrganisationListItemDto>>(
                "/dms/api/admin/v1/organisations");
            True(response.Count > 1);
        }
        
        [Fact]
        public async Task ListOrganisationsWithAuth0UserId()
        {
            var response = await _client.GetFromJsonAsync<List<OrganisationListItemDto>>(
                "/dms/api/admin/v1/organisations?auth0UserId=auth0|5ea7f465a370110bd9c6e838");
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
                var projectVersion =
                    await context.ProjectVersions.FirstOrDefaultAsync(v => v.ProjectId == defaultProject.Id);
                // assert a default project exists for this new organisation
                NotNull(defaultProject);
                NotNull(projectVersion);
                Equal(Defaults.EnquiryEntityNames.Length, defaultProject.EntityNames.Count);
            });

            // cleanup
            await _factory.WithDbContext(async context =>
            {
                var organisation = await context.Organisations
                    .Include(o => o.Projects)
                    .ThenInclude(p => p.EntityNames)
                    .FirstAsync(x => x.Id == Guid.Parse(organisationCreated.OrganisationId));
                var projectIds = organisation.Projects.Select(x => x.Id);
                var projectVersions =
                    await context.ProjectVersions.Where(v => projectIds.Contains(v.ProjectId)).ToListAsync();
                context.RemoveRange(projectVersions);
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