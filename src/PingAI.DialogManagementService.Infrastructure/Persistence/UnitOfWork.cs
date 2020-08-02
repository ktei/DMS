using System.Threading.Tasks;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;

namespace PingAI.DialogManagementService.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DialogManagementContext _context;

        public UnitOfWork(DialogManagementContext context)
        {
            _context = context;
        }

        public Task SaveChanges() => _context.SaveChangesAsync();
    }
}