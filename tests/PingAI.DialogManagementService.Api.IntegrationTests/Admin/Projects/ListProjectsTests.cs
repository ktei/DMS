using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models.Projects;
using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Admin.Projects
{
    public class ListProjectsTests : ApiTestBase
    {
        public ListProjectsTests(TestWebApplicationFactory factory, SharedDatabaseFixture fixture) : base(factory,
            fixture)
        {
        }

        [Fact]
        public async Task ListAll()
        {
            var client = Factory.CreateAdminUserAuthenticatedClient();

            var response = await client.GetFromJsonAsync<List<ProjectListItemDto>>(
                $"/dms/api/admin/v1/projects?");

            response.Should().NotBeNull();
            response.Should().NotBeEmpty(); 
        }

        [Fact]
        public async Task ListByOrganisationId()
        {
            var context = Fixture.CreateContext();
            var organisation = await context.Organisations.FirstAsync();
            var client = Factory.CreateAdminUserAuthenticatedClient();

            var response = await client.GetFromJsonAsync<List<ProjectListItemDto>>(
                $"/dms/api/admin/v1/projects?organisationId={organisation.Id}");

            response.Should().NotBeNull();
            response.Should().NotBeEmpty();
            response!.All(p => Guid.Parse(p.Organisation.OrganisationId) == organisation.Id)
                .Should().BeTrue();
        }

        [Fact]
        public async Task ForbidNonAdminUser()
        {
            var context = Fixture.CreateContext();
            var organisation = await context.Organisations.FirstAsync();
            var client = Factory.CreateUserAuthenticatedClient();

            var response = await client.GetAsync(
                $"/dms/api/admin/v1/projects?organisationId={organisation.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AllowAdminPortalClient()
        {
            var context = Fixture.CreateContext();
            var organisation = await context.Organisations.FirstAsync();
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("AdminPortalClientId",
                        TestAdminClientAuthHandler.AdminPortalClientId)
                }).Build();
            var client = Factory.CreateAdminClientAuthenticatedClient(services =>
                services.AddSingleton<IConfiguration>(config));
            
            var response = await client.SendAsync(
                new HttpRequestMessage(HttpMethod.Get,
                    $"/dms/api/admin/v1/projects?organisationId={organisation.Id}"));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}