using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
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

        public async Task<IReadOnlyList<Project>> ListByOrganisationId(Guid organisationId)
        {
            var results = await _context.Projects.Where(x => x.ProjectVersion.Version == ProjectVersionNumber.DesignTime
                                         && x.OrganisationId == organisationId)
                .ToListAsync();
            return results.ToImmutableList();
        }

        public async Task<IReadOnlyList<Project>> ListAll()
        {
            var results = await _context.Projects
                .Where(x => x.ProjectVersion.Version == ProjectVersionNumber.DesignTime).ToListAsync();
            return results.ToImmutableList();
        }

        public async Task<Project?> FindById(Guid id)
        {
            var project = await _context.Projects
                .Include(p => p.GreetingResponses)
                .Include(p => p.ProjectVersion)
                .FirstOrDefaultAsync(x => x.Id == id);
            return project;
        }
        
        public async Task<Project?> FindLatestVersion(Guid designTimeId)
        {
            var projectVersion = await _context.ProjectVersions
                .Include(x => x.Project).ThenInclude(x => x.ProjectVersion)
                .Where(x => x.VersionGroupId == designTimeId)
                .OrderBy("version DESC")
                .FirstOrDefaultAsync();
            if (projectVersion == null)
                return null;
            var project = await _context.Projects.FirstAsync(x => x.Id == projectVersion.ProjectId);
            return project;
        }

        public async Task<Project?> FindByIdWithJoins(Guid id)
        {
            var project = await _context.Projects
                .AsSplitQuery()
                .Include(p => p.ProjectVersion)
                .Include(p => p.EntityNames)
                .Include(p => p.EntityTypes).ThenInclude(x => x.Values)
                .Include(p => p.Intents).ThenInclude(i => i.PhraseParts)
                .Include(p => p.Responses)
                .Include(p => p.Queries)
                .Include(p => p.GreetingResponses).ThenInclude(x => x.Response)
                .FirstOrDefaultAsync(x => x.Id == id);
            return project;
        }

        public async Task<Project> Add(Project project)
        {
            var result =
                await _context.AddAsync(project ?? throw new ArgumentNullException(nameof(project)));
            return result.Entity;
        }
    }
}