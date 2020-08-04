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

        public TestDataFactory(DialogManagementContext context)
        {
            _context = context;
        }

        public async Task Setup()
        {
            Organisation =
                new Organisation(Guid.NewGuid(), "test", "test description", null);
            Project = new Project(Guid.NewGuid(), "test", Organisation.Id,
                "test widget title", "#ffffff",
                "test widget description", "test fallback message", 
                "test greeting message", new string[] { });
            await _context.AddAsync(Organisation);
            await _context.AddAsync(Project);
            await _context.SaveChangesAsync();
        }

        public async Task Cleanup()
        {
            _context.Remove(Project);
            _context.Remove(Organisation);
            await _context.SaveChangesAsync();
        }
    }
}