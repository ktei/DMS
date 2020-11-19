using System;
using System.Threading.Tasks;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence;
using PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Helpers;
using Xunit;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Repositories
{
    public class ChatHistoryRepositoryTests : IAsyncLifetime
    {
        private readonly DialogManagementContextFactory _dialogManagementContextFactory;
        private readonly TestDataFactory _testDataFactory;
        
        public ChatHistoryRepositoryTests()
        {
            _dialogManagementContextFactory = new DialogManagementContextFactory();
            _testDataFactory = new TestDataFactory(_dialogManagementContextFactory
                .CreateDbContext(new string[] { }));
        }

        [Fact]
        public async Task Test()
        {
            await CreateRandomChatHistories();
        }
        
        private async Task<ChatHistory[]> CreateRandomChatHistories()
        {
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[] { });
            var chatHistories = new[]
            {
                new ChatHistory(Guid.NewGuid(), _testDataFactory.Project.Id,
                    Guid.NewGuid(), Guid.NewGuid(), "{\"text\": {\"type\": \"TEXT\", \"value\": \"hi\"}}", null,
                    SessionStatus.CHATBOT, DateTime.UtcNow, DateTime.UtcNow),
                new ChatHistory(Guid.NewGuid(), _testDataFactory.Project.Id,
                    Guid.NewGuid(), Guid.NewGuid(), "{\"text\": {\"type\": \"TEXT\", \"value\": \"hello\"}}", null,
                    SessionStatus.CHATBOT, DateTime.UtcNow, DateTime.UtcNow),
            };
            await context.ChatHistories.AddRangeAsync(chatHistories);
            await context.SaveChangesAsync();
            return chatHistories;
        }
        
        public Task InitializeAsync() => _testDataFactory.Setup();

        public Task DisposeAsync() => _testDataFactory.Cleanup();
    }
}