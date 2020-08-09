using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Repositories
{
    public class IntentRepository : IIntentRepository
    {
        private readonly DialogManagementContext _context;

        public IntentRepository(DialogManagementContext context)
        {
            _context = context;
        }

        public Task<List<Intent>> GetIntentsByProjectId(Guid projectId) =>
            _context.Intents.Where(x => x.ProjectId == projectId)
                .OrderBy(x => x.Name)
                .ToListAsync();

        public Task<Intent?> GetIntentById(Guid intentId)
        {
            return _context.Intents
                .Include(x => x.PhraseParts)
                .ThenInclude(p => p.EntityType).ThenInclude(e => e.Values)
                .Include(x => x.PhraseParts)
                .ThenInclude(p => p.EntityName)
                .FirstOrDefaultAsync(x => x.Id == intentId);
        }

        public async Task<Intent> AddIntent(Intent intent)
        {
            var result = await _context.AddAsync(intent ?? throw new ArgumentNullException(nameof(intent)));
            return result.Entity;
        }
    }
}