using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task<List<Query>> GetQueriesByProjectId(Guid projectId,
            Expression<Func<Query, bool>>? filter = null)
        {
            var query = _context.Queries
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
                .Where(x => x.ProjectId == projectId);
            
            if (filter != null)
                query = query.Where(filter!);
            
            var results = await query.OrderBy(x => x.DisplayOrder)
                .ToListAsync();

            return results;
        }

        public void RemoveQuery(Query query)
        {
            _context.Queries.Remove(query);
        }

        public async Task<int> GetMaxDisplayOrder(Guid projectId)
        {
            var queryWithMaxDisplayOrder = await _context.Queries
                .Where(x => x.ProjectId == projectId)
                .OrderByDescending(x => x.DisplayOrder)
                .FirstOrDefaultAsync();

            return queryWithMaxDisplayOrder?.DisplayOrder ?? 0;
        }
    }
}