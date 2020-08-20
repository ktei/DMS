using System;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Domain.Events
{
    public class IntentDeletedEvent : DomainEvent
    {
        public Intent Intent { get; }

        public IntentDeletedEvent(Intent intent)
        {
            Intent = intent;
        }
    }
}