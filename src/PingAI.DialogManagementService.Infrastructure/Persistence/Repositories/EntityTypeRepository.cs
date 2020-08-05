using System;
using System.Collections.Generic;
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

        public Task<List<EntityType>> GetEntityTypesByProjectId(Guid projectId) =>
            _context.EntityTypes.Where(e => e.ProjectId == projectId).ToListAsync();

        public async Task<EntityType> AddEntityType(EntityType entityType)
        {
            var result = await _context.EntityTypes.AddAsync(entityType);
            return result.Entity;
        }
    }
}