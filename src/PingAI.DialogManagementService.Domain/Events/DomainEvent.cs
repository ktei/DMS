using System;
using MediatR;

namespace PingAI.DialogManagementService.Domain.Events
{
    public abstract class DomainEvent : INotification
    {
        public DateTime TimestampUtc { get; }

        protected DomainEvent()
        {
            TimestampUtc = DateTime.UtcNow;
        }
    }
}