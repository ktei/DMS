using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        public async Task<IReadOnlyList<Intent>> ListByProjectId(Guid projectId)
        {
            var results = await _context.Intents.Where(x => x.ProjectId == projectId)
                .OrderBy(x => x.Name)
                .ToListAsync();
            return results.ToImmutableList();
        }

        public async Task<Intent?> FindById(Guid intentId)
        {
            var intent = await _context.Intents
                .AsSplitQuery()
                .Include(x => x.PhraseParts)
                .ThenInclude(p => p.EntityType)
                .ThenInclude(e => e!.Values)

                .Include(x => x.PhraseParts)
                .ThenInclude(p => p.EntityName)
                .FirstOrDefaultAsync(x => x.Id == intentId);
            return intent;
        }

        public async Task<Intent> Add(Intent intent)
        {
            var result = await _context.AddAsync(intent ?? throw new ArgumentNullException(nameof(intent)));
            return result.Entity;
        }

        public void Remove(Intent intent)
        {
            _context.Intents.Remove(intent);
        }
    }
}
