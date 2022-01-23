using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services.Nlu;
using PingAI.DialogManagementService.Application.Queries.UpdateQuery;
using PingAI.DialogManagementService.Application.UnitTests.Helpers;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;
using PingAI.DialogManagementService.TestingUtil.AutoMoq;
using Xunit;
using PhrasePart = PingAI.DialogManagementService.Application.Queries.Shared.PhrasePart;
using Response = PingAI.DialogManagementService.Application.Queries.Shared.Response;

namespace PingAI.DialogManagementService.Application.UnitTests.Queries
{
    public class UpdateQueryCommandHandlerTests
    {
        [Fact]
        public async Task UpdateQuery()
        {
            var command = new UpdateQueryCommand(Guid.NewGuid(), "query",
                new[]
                {
                    new PhrasePart(Guid.NewGuid(), PhrasePartType.TEXT, 0, "Hello", null, null)
                }, new Expression[0],
                new[]
                {
                    Response.FromText("rte response", 0, ResponseType.RTE)
                }, "test", null, 0
            );
            Mock<INluService>? nluService = null;
            var sut = FixtureFactory.CreateSut<UpdateQueryCommandHandler>(fixture =>
            {
                fixture.InjectMock<IQueryRepository>(m => m.Setup(x =>
                    x.FindById(It.IsAny<Guid>())).ReturnsAsync(new Query(Guid.NewGuid(),
                    "query", new Expression[0], "query", null, 0)));
                fixture.InjectMock<IEntityNameRepository>(m => m.Setup(x => x.ListByProjectId(It.IsAny<Guid>()))
                    .ReturnsAsync(new EntityName[0]));
                nluService = fixture.InjectMock<INluService>(m => m.Setup(x => x.SaveIntent(It.IsAny<Intent>()))
                    .Returns(Task.CompletedTask)
                    .Verifiable());
                fixture.Inject<IUnitOfWork>(new UnitOfWorkMock());
            });

            var query = await sut.Handle(command, CancellationToken.None);

            query.Should().NotBeNull();
            nluService!.Verify();
        }
    }
}
