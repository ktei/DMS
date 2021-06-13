using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Repositories
{
    public class EntityNameRepository : IEntityNameRepository
    {
        private readonly DialogManagementContext _context;

        public EntityNameRepository(DialogManagementContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<EntityName>> ListByProjectId(Guid projectId)
        {
            var results = await _context.EntityNames.Where(x => x.ProjectId == projectId)
                .ToListAsync();
            return results.ToImmutableList();
        }

        public async Task<EntityName> Add(EntityName entityName)
        {
            var result = await _context.AddAsync(entityName);
            return result.Entity;
        }
    }
}
