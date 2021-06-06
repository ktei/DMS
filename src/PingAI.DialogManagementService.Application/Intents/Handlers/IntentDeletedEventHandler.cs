using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services.Nlu;
using PingAI.DialogManagementService.Domain.Events;

namespace PingAI.DialogManagementService.Application.Intents.Handlers
{
    public class IntentDeletedEventHandler : INotificationHandler<IntentDeletedEvent>
    {
        private readonly INluService _nluService;
        private readonly IIntentRepository _intentRepository;

        public IntentDeletedEventHandler(INluService nluService, IIntentRepository intentRepository)
        {
            _nluService = nluService;
            _intentRepository = intentRepository;
        }
        
        public Task Handle(IntentDeletedEvent notification, CancellationToken cancellationToken)
        {
            var intent = notification.Intent;
            _intentRepository.Remove(intent);
            return _nluService.DeleteIntent(intent.ProjectId, intent.Id);
        }
    }
}