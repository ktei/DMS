using System;

namespace PingAI.DialogManagementService.Api.Models.Webhooks
{
    public class WebhookListItem
    {
        public Guid ResponseId { get; set; }
        public string Name { get; set; }

        public WebhookListItem(Guid responseId, string name)
        {
            ResponseId = responseId;
            Name = name;
        }
    }
}