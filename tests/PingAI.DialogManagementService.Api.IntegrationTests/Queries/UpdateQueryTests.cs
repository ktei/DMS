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
    public class UpdateQueryTests : ApiTestBase
    {
        public UpdateQueryTests(TestWebApplicationFactory factory, SharedDatabaseFixture fixture) : base(factory,
            fixture)
        {
        }

        [Fact]
        public async Task UpdateQuery()
        {
            var context = Fixture.CreateContext();
            var project = await context.Projects.FirstAsync();
            var query = await context.Queries.FirstAsync(x => x.ProjectId == project.Id);
            var client = Factory.CreateUserAuthenticatedClient();
            var request = BuildRequest();

            var httpResponse = await client.PutAsJsonAsync(
                $"/dms/api/v1.1/queries/{query.Id}", request
            );

            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedQuery = await httpResponse.Content.ReadFromJsonAsync<QueryDto>();
            updatedQuery.Should().NotBeNull();
            updatedQuery!.Intents.Should().HaveCount(1);
            updatedQuery.Intents.First().PhraseParts.Should().HaveCount(4);
            updatedQuery.Responses.Should().HaveCount(2);
        }

        private static UpdateQueryDto BuildRequest()
        {
            return new UpdateQueryDto
            {
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
                            },
                            new CreatePhrasePartDto
                            {
                                Type = PhrasePartType.TEXT,
                                Text = "World!"
                            }
                        },
                        new[]
                        {
                            new CreatePhrasePartDto
                            {
                                Type = PhrasePartType.TEXT,
                                Text = "Goodbye, "
                            },
                            new CreatePhrasePartDto
                            {
                                Type = PhrasePartType.TEXT,
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
