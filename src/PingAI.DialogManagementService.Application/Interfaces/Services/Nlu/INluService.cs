using System;
using System.Threading.Tasks;

namespace PingAI.DialogManagementService.Application.Interfaces.Services.Nlu
{
    public interface INluService
    {
        Task<SaveIntentResponse> SaveIntent(SaveIntentRequest request);
        Task DeleteIntent(Guid projectId, Guid intentId);
    }
}