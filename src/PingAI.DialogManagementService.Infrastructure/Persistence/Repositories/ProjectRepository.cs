using System;
using System.Collections.Generic;
using System.Linq;
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

        public Task<List<Project>> GetProjectsByIds(IEnumerable<Guid> projectIds,
            Func<IQueryable<Project>, IQueryable<Project>>? configureQuery = null)
        {
            _ = projectIds ?? throw new ArgumentNullException(nameof(projectIds));
            var projects = _context.Projects.Where(p => projectIds.Contains(p.Id));
            projects = configureQuery?.Invoke(projects) ?? projects;
            return projects.ToListAsync();
        }

        public Task<Project?> GetProjectById(Guid id) => 
            _context.Projects.FirstOrDefaultAsync(x => x.Id == id);

        public Task<bool> ProjectNameExists(Guid organisationId, string name)
        {
            return _context.Projects.AnyAsync(p => p.Name == name && 
                                                   p.OrganisationId == organisationId);
        }

        public async Task<Project> AddProject(Project project)
        {
            var result =
                await _context.AddAsync(project ?? throw new ArgumentNullException(nameof(project)));
            return result.Entity;
        }
    }
}