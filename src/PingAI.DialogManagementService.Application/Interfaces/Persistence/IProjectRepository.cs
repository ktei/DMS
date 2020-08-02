using System;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Interfaces.Persistence
{
    public interface IProjectRepository
    {
        Task<Project?> GetProjectById(Guid id);
        Task<Project> AddProject(Project project);
    }
}