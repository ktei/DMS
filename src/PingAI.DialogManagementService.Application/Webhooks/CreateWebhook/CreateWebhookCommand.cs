using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;

namespace PingAI.DialogManagementService.Application.Webhooks.CreateWebhook
{
    public class CreateWebhookCommand  : IRequest<CreateWebhookResult>
    {
        public Guid ProjectId { get; }
        public string Name { get; }
        public string Method { get; }
        public string Url { get; }
        public List<KeyValuePair<string, string>> Headers { get; }

        public CreateWebhookCommand(Guid projectId, string name, string method, string url, 
            IEnumerable<KeyValuePair<string, string>> headers)
        {
            ProjectId = projectId;
            Name = name;
            Method = method;
            Url = url;
            Headers = headers.ToList();
        }
    }
}
