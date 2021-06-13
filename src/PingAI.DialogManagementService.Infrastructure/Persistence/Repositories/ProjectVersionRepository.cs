using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Repositories
{
    public class ProjectVersionRepository : IProjectVersionRepository
    {
        private readonly DialogManagementContext _context;

        public ProjectVersionRepository(DialogManagementContext context)
        {
            _context = context;
        }
        
        public async Task<ProjectVersion?> FindLatestByProjectId(Guid projectId)
        {
            var v = await _context.ProjectVersions
                .FirstOrDefaultAsync(x => x.ProjectId == projectId);
            if (v == null)
                return null;

            return await _context.ProjectVersions.Where(x => x.VersionGroupId == v.VersionGroupId)
                .OrderBy("version DESC")
                .FirstOrDefaultAsync();
        }
    }
}
