using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models.Webhooks;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence;
using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Webhooks
{
    public class QueryWebhooksTests : ApiTestBase
    {
        public QueryWebhooksTests(TestWebApplicationFactory factory, SharedDatabaseFixture fixture) : base(factory,
            fixture)
        {
        }

        [Fact]
        public async Task ListWebhooks()
        {
            // Arrange
            var context = Fixture.CreateContext();
            var (project, _, __) = await SetupWebhooks(context);
            
            // Act
            var client = Factory.CreateUserAuthenticatedClient();
            var actual = await client.GetFromJsonAsync<WebhookListItem[]>(
                $"/dms/api/v1/webhooks?projectId={project.Id}"
            );

            // Assert
            actual.Should().NotBeNull();
            actual!.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetWebhook()
        {
            // Arrange
            var context = Fixture.CreateContext();
            var (_, response, __) = await SetupWebhooks(context);
            
            // Act
            var client = Factory.CreateUserAuthenticatedClient();
            var actual = await client.GetFromJsonAsync<WebhookDetails>(
                $"/dms/api/v1/webhooks/{response.Id}");
            
            // Assert
            actual.Should().NotBeNull();
            actual!.Name.Should().Be(response.Resolution.Webhook!.EntityName);
        }

        private async Task<(Project, Response, Response)> SetupWebhooks(DialogManagementContext context)
        {
            var organisation = await context.Organisations.FirstAsync();
            var project = Project.CreateWithDefaults(organisation.Id,
                Guid.NewGuid().ToString());
            var entityName1 = new EntityName(project.Id, $"n{DateTime.UtcNow.Ticks}", true);
            var entityName2 = new EntityName(project.Id, $"n{DateTime.UtcNow.Ticks + 1}", true);
            var webhook1 = new WebhookResolution(entityName1.Id,
                entityName1.Name,
                "GET", "https://test.com", new WebhookHeader[0]);
            var webhook2 = new WebhookResolution(entityName1.Id,
                entityName1.Name,
                "GET", "https://test.com", new WebhookHeader[0]);
            var response1 = new Response(project.Id,
                Resolution.Factory.Webhook(webhook1), ResponseType.WEBHOOK, 5);
            var response2 = new Response(project.Id,
                Resolution.Factory.Webhook(webhook2), ResponseType.WEBHOOK, 6);
            await context.AddRangeAsync(
                project,
                entityName1,
                entityName2,
                response1,
                response2);
            await context.SaveChangesAsync();

            return (project, response1, response2);
        }
    }
}