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
        public ResponseType type { get; }

        public static Response FromText(string rteText, int order)
        {
            return new Response(rteText, null, null, order);
        }

        public static Response FromForm(FormResolution form, int order)
        {
            return new Response(null, form, null, order);
        }

        public static Response FromWebhook(Guid webhookResponseId, int order)
        {
            return new Response(null, null, webhookResponseId, order);
        }

        private Response(string? rteText, FormResolution? form, Guid? webhookResponseId, int order)
        {
            RteText = rteText;
            Form = form;
            WebhookResponseId = webhookResponseId;
            Order = order;
        }
    }
}