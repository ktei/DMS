using System;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Intents.GetIntent
{
    public class GetIntentQuery : IRequest<Intent>
    {
        public Guid IntentId { get; set; }

        public GetIntentQuery(Guid intentId)
        {
            IntentId = intentId;
        }

        public GetIntentQuery()
        {
            
        }
    }
}