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
        public ResponseType Type { get; }

        public static Response FromText(string rteText, int order, ResponseType type)
        {
            return new Response(rteText, null, null, order, type);
        }

        public static Response FromForm(FormResolution form, int order)
        {
            return new Response(null, form, null, order, ResponseType.FORM);
        }

        public static Response FromWebhook(Guid webhookResponseId, int order)
        {
            return new Response(null, null, webhookResponseId, order, ResponseType.WEBHOOK);
        }

        private Response(string? rteText, FormResolution? form, Guid? webhookResponseId, int order, ResponseType type)
        {
            RteText = rteText;
            Form = form;
            WebhookResponseId = webhookResponseId;
            Order = order;
            Type = type;
        }
    }
}