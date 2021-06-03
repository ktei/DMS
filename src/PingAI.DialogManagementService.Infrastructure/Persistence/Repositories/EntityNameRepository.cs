using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Repositories
{
    public class EntityNameRepository : IEntityNameRepository
    {
        private readonly DialogManagementContext _context;

        public EntityNameRepository(DialogManagementContext context)
        {
            _context = context;
        }

        public async Task<ReadOnlyCollection<EntityName>> ListByProjectId(Guid projectId)
        {
            var results = await _context.EntityNames.Where(x => x.ProjectId == projectId)
                .ToListAsync();
            return results.AsReadOnly();
        }

        public async Task<EntityName> Add(EntityName entityName)
        {
            var result = await _context.AddAsync(entityName);
            return result.Entity;
        }
    }
}
