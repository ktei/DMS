using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Repositories
{
    public class ProjectVersionRepository : IProjectVersionRepository
    {
        private readonly DialogManagementContext _context;

        public ProjectVersionRepository(DialogManagementContext context)
        {
            _context = context;
        }
        
        public Task<List<ProjectVersion>> GetDesignTimeVersionsByOrganisationId(Guid organisationId)
        {
            return _context.ProjectVersions.Where(p => p.OrganisationId == organisationId &&
                                                p.Version == Ver.DesignTime)
                .ToListAsync();
        }

        public async Task<ProjectVersion> AddProjectVersion(ProjectVersion projectVersion)
        {
            var result = await _context.ProjectVersions.AddAsync(projectVersion);
            return result.Entity;
        }
    }
}