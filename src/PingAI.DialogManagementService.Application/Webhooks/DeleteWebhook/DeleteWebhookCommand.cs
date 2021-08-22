using System;
using MediatR;

namespace PingAI.DialogManagementService.Application.Webhooks.DeleteWebhook
{
    public class DeleteWebhookCommand : IRequest
    {
        public Guid ResponseId { get; }

        public DeleteWebhookCommand(Guid responseId)
        {
            ResponseId = responseId;
        }
    }
}