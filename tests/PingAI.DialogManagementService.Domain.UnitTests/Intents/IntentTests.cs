using System;
using PingAI.DialogManagementService.Domain.Model;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Domain.UnitTests.Intents
{
    public class IntentTests
    {
        [Fact]
        public void UpdatePhraseParts()
        {
            // Arrange
            var sut = new Intent(Guid.NewGuid(), "test", Guid.NewGuid(), IntentType.STANDARD);
            sut.UpdatePhrases(new[]
            {
                new PhrasePart(Guid.NewGuid(), Guid.NewGuid(),
                    Guid.NewGuid(), 0, "test", 
                    null, PhrasePartType.TEXT, null, null),
            });
            
            // Act
            sut.UpdatePhrases(new[] { new PhrasePart(Guid.NewGuid(),
                sut.Id, Guid.NewGuid(), 0, "Hello, World!", null, PhrasePartType.TEXT,
                null, null)});
            var actual = sut.PhraseParts;

            // Assert
            Single(actual);
            Equal("Hello, World!", actual[0].Text);
            Equal(PhrasePartType.TEXT, actual[0].Type);
            Null(actual[0].Value);
            Null(actual[0].EntityNameId);
            Null(actual[0].EntityTypeId);
        }
    }
}