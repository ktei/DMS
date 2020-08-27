using System;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Persistence
{
    public interface ISlackWorkspaceRepository
    {
        Task<SlackWorkspace?> GetSlackWorkspaceByProjectId(Guid projectId);
        Task<SlackWorkspace> AddSlackWorkspace(SlackWorkspace slackWorkspace);
    }
}