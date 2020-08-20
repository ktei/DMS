using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Services.Nlu;
using PingAI.DialogManagementService.Domain.Events;

namespace PingAI.DialogManagementService.Application.Intents.Handlers
{
    public class IntentUpdatedEventHandler : INotificationHandler<IntentUpdatedEvent>
    {
        private readonly INluService _nluService;

        public IntentUpdatedEventHandler(INluService nluService)
        {
            _nluService = nluService;
        }
        
        public Task Handle(IntentUpdatedEvent notification, CancellationToken cancellationToken)
        {
            var intent = notification.Intent;
            return _nluService.SaveIntent(new SaveIntentRequest
            {
                IntentId = intent.Id,
                Name = intent.Name,
                ProjectId = intent.ProjectId,
                TrainingPhrases = intent.PhraseParts.GroupBy(p => p.PhraseId)
                    .Select(g => new TrainingPhrase
                    {
                        Parts = g.Select(p => new PhrasePart
                        {
                            EntityName = p.EntityName?.Name,
                            EntityType = p.EntityType?.Name,
                            EntityValue = p.Value,
                            Text = p.Text,
                            Type = p.Type.ToString()
                        }).ToList()
                    }).ToList()
            });
        }
    }
}