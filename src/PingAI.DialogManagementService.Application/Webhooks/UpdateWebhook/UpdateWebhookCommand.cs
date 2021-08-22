using System;
using System.Collections.Generic;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Webhooks.UpdateWebhook
{
    public class UpdateWebhookCommand : IRequest<Response>
    {
        public Guid ResponseId { get; }
        public string Method { get; }
        public string Url { get; }
        public List<KeyValuePair<string, string>> Headers { get; }

        public UpdateWebhookCommand(Guid responseId, string method, string url,
            List<KeyValuePair<string, string>> headers)
        {
            ResponseId = responseId;
            Method = method;
            Url = url;
            Headers = headers;
        }
    }
}