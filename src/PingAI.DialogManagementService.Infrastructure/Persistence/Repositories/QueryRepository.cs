using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Repositories
{
    public class QueryRepository : IQueryRepository
    {
        private readonly DialogManagementContext _context;

        public QueryRepository(DialogManagementContext context)
        {
            _context = context;
        }

        public Task<Query?> GetQueryById(Guid queryId) =>
            _context.Queries
                .Include(x => x.QueryIntents)
                .ThenInclude(x => x.Intent)
                .ThenInclude(x => x.PhraseParts)
                .ThenInclude(x => x.EntityName)
                
                .Include(x => x.QueryIntents)
                .ThenInclude(x => x.Intent)
                .ThenInclude(x => x.PhraseParts)
                .ThenInclude(x => x.EntityType)

                .Include(x => x.QueryResponses)
                .ThenInclude(x => x.Response)
                .FirstOrDefaultAsync(x => x.Id == queryId);

        public async Task<Query> AddQuery(Query query)
        {
            var result = await _context.Queries.AddAsync(query);
            return result.Entity;
        }

        public async Task<List<Query>> GetQueriesByProjectId(Guid projectId)
        {
            var results = await _context.Queries
                .Include(x => x.QueryIntents)
                .ThenInclude(x => x.Intent)
                .ThenInclude(x => x.PhraseParts)
                .ThenInclude(x => x.EntityName)
                
                .Include(x => x.QueryIntents)
                .ThenInclude(x => x.Intent)
                .ThenInclude(x => x.PhraseParts)
                .ThenInclude(x => x.EntityType)

                .Include(x => x.QueryResponses)
                .ThenInclude(x => x.Response)
                .Where(x => x.ProjectId == projectId).ToListAsync();
            return results;
        }
    }
}