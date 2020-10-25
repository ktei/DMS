using System;
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

        public async Task DeleteIntent(Guid projectId, Guid intentId)
        {
            var httpResponse = 
                await _httpClient.DeleteAsync(BuildApiPath("Intents", projectId, intentId));
            if (httpResponse.IsSuccessStatusCode)
            {
                return;
            }

            throw await httpResponse.AsDomainException();
        }

        public async Task Export(Guid sourceProjectId, Guid targetProjectId)
        {
            var httpResponse =
                await _httpClient.PostAsJsonAsync(BuildApiPath("Projects", "export"),
                    new
                    {
                        sourceProjectId,
                        targetProjectId
                    });
            
            if (httpResponse.IsSuccessStatusCode)
            {
                return;
            }
            
            throw await httpResponse.AsDomainException();
        }

        public async Task Import(Guid sourceProjectId, Guid targetProjectId, bool runtime = true)
        {
            var httpResponse =
                await _httpClient.PostAsJsonAsync(BuildApiPath("Projects", "import"),
                    new
                    {
                        sourceProjectId,
                        targetProjectId,
                        projectVersion = runtime ? "Runtime" : "DesignTime"
                    });
            
            if (httpResponse.IsSuccessStatusCode)
                return;

            throw await httpResponse.AsDomainException();
        }

        private static string BuildApiPath(params object[] segments)
            => $"/nlu/1/{string.Join("/", segments)}";
    }
}
