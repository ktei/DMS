using System;
using System.Threading.Tasks;
using FluentAssertions;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
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
        public async Task GetChatHistories()
        {
            // Arrange
            var sut = new ChatHistoryRepository(_dialogManagementContextFactory.CreateDbContext(new string[] { }));
            var dbContext = _dialogManagementContextFactory.CreateDbContext(new string[] { });
            var publishedProject = _testDataFactory.Project.Export();
            await dbContext.AddAsync(publishedProject);
            await dbContext.AddAsync(new ProjectVersion(publishedProject.Id,
                _testDataFactory.Organisation.Id, _testDataFactory.Project.Id,
                new ProjectVersionNumber(1)));
            await dbContext.SaveChangesAsync();
            var chatHistories = await CreateRandomChatHistories(publishedProject.Id);
            
            // Act
            var actual = await sut.GetChatHistories(_testDataFactory.Project.Id, 
                DateTime.UtcNow.AddMinutes(-1), null);

            // Assert
            actual.Should().HaveCountGreaterOrEqualTo(2);
        }
        
        private async Task<ChatHistory[]> CreateRandomChatHistories(Guid projectId)
        {
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[] { });
            var chatHistories = new[]
            {
                new ChatHistory(Guid.NewGuid(), projectId,
                    Guid.NewGuid(), Guid.NewGuid(), "{\"text\": {\"type\": \"TEXT\", \"value\": \"hi\"}}", null,
                    SessionStatus.CHATBOT, DateTime.UtcNow, DateTime.UtcNow),
                new ChatHistory(Guid.NewGuid(), projectId,
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