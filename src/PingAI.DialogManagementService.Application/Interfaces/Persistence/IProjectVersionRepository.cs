using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Persistence
{
    public interface IProjectVersionRepository
    {
        Task<List<ProjectVersion>> GetDesignTimeVersionsByOrganisationId(Guid organisationId);
        // TODO: this method is temporary, just for super admins...
        Task<List<ProjectVersion>> GetDesignTimeVersions();
        Task<ProjectVersion?> GetLatestVersionByProjectId(Guid projectId);
        Task<ProjectVersion> AddProjectVersion(ProjectVersion projectVersion);
    }
}