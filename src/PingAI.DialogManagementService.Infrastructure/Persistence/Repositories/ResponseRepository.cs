using System.Threading.Tasks;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Repositories
{
    public class ResponseRepository : IResponseRepository
    {
        private readonly DialogManagementContext _context;

        public ResponseRepository(DialogManagementContext context)
        {
            _context = context;
        }
        
        public async Task<Response> AddResponse(Response response)
        {
            var result = await _context.Responses.AddAsync(response);
            return result.Entity;
        }
    }
}