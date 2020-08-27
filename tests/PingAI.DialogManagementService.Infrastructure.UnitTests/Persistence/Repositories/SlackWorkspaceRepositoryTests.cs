using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
using PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Helpers;
using Xunit;
using static Xunit.Assert;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Repositories
{
    public class SlackWorkspaceRepositoryTests : IAsyncLifetime
    {
        private readonly DialogManagementContextFactory _dialogManagementContextFactory;
        private readonly TestDataFactory _testDataFactory;

        public SlackWorkspaceRepositoryTests()
        {
            _dialogManagementContextFactory = new DialogManagementContextFactory();
            _testDataFactory = new TestDataFactory(_dialogManagementContextFactory.CreateDbContext(new string[] { }));
        }


        [Fact]
        public async Task AddSlackWorkspace()
        {
            // Arrange
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[] { });
            var project = _testDataFactory.Project;
            var sut = new SlackWorkspaceRepository(context);
            
            // Act
            var slackWorkspace = await sut.AddSlackWorkspace(
                new SlackWorkspace(project.Id, "accesstoken", "http://webhook.com"));
            await context.SaveChangesAsync();

            // Assert
            var actual = await context.SlackWorkspaces.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == slackWorkspace.Id);
            Equal(slackWorkspace.OAuthAccessToken, actual.OAuthAccessToken);
        }

        public Task InitializeAsync() => _testDataFactory.Setup();

        public Task DisposeAsync() => _testDataFactory.Cleanup();
    }
}