using System;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Services.Nlu
{
    public interface INluService
    {
        Task SaveIntent(Intent intent);
        Task DeleteIntent(Guid projectId, Guid intentId);
        /// <summary>
        /// Export NLU data from source project and save it as target project.
        /// </summary>
        Task Export(Guid sourceProjectId, Guid targetProjectId);
        
        /// <summary>
        /// Import data from source project and apply it to target project.
        /// </summary>
        Task Import(Guid sourceProjectId, Guid targetProjectId, bool runtime = true);
    }
}