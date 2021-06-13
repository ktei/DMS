using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Repositories
{
    public class QueryRepository : IQueryRepository
    {
        private readonly DialogManagementContext _context;

        public QueryRepository(DialogManagementContext context)
        {
            _context = context;
        }

        public async Task<Query?> FindById(Guid queryId)
        {
            var query = await _context.Queries
                .AsSplitQuery()
                .Include(x => x.Intents)
                .ThenInclude(x => x.PhraseParts)
                .ThenInclude(x => x.EntityName)

                .Include(x => x.Intents)
                .ThenInclude(x => x.PhraseParts)
                .ThenInclude(x => x.EntityType)

                .Include(x => x.Responses)
                .FirstOrDefaultAsync(x => x.Id == queryId);
            return query;
        }
        
        public async Task<IReadOnlyList<Query>> ListByProjectId(Guid projectId, ResponseType? responseType)
        {
            var query = _context.Queries
                .AsSplitQuery()

                .Include(x => x.Intents)
                .ThenInclude(x => x.PhraseParts)
                .ThenInclude(x => x.EntityName)

                .Include(x => x.Intents)
                .ThenInclude(x => x.PhraseParts)
                .ThenInclude(x => x.EntityType)

                .Include(x => x.Responses)
                .Where(x => x.ProjectId == projectId);

            // TODO: we shouldn't compare enums with "ToString", but
            // without it the code throws runtime exception complaining
            // it can't find enum_Response_type

            switch (responseType)
            {
                // TODO: this logic is not gonna hold soon
                case ResponseType.RTE:
                    // RTE query means all responses should be RTE
                    query = query.Where(x => x.Responses.All(
                        qr => qr.Type == ResponseType.RTE ||
                              qr.Type == ResponseType.QUICK_REPLY));
                    break;
                case ResponseType.HANDOVER:
                case ResponseType.FORM:
                    // otherwise, we filter by response type
                    query = query.Where(x => x.Responses.Any(
                        qr => qr.Type == responseType));
                    break;
            }

            var results = await query.OrderBy(x => x.DisplayOrder)
                .ToListAsync();

            return results.ToImmutableList();
        }

        public async Task<Query> Add(Query query)
        {
            var result = await _context.Queries.AddAsync(query);
            return result.Entity;
        }

        public async Task<IReadOnlyList<Query>> ListByProjectIdWithoutJoins(Guid projectId)
        {
            var results = await _context.Queries.Where(q => q.ProjectId == projectId)
                .ToListAsync();
            return results.ToImmutableList();
        }

        public void Remove(Query query)
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
