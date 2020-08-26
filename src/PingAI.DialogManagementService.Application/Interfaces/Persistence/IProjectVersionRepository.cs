using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Persistence
{
    public interface IProjectVersionRepository
    {
        Task<List<ProjectVersion>> GetDesignTimeVersionsByOrganisationId(Guid organisationId);
        Task<ProjectVersion?> GetLatestVersionByProjectId(Guid projectId);
        Task<ProjectVersion> AddProjectVersion(ProjectVersion projectVersion);
    }
}