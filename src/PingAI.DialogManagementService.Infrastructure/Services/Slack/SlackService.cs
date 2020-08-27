using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Application.Interfaces.Services.Slack;
using PingAI.DialogManagementService.Infrastructure.Configuration;
using PingAI.DialogManagementService.Infrastructure.Utils;

namespace PingAI.DialogManagementService.Infrastructure.Services.Slack
{
    public class SlackService : ISlackService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfigurationManager _configurationManager;

        public SlackService(HttpClient httpClient, IConfigurationManager configurationManager)
        {
            _httpClient = httpClient;
            _configurationManager = configurationManager;
        }
        
        public async Task<ExchangeCodeResponse> ExchangeOAuthCode(string code, string redirectUri)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentException(nameof(code));
            if (string.IsNullOrEmpty(_configurationManager.SlackClientId))
                throw new InvalidOperationException($"Configure {nameof(_configurationManager.SlackClientId)} first");
            if (string.IsNullOrEmpty(_configurationManager.SlackClientSecret))
                throw new InvalidOperationException($"Configure {nameof(_configurationManager.SlackClientSecret)} first");
            
            var dict = new Dictionary<string, string>
            {
                {"code", code},
                {"client_id", _configurationManager.SlackClientId},
                {"client_secret", _configurationManager.SlackClientSecret},
                {"redirect_uri", redirectUri.Substring(0, 
                    redirectUri.IndexOf("?", StringComparison.OrdinalIgnoreCase))}
            };
            var req = new HttpRequestMessage(HttpMethod.Post, "/api/oauth.v2.access")
            {
                Content = new FormUrlEncodedContent(dict)
            };
            var res = await _httpClient.SendAsync(req);
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                return await res.Content.ReadFromJsonAsync<ExchangeCodeResponse>();
            }

            throw await res.AsDomainException();
        }
    }
}