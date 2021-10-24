using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Api.IntegrationTests.Utils;
using PingAI.DialogManagementService.Api.Models.Intents;
using PingAI.DialogManagementService.Api.Models.Queries;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Api.IntegrationTests.Queries
{
    public class WebhookTests : ApiTestBase
    {
        public WebhookTests(TestWebApplicationFactory factory, SharedDatabaseFixture fixture) : base(factory,
            fixture)
        {
        }

        [Fact]
        public async Task CreateQueryWithWebhookAndRetrieveIt()
        {
            var context = Fixture.CreateContext();
            var project = await context.Projects.FirstAsync();
            var entityName = new EntityName(project.Id, $"n{DateTime.UtcNow.Ticks}", true);
            var webhook = new WebhookResolution(entityName.Id,
                entityName.Name,
                "GET", "https://test.com", new WebhookHeader[0]);
            var webhookResponse = new Response(
                project.Id, Resolution.Factory.Webhook(webhook), ResponseType.WEBHOOK, 0);
            await context.AddAsync(webhookResponse);
            await context.SaveChangesAsync();
            var request = BuildRequest(project.Id, webhookResponse.Id);
            var client = Factory.CreateUserAuthenticatedClient();

            var createResponse = await client.PostAsJsonAsync(
                "/dms/api/v1.1/queries", request
            );
            
            createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var createdQuery = await createResponse.Content.ReadFromJsonAsync<QueryDto>();
            var queryResponse = await client.GetAsync(
                $"/dms/api/v1/queries/{createdQuery!.QueryId}");
            queryResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var actual = await queryResponse.Content.ReadFromJsonAsync<QueryDto>();
            actual!.Responses.Should().HaveCount(2);
            actual.Responses.Should().Contain(x => x.Type == ResponseType.WEBHOOK.ToString() &&
                                                   x.Resolution.Webhook!.ResponseId == webhookResponse.Id.ToString());
        }
        
        private static CreateQueryDto BuildRequest(Guid projectId,
            Guid webhookResponseId)
        {
            return new CreateQueryDto
            {
                ProjectId = projectId.ToString(),
                Name = Guid.NewGuid().ToString(),
                DisplayOrder = 0,
                Description = "description",
                Intent = new CreateIntentDto
                {
                    Name = $"Intent {DateTime.UtcNow.Ticks}",
                    PhraseParts = new[]
                    {
                        new[]
                        {
                            new CreatePhrasePartDto
                            {
                                Type = PhrasePartType.TEXT,
                                Text = "Hello, "
                            }
                        }
                    }
                },
                Responses = new[]
                {
                    new CreateResponseDto
                    {
                        Type = ResponseType.WEBHOOK.ToString(),
                        Webhook = new CreateWebhookDto
                        {
                            ResponseId = webhookResponseId
                        },
                        Order = 0,
                    },
                    new CreateResponseDto
                    {
                        Type = ResponseType.RTE.ToString(),
                        RteText = "Yes",
                        Order = 1
                    }
                }
            };
        }
    }
}