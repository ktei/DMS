using System.Text.Json.Serialization;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class WebhookHeader
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonConstructor]
        public WebhookHeader(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}