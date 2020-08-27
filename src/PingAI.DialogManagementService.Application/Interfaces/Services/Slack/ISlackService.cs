using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PingAI.DialogManagementService.Application.Interfaces.Services.Slack
{
    public interface ISlackService
    {
        Task<ExchangeCodeResponse> ExchangeOAuthCode(string state, string code);
    }

    public class ExchangeCodeResponse
    {
        [JsonPropertyName("ok")]
        public bool Ok { get; set; }
        
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
        
        [JsonPropertyName("error")]
        public string? Error { get; set; }
        
        [JsonPropertyName("incoming_webhook")]
        public IncomingWebhook? IncomingWebhook { get; set; }
    }

    public class IncomingWebhook
    {
        [JsonPropertyName("channel")]
        public string Channel { get; set; }
        
        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; }
        
        [JsonPropertyName("configuration_url")]
        public string ConfigurationUrl { get; set; }
        
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
