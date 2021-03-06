using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
using PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Util;
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
            var slackWorkspace = await sut.Add(
                new SlackWorkspace(project.Id, "accesstoken", "http://webhook.com",
                    "team_123"));
            await context.SaveChangesAsync();

            // Assert
            var actual = await context.SlackWorkspaces.AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == slackWorkspace.Id);
            Equal(slackWorkspace.OAuthAccessToken, actual.OAuthAccessToken);
            Equal("team_123", actual.TeamId);
        }

        public Task InitializeAsync() => _testDataFactory.Setup();

        public Task DisposeAsync() => _testDataFactory.Cleanup();
    }
}