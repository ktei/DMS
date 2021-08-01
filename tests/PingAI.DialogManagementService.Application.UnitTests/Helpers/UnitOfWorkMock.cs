using System;
using System.Threading.Tasks;
using Moq;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.UnitTests.Helpers
{
    public class UnitOfWorkMock : IUnitOfWork
    {
        public Mock<Func<Task>>? SaveChangesMock { get; set; }
        public Mock<Func<Task>>? ExecuteTransactionMock { get; set; }

        public int GetContextHashCode()
        {
            return 0;
        }

        public Task<User> GetUser(string auth0Id)
        {
            return null;
        }

        public Task SaveChanges()
        {
            if (SaveChangesMock != null)
                return SaveChangesMock.Object.Invoke();
            return Task.CompletedTask;
        }

        public async Task ExecuteTransaction(Func<Task> beforeCommit)
        {
            if (ExecuteTransactionMock != null)
            {
                await ExecuteTransactionMock.Object.Invoke();
                return;
            }

            await beforeCommit();
        }
    }
}
