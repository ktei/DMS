using System;
using System.Collections.Generic;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Webhooks.ListWebhooks
{
    public class ListWebhooksQuery : IRequest<IReadOnlyList<Response>>
    {
        public Guid ProjectId { get; }

        public ListWebhooksQuery(Guid projectId)
        {
            ProjectId = projectId;
        }
    }
}