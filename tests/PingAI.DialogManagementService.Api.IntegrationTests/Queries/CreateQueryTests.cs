using System;
using System.Linq;
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
    public class CreateQueryTests : ApiTestBase
    {
        public CreateQueryTests(TestWebApplicationFactory factory, SharedDatabaseFixture fixture) : base(factory,
            fixture)
        {
        }

        [Fact]
        public async Task CreateQuery()
        {
            var context = Fixture.CreateContext();
            var project = await context.Projects.FirstAsync();
            var request = BuildRequest(project.Id);
            var client = Factory.CreateUserAuthenticatedClient();

            var httpResponse = await client.PostAsJsonAsync(
                "/dms/api/v1.1/queries", request
            );

            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var createdQuery = await httpResponse.Content.ReadFromJsonAsync<QueryDto>();
            createdQuery.Should().NotBeNull();
            createdQuery!.Intents.Should().HaveCount(1);
            createdQuery.Intents.First().PhraseParts.Should().HaveCount(4);
            createdQuery.Responses.Should().HaveCount(2);
        }

        private static CreateQueryDto BuildRequest(Guid projectId)
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
                                Type = PhrasePartType.TEXT.ToString(),
                                Text = "Hello, "
                            },
                            new CreatePhrasePartDto
                            {
                                Type = PhrasePartType.TEXT.ToString(),
                                Text = "World!"
                            }
                        },
                        new[]
                        {
                            new CreatePhrasePartDto
                            {
                                Type = PhrasePartType.TEXT.ToString(),
                                Text = "Goodbye, "
                            },
                            new CreatePhrasePartDto
                            {
                                Type = PhrasePartType.TEXT.ToString(),
                                Text = "World!"
                            }
                        }
                    }
                },
                Responses = new[]
                {
                    new CreateResponseDto
                    {
                        Type = ResponseType.RTE.ToString(),
                        RteText = "Yes",
                    },
                    new CreateResponseDto
                    {
                        Type = ResponseType.FORM.ToString(),
                        Form = new CreateResponseFormDto
                        {
                            Fields = new[]
                            {
                                new CreateResponseFormDto.Field
                                {
                                    DisplayName = "F1",
                                    Name = "f1"
                                },
                                new CreateResponseFormDto.Field
                                {
                                    DisplayName = "F2",
                                    Name = "f2"
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
