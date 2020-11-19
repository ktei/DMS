using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Repositories
{
    public class ChatHistoryRepository : IChatHistoryRepository
    {
        private readonly DialogManagementContext _context;

        public ChatHistoryRepository(DialogManagementContext context)
        {
            _context = context;
        }

        public async Task<List<ChatHistory>> GetChatHistories(Guid designTimeProjectId, DateTime timeRangeStartUtc,
            DateTime? timeRangeEndUtc)
        {
            var allPublishedProjectVersions = await _context.ProjectVersions.Where(x =>
                    x.VersionGroupId == designTimeProjectId)
                .ToListAsync();
            var projectIdsToQuery = allPublishedProjectVersions.Select(x => x.ProjectId)
                .ToArray();
            var query = _context.ChatHistories
                .Where(x => projectIdsToQuery.Contains(x.ProjectId));
            query = query.Where(x => x.CreatedAt >= timeRangeStartUtc);
            if (timeRangeEndUtc.HasValue)
            {
                query = query.Where(x => x.CreatedAt <= timeRangeEndUtc);
            }

            return await query.ToListAsync();
        }
    }
}
