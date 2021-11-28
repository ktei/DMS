using System;
using System.Collections.Generic;
using System.Linq;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Queries.Shared
{
    internal static class IntentExtensions
    {
        public static void AddPhrases(this Intent intent, IEnumerable<string> phrases,
            IReadOnlyList<EntityName> entityNames)
        {
            var phraseParts = phrases.Select(x =>
            {
                var phraseId = Guid.NewGuid();
                return PhraseParser.ConvertToParts(phraseId, x);
            }).SelectMany(x => x);
            intent.AddPhrases(phraseParts, entityNames);
        }
        
        public static void AddPhrases(this Intent intent, IEnumerable<PhrasePart> phraseParts,
            IReadOnlyList<EntityName> entityNames)
        {
            var phraseDisplayOrder = 0;
            foreach (var groupedPhraseParts in phraseParts.GroupBy(p => p.PhraseId))
            {
                var phrase = new Phrase(phraseDisplayOrder++);
                foreach (var phrasePart in groupedPhraseParts)
                {
                    if (phrasePart.EntityName != null)
                    {
                        var existingEntityName = entityNames
                            .FirstOrDefault(e => e.Name == phrasePart.EntityName);
                        phrase.AppendEntity(phrasePart.Text,
                            existingEntityName ?? new EntityName(intent.ProjectId, phrasePart.EntityName, true));
                    }
                    else
                    {
                        phrase.AppendText(phrasePart.Text);
                    }
                }

                intent.AddPhrase(phrase);
            }
        }
    }
}
