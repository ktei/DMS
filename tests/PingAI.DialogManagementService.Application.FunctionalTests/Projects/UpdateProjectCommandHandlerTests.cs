using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Application.Projects.UpdateProject;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
using Xunit;
using static Xunit.Assert;
using static Moq.It;

namespace PingAI.DialogManagementService.Application.FunctionalTests.Projects
{
    public class UpdateProjectCommandHandlerTests
    {
        private readonly DialogManagementContextFactory _dialogManagementContextFactory;

        public UpdateProjectCommandHandlerTests()
        {
            _dialogManagementContextFactory = new DialogManagementContextFactory();
        }

        [Fact]
        public async Task UpdateProject()
        {
            // Arrange
            await using var context = _dialogManagementContextFactory.CreateDbContext(new string[] { });
            var projectRepository = new ProjectRepository(context);
            var unitOfWork = new UnitOfWork(context);
            var authServiceMock = new Mock<IAuthService>();
            authServiceMock.Setup(m => m.UserCanWriteProject(IsAny<Project>()))
                .ReturnsAsync(true);
            var sut = new UpdateProjectCommandHandler(projectRepository, unitOfWork, authServiceMock.Object);
            var organisation = new Organisation(Guid.NewGuid(), Guid.NewGuid().ToString(),
                "test", null);
            var project = new Project(Guid.NewGuid(), "test project", organisation.Id,
                "widget title", "#ffffff", "widget description",
                "fallback message", "greeting message", new string[] { });
            await context.AddAsync(organisation);
            await context.AddAsync(project);
            await context.SaveChangesAsync();

            // Act
            await sut.Handle(new UpdateProjectCommand(project.Id, "my title", "#eeeeee",
                    "my description", "my fallback message", "my greeting message",
                    new []{"e1", "e2"}),
                CancellationToken.None);
            var actual = await context.Projects.AsNoTracking().FirstAsync(x => x.Id == project.Id);

            // Assert

            // clean up
            context.Remove(organisation);
            context.Remove(project);
            await context.SaveChangesAsync();

            NotNull(actual);
            Equal("my title", actual.WidgetTitle);
            Equal("#eeeeee", actual.WidgetColor);
            Equal("my description", actual.WidgetDescription);
            Equal("my fallback message", actual.FallbackMessage);
            Equal("my greeting message", actual.GreetingMessage);
            Equal(new[]{"e1", "e2"}, actual.Enquiries);
        }
    }
}