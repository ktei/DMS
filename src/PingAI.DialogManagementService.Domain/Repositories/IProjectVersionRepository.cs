using System;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Domain.Repositories
{
    public interface IProjectVersionRepository
    {
        Task<ProjectVersion?> FindLatestByProjectId(Guid projectId);
        Task<ProjectVersion> Add(ProjectVersion projectVersion);
    }
}