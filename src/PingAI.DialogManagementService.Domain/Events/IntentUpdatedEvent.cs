using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Domain.Events
{
    public class IntentUpdatedEvent : DomainEvent 
    {
        public Intent Intent { get; }

        public IntentUpdatedEvent(Intent intent)
        {
            Intent = intent;
        }
    }
}