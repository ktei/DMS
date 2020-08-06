using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Application.Interfaces.Services.Nlu;
using PingAI.DialogManagementService.Infrastructure.Utils;

namespace PingAI.DialogManagementService.Infrastructure.Services.Nlu
{
    public class NluService : INluService
    {
        private readonly HttpClient _httpClient;

        public NluService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<SaveIntentResponse> SaveIntent(SaveIntentRequest request)
        {
            var httpResponse = await _httpClient.PutAsJsonAsync(
                BuildApiPath("Intents", request.ProjectId),
                request);
            if (httpResponse.IsSuccessStatusCode)
            {
                return await httpResponse.Content.ReadFromJsonAsync<SaveIntentResponse>();
            }

            throw await httpResponse.AsDomainException();
        }

        private static string BuildApiPath(params object[] segments)
            => $"/nlu/1/{string.Join("/", segments)}";
    }
}
