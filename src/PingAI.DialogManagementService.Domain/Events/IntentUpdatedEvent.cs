using System;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Domain.Events
{
    public class IntentUpdatedEvent : DomainEvent 
    {
        public Intent Intent { get; }
        public Guid? DesignTimeProjectId { get; }


        public IntentUpdatedEvent(Intent intent, Guid? designTimeProjectId)
        {
            Intent = intent;
            DesignTimeProjectId = designTimeProjectId;
        }
    }
}