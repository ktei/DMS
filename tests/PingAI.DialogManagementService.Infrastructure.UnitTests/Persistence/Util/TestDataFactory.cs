using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Util
{
    public class TestDataFactory
    {
        private readonly DialogManagementContext _context;
        public Organisation Organisation { get; private set; }
        public Project Project { get; private set; }
        public EntityType EntityType { get; private set; }
        public EntityName EntityName { get; private set; }

        public TestDataFactory(DialogManagementContext context)
        {
            _context = context;
        }

        public async Task Setup()
        {
            // Organisation =
            //     new Organisation(Guid.NewGuid().ToString(), "test description");
            // Project = new Project(Guid.NewGuid().ToString(), Organisation.Id, 
            //     "test widget title", Defaults.WidgetColor,
            //     "test widget description", "test fallback message", 
            //     new string[] { }, null,
            //     Defaults.BusinessTimezone, Defaults.BusinessTimeStartUtc, Defaults.BusinessTimeEndUtc,
            //     null);
            // Organisation.AddProject(Project);
            // await _context.AddRangeAsync(Organisation);
            // EntityName = new EntityName(Project.Id, "hometown", true);
            // EntityType = new EntityType(Project.Id, "city", "city name");
            // await _context.AddRangeAsync(EntityName, EntityType);
            // await _context.SaveChangesAsync();
        }

        public async Task Cleanup()
        {
            // var slackWorkspaces = await _context.SlackWorkspaces
            //     .Where(x => x.ProjectId == Project.Id).ToListAsync();
            // var publishedProjects = 
            //     await _context.ProjectVersions.Include(x => x.Project)
            //         .Include(x => x.Project).ThenInclude(p => p.EntityNames)
            //         .Include(x => x.Project).ThenInclude(p => p.EntityTypes)
            //     .Where(x => x.VersionGroupId == Project.Id)
            //     .ToListAsync();
            // var publishedProjectIds = publishedProjects.Select(x => x.ProjectId)
            //     .ToList();
            // var chatHistories = await _context.ChatHistories
            //     .Where(x => publishedProjectIds.Contains(x.ProjectId)).ToListAsync();
            // _context.RemoveRange(slackWorkspaces);
            // _context.RemoveRange(chatHistories);
            // _context.RemoveRange(publishedProjects.Select(x => x.Project));
            // _context.RemoveRange(publishedProjects.SelectMany(p => p.Project.EntityNames));
            // _context.RemoveRange(publishedProjects.SelectMany(p => p.Project.EntityTypes));
            // _context.RemoveRange(publishedProjects);
            // _context.RemoveRange(Project, Organisation, EntityName, EntityType);
            // await _context.SaveChangesAsync();
        }
    }
}