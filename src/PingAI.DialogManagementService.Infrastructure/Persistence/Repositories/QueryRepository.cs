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

        public async Task<Query?> GetQueryByIdWithoutJoins(Guid queryId)
        {
            var query = await _context.Queries.FirstOrDefaultAsync(q => q.Id == queryId);
            return query;
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

        public Task<List<Query>> GetQueriesByProjectId(Guid projectId)
        {
            return _context.Queries.Where(q => q.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task<List<Query>> GetQueriesByProjectId(Guid projectId, ResponseType? responseType)
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

            // TODO: we shouldn't compare enums with "ToString", but
            // without it the code throws runtime exception complaining
            // it can't find enum_Response_type
            
            // TODO: this logic is not gonna hold soon
            if (responseType == ResponseType.RTE)
            {
                // RTE query means all responses should be RTE
                query = query.Where(x => x.QueryResponses.All(
                    qr => qr.Response!.Type.ToString() == ResponseType.RTE.ToString() ||
                          qr.Response!.Type.ToString() == ResponseType.QUICK_REPLY.ToString()));
            }
            else if (responseType == ResponseType.HANDOVER || responseType == ResponseType.FORM)
            {
                // otherwise, we filter by response type
                query = query.Where(x => x.QueryResponses.Any(
                    qr => qr.Response!.Type.ToString() == responseType.ToString()));
            }

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