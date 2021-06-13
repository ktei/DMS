using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Application.Interfaces.Services.Nlu;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Utils;
using PhrasePart = PingAI.DialogManagementService.Infrastructure.Services.Nlu.PhrasePart;

namespace PingAI.DialogManagementService.Infrastructure.Services.Nlu
{
    public class NluService : INluService
    {
        private readonly HttpClient _httpClient;

        public NluService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SaveIntent(Intent intent)
        {
            var httpResponse = await _httpClient.PutAsJsonAsync(
                BuildApiPath("Intents", intent.ProjectId),
                new SaveIntentRequest
                {

                    IntentId = intent.Id,
                    Name = intent.Name,
                    ProjectId = intent.ProjectId,
                    TrainingPhrases = intent.PhraseParts.GroupBy(p => p.PhraseId)
                        .Select(g => new TrainingPhrase
                        {
                            Parts = g.Select(p => new PhrasePart
                            {
                                EntityName = p.EntityName?.Name,
                                EntityType = p.EntityType?.Name,
                                EntityValue = p.Value,
                                Text = p.Text,
                                Type = p.Type.ToString()
                            }).ToList()
                        }).ToList()

                });
            if (!httpResponse.IsSuccessStatusCode)
            {
                throw await httpResponse.AsDomainException();
            }
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
