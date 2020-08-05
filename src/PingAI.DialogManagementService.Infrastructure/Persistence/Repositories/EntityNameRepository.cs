using System;
using System.Collections.Generic;
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

        public Task<List<EntityName>> GetEntityNamesByProjectId(Guid projectId) =>
            _context.EntityNames.Where(x => x.ProjectId == projectId)
                .ToListAsync();

        public async Task<EntityName> AddEntityName(EntityName entityName)
        {
            var result = await _context.AddAsync(entityName);
            return result.Entity;
        }
    }
}