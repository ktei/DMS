using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Intents
{
    public static class PhraseParser
    {
        private static readonly Regex RegexExp =
            new Regex(@"\[([\w\s@.+\-,]+)\]\{([a-zA-Z0-9_.-]+)(@([a-zA-Z0-9_.-]+))?\}");
        public static CreatePhrasePartDto[] ConvertToParts(string phrase)
        {
            var results = new List<CreatePhrasePartDto>();
            var index = 0;
            var matches = RegexExp.Matches(phrase);
            var nextMatch = matches.FirstOrDefault();
            while (nextMatch?.Success == true)
            {
                if (nextMatch.Index != 0)
                {
                    results.Add(new CreatePhrasePartDto
                    {
                        Text = phrase.Substring(index, nextMatch.Index - index),
                        Type = PhrasePartType.TEXT
                    });
                }
                results.Add(new CreatePhrasePartDto
                {
                    Text = nextMatch.Groups[1].Value,
                    EntityName = nextMatch.Groups[2].Value,
                    EntityType = string.IsNullOrEmpty(nextMatch.Groups[4].Value) ?
                        nextMatch.Groups[2].Value : nextMatch.Groups[4].Value,
                    Type = PhrasePartType.ENTITY
                });
                index = nextMatch.Index + nextMatch.Groups[0].Value.Length;
                nextMatch = nextMatch.NextMatch();
            }
            
            var remaining = phrase.Substring(index, phrase.Length - index);
            if (!string.IsNullOrEmpty(remaining))
            {
                results.Add(new CreatePhrasePartDto
                {
                    Text = remaining,
                    Type = PhrasePartType.TEXT
                });
            }

            return results.ToArray();
        }
    }
}
