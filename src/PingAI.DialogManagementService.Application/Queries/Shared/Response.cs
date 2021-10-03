using System;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Queries.Shared
{
    public class Response
    {
        public string? RteText { get; }
        public FormResolution? Form { get; }
        public Guid? WebhookResponseId { get; }
        public int Order { get; }

        public Response(string? rteText, FormResolution? form, Guid? webhookResponseId, int order)
        {
            RteText = rteText;
            Form = form;
            WebhookResponseId = webhookResponseId;
            Order = order;
        }
    }
}