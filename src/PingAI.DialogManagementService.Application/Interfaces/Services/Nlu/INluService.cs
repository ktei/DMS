using System;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Services.Nlu
{
    public interface INluService
    {
        Task SaveIntent(Intent intent);
        Task DeleteIntent(Guid projectId, Guid intentId);
        Task Export(Guid sourceProjectId, Guid targetProjectId);
        Task Import(Guid sourceProjectId, Guid targetProjectId, bool runtime = true);
    }
}