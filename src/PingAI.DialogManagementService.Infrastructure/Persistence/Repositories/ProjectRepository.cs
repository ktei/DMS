using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DialogManagementContext _context;

        public ProjectRepository(DialogManagementContext context)
        {
            _context = context;
        }

        public Task<Project?> GetProjectById(Guid id) => 
            _context.Projects.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<Project> AddProject(Project project)
        {
            var result =
                await _context.AddAsync(project ?? throw new ArgumentNullException(nameof(project)));
            return result.Entity;
        }
    }
}