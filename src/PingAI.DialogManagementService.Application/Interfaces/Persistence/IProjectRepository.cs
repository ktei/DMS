using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Persistence
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetProjectsByIds(IEnumerable<Guid> projectIds);
        Task<Project?> GetProjectById(Guid id);
        
        // TODO: this is a bit disgusting, fix me
        Task<Project?> GetFullProjectById(Guid id);

        Task<bool> ProjectNameExists(Guid organisationId, string name);
        Task<Project> AddProject(Project project);
    }
}