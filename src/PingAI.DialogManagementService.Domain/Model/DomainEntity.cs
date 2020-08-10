using System.Collections.Generic;
using System.Collections.Immutable;
using MediatR;

namespace PingAI.DialogManagementService.Domain.Model
{
    public abstract class DomainEntity
    {
        private List<INotification>? _domainEvents;
        public IReadOnlyList<INotification> DomainEvents => 
            (_domainEvents ?? new List<INotification>()).ToImmutableList();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents ??= new List<INotification>();
            _domainEvents.Add(eventItem);
        }
        
        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }
    }
}