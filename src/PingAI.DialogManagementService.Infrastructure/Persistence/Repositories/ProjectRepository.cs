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

        public Task<List<Project>> GetProjectsByIds(IEnumerable<Guid> projectIds)
        {
            _ = projectIds ?? throw new ArgumentNullException(nameof(projectIds));
            var projects = _context.Projects
                .Include(p => p.Organisation)
                .Where(p => projectIds.Contains(p.Id))
                .OrderBy(p => p.Name);
            return projects.ToListAsync();
        }

        public async Task<Project?> FindById(Guid id)
        {
            var project = await _context.Projects
                .Include(p => p.GreetingResponses)
                .FirstOrDefaultAsync(x => x.Id == id);
            return project;
        }
        
        public async Task<Project?> FindByIdWithJoins(Guid id)
        {
            var project = await _context.Projects
                .AsSplitQuery()
                .Include(p => p.EntityNames)
                .Include(p => p.EntityTypes).ThenInclude(x => x.Values)
                .Include(p => p.Intents).ThenInclude(i => i.PhraseParts)
                .Include(p => p.Responses)
                .Include(p => p.Queries)
                .Include(p => p.GreetingResponses).ThenInclude(x => x.Response)
                .FirstOrDefaultAsync(x => x.Id == id);
            return project;
        }

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