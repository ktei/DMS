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
    public class UpdateWebhookTests : ApiTestBase
    {
        public UpdateWebhookTests(TestWebApplicationFactory factory, SharedDatabaseFixture fixture) : base(factory,
            fixture)
        {
        }

        [Fact]
        public async Task UpdateWebhook()
        {
            var context = Fixture.CreateContext();
            var defaultProject = await context.Projects
                .Include(x => x.Organisation)
                .SingleAsync(x => x.Name == SharedDatabaseFixture.DefaultProjectName);
            var project = Project.CreateWithDefaults(defaultProject.OrganisationId,
                Guid.NewGuid().ToString());
            var entityName = new EntityName(project.Id,
                $"n{DateTime.UtcNow.Ticks}", true);
            var response = new Response(project.Id,
                Resolution.Factory.Webhook(new WebhookResolution(
                    entityName.Id, entityName.Name,
                    "GET", "https://test.com", new WebhookHeader[0])),
                ResponseType.WEBHOOK, 0);
            await context.AddRangeAsync(project, entityName, response);
            await context.SaveChangesAsync();
            
            var client = Factory.CreateUserAuthenticatedClient();
            var actual = await client.PutAsJsonAsync(
                $"/dms/api/v1/webhooks/{response.Id}", new UpdateWebhookRequest
                {
                    Method = "GET",
                    Url = "https://test2.com",
                    Headers = new List<KeyValuePair<string, string>>(0),
                });
            
            actual.StatusCode.Should().Be(HttpStatusCode.OK);
            var webhookUpdated = await actual.Content.ReadFromJsonAsync<WebhookDetails>();
            webhookUpdated!.Url.Should().Be("https://test2.com");
            context.ChangeTracker.Clear();
            var responseUpdated = await context.Responses.SingleOrDefaultAsync(x => x.Id == webhookUpdated.ResponseId);
            responseUpdated.Resolution.Webhook!.Url.Should().Be("https://test2.com");
        }
    }
}