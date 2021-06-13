using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Domain.Repositories
{
    public interface IProjectRepository
    {
        Task<IReadOnlyList<Project>> ListByOrganisationId(Guid organisationId);
        Task<IReadOnlyList<Project>> ListAll();
        Task<Project?> FindById(Guid id);
        Task<Project?> FindLatestVersion(Guid designTimeId);
        
        // TODO: this is a bit disgusting, fix me
        Task<Project?> FindByIdWithJoins(Guid id);
        Task<Project> Add(Project project);
    }
}