using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
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
                                                p.Version == ProjectVersionNumber.DesignTime)
                .ToListAsync();
        }

        public Task<List<ProjectVersion>> GetDesignTimeVersions()
        {
            return _context.ProjectVersions.Where(p => p.Version == ProjectVersionNumber.DesignTime)
                .ToListAsync();
        }

        public async Task<ProjectVersion?> GetLatestVersionByProjectId(Guid projectId)
        {
            var v = await _context.ProjectVersions
                .FirstOrDefaultAsync(x => x.ProjectId == projectId);
            if (v == null)
                return null;

            return await _context.ProjectVersions.Where(x => x.VersionGroupId == v.VersionGroupId)
                .OrderBy("version DESC")
                // .OrderByDescending(x => x.Version.Number)
                .FirstOrDefaultAsync();
        }

        public async Task<ProjectVersion> AddProjectVersion(ProjectVersion projectVersion)
        {
            var result = await _context.ProjectVersions.AddAsync(projectVersion);
            return result.Entity;
        }
    }
}