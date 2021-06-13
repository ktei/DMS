using System;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Domain.Repositories
{
    public interface ISlackWorkspaceRepository
    {
        Task<SlackWorkspace?> FindByProjectId(Guid projectId);
        Task<SlackWorkspace> Add(SlackWorkspace slackWorkspace);
    }
}