using System;
using MediatR;

namespace PingAI.DialogManagementService.Domain.Events
{
    public class IntentDeletedEvent : INotification
    {
        public Guid ProjectId { get; }
        public Guid IntentId { get; }

        public IntentDeletedEvent(Guid projectId, Guid intentId)
        {
            ProjectId = projectId;
            IntentId = intentId;
        }
    }
}