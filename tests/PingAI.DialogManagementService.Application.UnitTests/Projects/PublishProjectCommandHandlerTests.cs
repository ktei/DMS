using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services.Nlu;
using PingAI.DialogManagementService.Application.Interfaces.Services.Security;
using PingAI.DialogManagementService.Application.Projects.PublishProject;
using PingAI.DialogManagementService.Application.UnitTests.Helpers;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;
using PingAI.DialogManagementService.TestingUtil.AutoMoq;
using Xunit;

namespace PingAI.DialogManagementService.Application.UnitTests.Projects
{
    public class PublishProjectCommandHandlerTests
    {
        [Fact]
        public async Task PublishProject()
        {
            var sourceProjectId = Guid.NewGuid();
            var targetProjectId = Guid.NewGuid();
            var command = new PublishProjectCommand(sourceProjectId);
            Mock<INluService>? nluService = null;
            var sut = FixtureFactory.CreateSut<PublishProjectCommandHandler>(
                fixture =>
                {
                    fixture.InjectMock<IAuthorizationService>(m =>
                        m.Setup(x => x.UserCanWriteProject(sourceProjectId))
                            .ReturnsAsync(true));
                    nluService = fixture.InjectMock<INluService>(m =>
                    {
                        m
                            .Setup(x => x.Export(sourceProjectId, targetProjectId))
                            .Returns(Task.CompletedTask)
                            .Verifiable();
                        m.Setup(x => x.Import(targetProjectId, sourceProjectId, true))
                            .Returns(Task.CompletedTask)
                            .Verifiable();
                    });

                    var projectRepo = fixture.InjectMock<IProjectRepository>(m =>
                        m.Setup(x => x.Add(It.IsAny<Project>()))
                            .ReturnsAsync(Project.CreateWithDefaults(Guid.NewGuid(), $"test_{DateTime.UtcNow.Ticks}"))
                            .Callback((Project project) =>
                            {
                                project.GetType().GetProperty("Id")!.SetValue(project, targetProjectId);
                            }));
                    projectRepo.Setup(x => x.FindByIdWithJoins(sourceProjectId))
                        .ReturnsAsync(new Project(sourceProjectId, Guid.NewGuid(), "test",
                            Defaults.WidgetTitle, Defaults.WidgetColor,
                            Defaults.WidgetDescription, Defaults.FallbackMessage,
                            null, new []{"http://mydomain.com"}, Defaults.BusinessTimezone, Defaults.BusinessTimeStartUtc,
                            Defaults.BusinessTimeEndUtc, "abc@gmail.com"));
                    fixture.Inject<IUnitOfWork>(new UnitOfWorkMock());
                });

            var actual = await sut.Handle(command, CancellationToken.None);

            actual.Should().NotBeNull();
            nluService!.Verify();
        }
    }
}