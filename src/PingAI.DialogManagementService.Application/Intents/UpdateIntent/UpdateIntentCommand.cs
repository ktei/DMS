using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Intents.UpdateIntent
{
    public class UpdateIntentCommand : IRequest<Intent>
    {
        public Guid IntentId { get; set; }
        public string Name { get; set; }
        public List<PhrasePart>? PhraseParts { get; set; }

        public UpdateIntentCommand(Guid intentId, string name, IEnumerable<PhrasePart>? phraseParts)
        {
            IntentId = intentId;
            Name = name;
            PhraseParts = phraseParts?.ToList();
        }

        public UpdateIntentCommand()
        {
            
        }
    }
}