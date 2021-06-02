using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Repositories
{
    public class EntityTypeRepository : IEntityTypeRepository
    {
        private readonly DialogManagementContext _context;

        public EntityTypeRepository(DialogManagementContext context)
        {
            _context = context;
        }

        public async Task<ReadOnlyCollection<EntityType>> ListByProjectId(Guid projectId)
        {
            var results = await _context.EntityTypes.Where(e => e.ProjectId == projectId).ToListAsync();
            return results.AsReadOnly();
        }
    }
}
