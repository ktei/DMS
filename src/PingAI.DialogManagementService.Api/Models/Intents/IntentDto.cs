using System;
using System.Collections.Generic;
using System.Linq;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Intents
{
    public class IntentDto
    {
        public string IntentId { get; set; }
        public string Name { get; set; }
        public string ProjectId { get; set; }
        public string Type { get; set; }
        public PhrasePartDto[] PhraseParts { get; set; }

        public IntentDto(string intentId, string name, string projectId, string type,
            IEnumerable<PhrasePartDto> phraseParts)
        {
            IntentId = intentId;
            Name = name;
            ProjectId = projectId;
            Type = type;
            PhraseParts = (phraseParts ?? new PhrasePartDto[0]).ToArray();
        }

        public IntentDto(Intent intent) : this(intent.Id.ToString(),
            intent.Name, intent.ProjectId.ToString(), intent.Type.ToString(), 
            intent.PhraseParts.Select(x => new PhrasePartDto(x)))
        {
            
        }

        public IntentDto()
        {
            
        }
    }
}