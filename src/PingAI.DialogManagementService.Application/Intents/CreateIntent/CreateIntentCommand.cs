using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Intents.CreateIntent
{
    public class CreateIntentCommand : IRequest<Intent>
    {
        public Guid IntentId { get; set; }
        public string Name { get; set; }
        public Guid ProjectId { get; set; }
        public IntentType IntentType { get; set; }
        public List<PhrasePart> PhraseParts { get; set; }

        public CreateIntentCommand(Guid intentId, string name, Guid projectId, 
            IntentType intentType, IEnumerable<PhrasePart>? phraseParts)
        {
            IntentId = intentId;
            Name = name;
            ProjectId = projectId;
            IntentType = intentType;
            PhraseParts = phraseParts?.ToList() ?? new List<PhrasePart>();
        }
    }
}