using FluentAssertions;
using PingAI.DialogManagementService.Api.Models.Intents;
using PingAI.DialogManagementService.Domain.Model;
using Xunit;

namespace PingAI.DialogManagementService.Api.UnitTests.Models.Intents
{
    public class PhraseParserTests
    {
        [Fact]
        public void ConvertStringPhraseToParts()
        {
            const string phrase =
                "I want to book a flight from [Sydney]{departure_location@location} to " +
                "[Melbourne]{arrival_location@location} for [John Smith]{name}";
            var expected = new[]
            {
                new CreatePhrasePartDto
                {
                    Text = "I want to book a flight from ",
                    Type = PhrasePartType.TEXT
                },
                new CreatePhrasePartDto
                {
                    Text = "Sydney",
                    Type = PhrasePartType.ENTITY,
                    EntityName = "departure_location",
                    EntityType = "location"
                },
                new CreatePhrasePartDto
                {
                    Text = " to ",
                    Type = PhrasePartType.TEXT
                },
                new CreatePhrasePartDto
                {
                    Text = "Melbourne",
                    Type = PhrasePartType.ENTITY,
                    EntityName = "arrival_location",
                    EntityType = "location"
                },
                new CreatePhrasePartDto
                {
                    Text = " for ",
                    Type = PhrasePartType.TEXT
                },
                new CreatePhrasePartDto
                {
                    Text = "John Smith",
                    Type = PhrasePartType.ENTITY,
                    EntityName = "name",
                    EntityType = "name"
                }
            };

            var actual = PhraseParser.ConvertToParts(phrase);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}