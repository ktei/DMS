using System;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Helpers
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
            Organisation =
                new Organisation("test", "test description", null);
            Project = new Project("test", Organisation.Id,
                "test widget title", "#ffffff",
                "test widget description", "test fallback message", 
                "test greeting message", new string[] { });
            await _context.AddRangeAsync(Organisation, Project);
            EntityName = new EntityName("hometown", Project.Id, true);
            EntityType = new EntityType("city", Project.Id, "city name", null);
            await _context.AddRangeAsync(EntityName, EntityType);
            await _context.SaveChangesAsync();
        }

        public async Task Cleanup()
        {
            _context.RemoveRange(Project, Organisation, EntityName, EntityType);
            await _context.SaveChangesAsync();
        }
    }
}