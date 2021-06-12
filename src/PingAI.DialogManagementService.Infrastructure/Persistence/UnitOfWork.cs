using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        public async Task ExecuteTransaction(Func<Task> beforeCommit)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await _context.BeginTransactionAsync();
                await _context.CommitTransactionAsync(beforeCommit);
            });
        }
    }
}
