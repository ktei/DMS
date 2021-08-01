using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Application.Interfaces.Services.Caching;
using PingAI.DialogManagementService.Application.Interfaces.Services.Security;
using PingAI.DialogManagementService.Application.Projects.GetProject;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;
using PingAI.DialogManagementService.TestingUtil.AutoMoq;
using Xunit;
using static Moq.It;

namespace PingAI.DialogManagementService.Application.UnitTests.Projects
{
    public class GetProjectQueryHandlerTests
    {
        [Fact]
        public async Task ReturnProjectInCacheIfAvailable()
        {
            Mock<IProjectRepository> projectRepoMock = new Mock<IProjectRepository>();
            var cachedProject = new ProjectCache
            {
                Id = Guid.NewGuid(),
                Name = "CACHED_PROJECT",
                WidgetColor = Defaults.WidgetColor,
                BusinessTimezone = "Australia/Sydney"
            };
            var sut = FixtureFactory.CreateSut<GetProjectQueryHandler>(fixture =>
            {
                fixture.InjectMock<IAuthorizationService>(m =>
                    m.Setup(s => s.UserCanReadProject(IsAny<Guid>()))
                        .ReturnsAsync(true));
                fixture.InjectMock<ICacheService>(m =>
                    m.Setup(s => s.GetObject<ProjectCache>(IsAny<string>(), default))
                        .ReturnsAsync(cachedProject));
                fixture.Inject(projectRepoMock);
            });
            
            var actual = await sut.Handle(new GetProjectQuery(cachedProject.Id), default);
            
            actual.Should().BeEquivalentTo(cachedProject, config =>
                config.Including(x => x.Id)
                    .Including(x => x.Name)
                    .Including(x => x.BusinessTimezone));
            projectRepoMock.Verify(x => x.FindById(IsAny<Guid>()), Times.Never);
        }
    }
}
