using System;
using FluentAssertions;
using PingAI.DialogManagementService.Application.Queries.Shared;
using Xunit;
using PhrasePart = PingAI.DialogManagementService.Application.Queries.Shared.PhrasePart;

namespace PingAI.DialogManagementService.Application.UnitTests.Queries
{
    public class PhraseParserTests
    {
        [Fact]
        public void ConvertStringPhraseToParts()
        {
            const string phrase =
                "I want to book a flight from [Sydney]{departure_location} to " +
                "[Melbourne]{arrival_location} for [John Smith]{name}";
            var phraseId = Guid.NewGuid();
            var expected = new[]
            {
                PhrasePart.CreateText(phraseId,
                    0, "I want to book a flight from "),
                PhrasePart.CreateEntity(phraseId,
                    1, "Sydney", "departure_location"),
                PhrasePart.CreateText(phraseId, 2, " to "),
                PhrasePart.CreateEntity(phraseId, 3,
                    "Melbourne", "arrival_location"),
                PhrasePart.CreateText(phraseId, 4, " for "),
                PhrasePart.CreateEntity(phraseId, 5, "John Smith",
                    "name")
            };

            var actual = PhraseParser.ConvertToParts(phraseId, phrase);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
