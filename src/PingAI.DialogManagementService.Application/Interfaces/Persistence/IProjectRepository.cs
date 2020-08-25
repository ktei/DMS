using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Persistence
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetProjectsByIds(IEnumerable<Guid> projectIds,
            Func<IQueryable<Project>, IQueryable<Project>>? configureQuery = null);
        Task<Project?> GetProjectById(Guid id);
        Task<Project> AddProject(Project project);
    }
}