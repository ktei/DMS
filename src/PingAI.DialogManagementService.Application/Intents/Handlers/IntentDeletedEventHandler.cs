using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Services.Nlu;
using PingAI.DialogManagementService.Domain.Events;

namespace PingAI.DialogManagementService.Application.Intents.Handlers
{
    public class IntentDeletedEventHandler : INotificationHandler<IntentDeletedEvent>
    {
        private readonly INluService _nluService;

        public IntentDeletedEventHandler(INluService nluService)
        {
            _nluService = nluService;
        }
        
        public Task Handle(IntentDeletedEvent notification, CancellationToken cancellationToken)
        {
            return _nluService.DeleteIntent(notification.ProjectId, notification.IntentId);
        }
    }
}