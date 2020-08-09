using System;
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
                .Include(x => x.QueryIntents).ThenInclude(x => x.Intent)
                .Include(x => x.QueryResponses).ThenInclude(x => x.Response)
                .FirstOrDefaultAsync(x => x.Id == queryId);

        public async Task<Query> AddQuery(Query query)
        {
            var result = await _context.Queries.AddAsync(query);
            return result.Entity;
        }
    }
}