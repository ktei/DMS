using System;
using System.Text.Json.Serialization;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class WebhookResolution
    {
        [JsonPropertyName("entityNameId")]
        public Guid EntityNameId { get; set; }
        [JsonPropertyName("entityName")]
        public string EntityName { get; set; }
        
        [JsonPropertyName("method")]
        public string Method { get; set; }
        
        public string Url { get; set; }
        
        [JsonPropertyName("headers")]
        public WebhookHeader[] Headers { get; set; }

        [JsonConstructor]
        public WebhookResolution(Guid entityNameId, string entityName, string method, string url,
            WebhookHeader[] headers)
        {
            EntityNameId = entityNameId;
            EntityName = entityName;
            Method = method;
            Url = url;
            Headers = headers;
        }
    }
}