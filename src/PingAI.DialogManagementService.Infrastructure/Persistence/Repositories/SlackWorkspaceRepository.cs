using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Repositories
{
    public class SlackWorkspaceRepository : ISlackWorkspaceRepository
    {
        private readonly DialogManagementContext _context;

        public SlackWorkspaceRepository(DialogManagementContext context)
        {
            _context = context;
        }

        public Task<SlackWorkspace?> GetSlackWorkspaceByProjectId(Guid projectId)
        {
            return _context.SlackWorkspaces
                .FirstOrDefaultAsync(x => x.ProjectId == projectId);
        }

        public async Task<SlackWorkspace> AddSlackWorkspace(SlackWorkspace slackWorkspace)
        {
            _ = slackWorkspace ?? throw new ArgumentNullException(nameof(slackWorkspace));
            return (await _context.SlackWorkspaces.AddAsync(slackWorkspace)).Entity;
        }
    }
}