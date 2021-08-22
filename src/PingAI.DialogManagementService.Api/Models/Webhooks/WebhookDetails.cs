using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Webhooks
{
    public class WebhookDetails
    {
        public Guid ResponseId { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public string Method { get; set; }
        public string Url { get; set; }
        public IReadOnlyList<KeyValuePair<string, string>> Headers { get; set; }

        public WebhookDetails(Response response)
        {
            ResponseId = response.Id;
            ProjectId = response.ProjectId;
            Name = response.Resolution!.Webhook!.EntityName;
            Method = response.Resolution.Webhook.Method;
            Url = response.Resolution.Webhook.Url;
            Headers =
                response.Resolution.Webhook.Headers.Select(h => new KeyValuePair<string, string>(h.Name, h.Value))
                    .ToImmutableList();
        }
    }
}