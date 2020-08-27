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
        
        [JsonPropertyName("accesss_token")]
        public string AccessToken { get; set; }
        
        [JsonPropertyName("error")]
        public string Error { get; set; }
    }
}
