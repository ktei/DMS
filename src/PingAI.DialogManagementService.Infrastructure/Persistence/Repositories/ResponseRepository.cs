using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Response?> FindById(Guid responseId)
        {
            var response = await _context.Responses
                .SingleOrDefaultAsync(x => x.Id == responseId);
            return response;
        }

        public async Task<IReadOnlyList<Response>> ListByProjectId(Guid projectId, ResponseType responseType)
        {
            var results = await _context.Responses.Where(r => r.ProjectId == projectId)
                .Where(r => r.Type == responseType)
                .ToListAsync();
            return results.ToImmutableList();
        }

        public async Task<Response> Add(Response response)
        {
            var entry = await _context.Responses.AddAsync(response);
            return entry.Entity;
        }

        public void Remove(Response response)
        {
            _context.Responses.Remove(response);
        }

        public void RemoveRange(IEnumerable<Response> responses)
        {
            _context.Responses.RemoveRange(responses);
        }
    }
}