using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PingAI.DialogManagementService.Application.Queries.Shared
{
    public static class PhraseParser
    {
        private static readonly Regex RegexExp =
            new Regex(@"\[([\w\s@.+\-,]+)\]\{([a-zA-Z0-9_.-]+)(@([a-zA-Z0-9_.-]+))?\}");
        public static PhrasePart[] ConvertToParts(Guid phraseId, string phrase)
        {
            var results = new List<PhrasePart>();
            var index = 0;
            var position = 0;
            var matches = RegexExp.Matches(phrase);
            var nextMatch = matches.FirstOrDefault();
            while (nextMatch?.Success == true)
            {
                if (nextMatch.Index != 0)
                {
                    results.Add(PhrasePart.CreateText(phraseId, position++,
                        phrase.Substring(index, nextMatch.Index - index)));
                }
                results.Add(PhrasePart.CreateEntity(phraseId,
                    position++,
                    nextMatch.Groups[1].Value, 
                    nextMatch.Groups[2].Value));
                index = nextMatch.Index + nextMatch.Groups[0].Value.Length;
                nextMatch = nextMatch.NextMatch();
            }
            
            var remaining = phrase.Substring(index, phrase.Length - index);
            if (!string.IsNullOrEmpty(remaining))
            {
                results.Add(PhrasePart.CreateText(phraseId, position++,
                    remaining));
            }

            return results.ToArray();
        }
    }
}
