using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models.Webhooks;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Webhooks
{
    public class CreateWebhookTests : ApiTestBase
    {
        public CreateWebhookTests(TestWebApplicationFactory factory, SharedDatabaseFixture fixture) : base(factory,
            fixture)
        {
        }

        [Fact]
        public async Task CreateWebhook()
        {
            var context = Fixture.CreateContext();
            var defaultProject = await context.Projects
                .Include(x => x.Organisation)
                .SingleAsync(x => x.Name == SharedDatabaseFixture.DefaultProjectName);
            var project = Project.CreateWithDefaults(defaultProject.OrganisationId,
                Guid.NewGuid().ToString());
            await context.AddAsync(project);
            await context.SaveChangesAsync();
            var entityName = new EntityName(project.Id, 
                $"n{DateTime.UtcNow.Ticks}", true);

            var client = Factory.CreateUserAuthenticatedClient();
            var actual = await client.PostAsJsonAsync(
                $"/dms/api/v1/webhooks", new CreateWebhookRequest
                {
                    Method = "GET",
                    Url = "https://test.com",
                    Name = entityName.Name,
                    Headers = new List<KeyValuePair<string, string>>(0),
                    ProjectId = project.Id
                });

            actual.StatusCode.Should().Be(HttpStatusCode.OK);
            var webhookCreated = await actual.Content.ReadFromJsonAsync<WebhookDetails>();
            webhookCreated!.Name.Should().Be(entityName.Name);
            context.ChangeTracker.Clear();
            var responseCreated = await context.Responses.SingleOrDefaultAsync(x => x.Id == webhookCreated.ResponseId);
            responseCreated.Should().NotBeNull();
        }
    }
}