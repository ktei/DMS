using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;

namespace PingAI.DialogManagementService.Infrastructure.Persistence.Repositories
{
    public class SlackWorkspaceRepository : ISlackWorkspaceRepository
    {
        private readonly DialogManagementContext _context;

        public SlackWorkspaceRepository(DialogManagementContext context)
        {
            _context = context;
        }

        public async Task<SlackWorkspace?> FindByProjectId(Guid projectId)
        {
            var slackWorkspace = await _context.SlackWorkspaces
                .FirstOrDefaultAsync(x => x.ProjectId == projectId);
            return slackWorkspace;
        }

        public async Task<SlackWorkspace> Add(SlackWorkspace slackWorkspace)
        {
            _ = slackWorkspace ?? throw new ArgumentNullException(nameof(slackWorkspace));
            return (await _context.SlackWorkspaces.AddAsync(slackWorkspace)).Entity;
        }
    }
}