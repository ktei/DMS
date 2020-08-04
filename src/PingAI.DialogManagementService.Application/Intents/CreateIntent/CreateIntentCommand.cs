using System;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Intents.CreateIntent
{
    public class CreateIntentCommand : IRequest<Intent>
    {
        public string Name { get; set; }
        public Guid ProjectId { get; set; }
        public IntentType IntentType { get; set; }
    }
}