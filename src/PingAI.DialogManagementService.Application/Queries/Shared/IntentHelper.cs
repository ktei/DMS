using System.Collections.Generic;
using System.Linq;
using PingAI.DialogManagementService.Domain.Model;
using PhrasePart = PingAI.DialogManagementService.Application.Queries.Shared.PhrasePart;

namespace PingAI.DialogManagementService.Application.Queries.Shared
{
    internal static class IntentHelper
    {
        public static void AddPhrases(Intent intent, IEnumerable<PhrasePart> phraseParts,
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
