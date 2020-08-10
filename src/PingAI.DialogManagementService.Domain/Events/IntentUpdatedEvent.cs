using System;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Domain.Events
{
    public class IntentUpdatedEvent : INotification
    {
        public Guid IntentId { get; }
        public Guid ProjectId { get; }
        public string IntentName { get; }
        public PhrasePart[] PhraseParts { get; }

        public IntentUpdatedEvent(Guid intentId, Guid projectId, string intentName, PhrasePart[] phraseParts)
        {
            IntentId = intentId;
            ProjectId = projectId;
            IntentName = intentName;
            PhraseParts = phraseParts;
        }
    }
}