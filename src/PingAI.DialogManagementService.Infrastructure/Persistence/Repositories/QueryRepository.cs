using System.Threading.Tasks;
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

        public async Task<Query> AddQuery(Query query)
        {
            var result = await _context.Queries.AddAsync(query);
            return result.Entity;
        }
    }
}