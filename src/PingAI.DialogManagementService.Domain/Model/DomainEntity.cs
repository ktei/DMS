using System.Collections.Generic;
using System.Collections.Immutable;
using MediatR;
using PingAI.DialogManagementService.Domain.Events;

namespace PingAI.DialogManagementService.Domain.Model
{
    public abstract class DomainEntity
    {
        private List<DomainEvent>? _domainEvents;
        public IReadOnlyList<DomainEvent> DomainEvents => 
            (_domainEvents ?? new List<DomainEvent>()).ToImmutableList();

        public void AddDomainEvent(DomainEvent eventItem)
        {
            _domainEvents ??= new List<DomainEvent>();
            _domainEvents.Add(eventItem);
        }
        
        public void RemoveDomainEvent(DomainEvent eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }
    }
}