using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Repositories
{
    public class ResponseRepository : IResponseRepository
    {
        private readonly DialogManagementContext _context;

        public ResponseRepository(DialogManagementContext context)
        {
            _context = context;
        }

        public Task<Response?> GetResponseById(Guid responseId) =>
            _context.Responses.FirstOrDefaultAsync(x => x.Id == responseId);

        public async Task<Response> AddResponse(Response response)
        {
            var result = await _context.Responses.AddAsync(response);
            return result.Entity;
        }
    }
}