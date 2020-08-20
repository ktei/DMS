using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence
{
    internal static class MediatorExtensions
    {
        public static async Task DispatchDomainEvents(this IMediator mediator, DialogManagementContext context)
        {
            var domainEntities = context.ChangeTracker
                .Entries<DomainEntity>()
                .Where(x => x.Entity.DomainEvents.Any())
                .ToImmutableList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .OrderBy(x => x.TimestampUtc)
                .ToImmutableList();

            foreach (var domainEntity in domainEntities)
            {
                domainEntity.Entity.ClearDomainEvents();
            }

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
    }
}