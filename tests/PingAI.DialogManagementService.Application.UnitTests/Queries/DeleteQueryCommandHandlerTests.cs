using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using MediatR;
using Moq;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services.Nlu;
using PingAI.DialogManagementService.Application.Queries.DeleteQuery;
using PingAI.DialogManagementService.Application.UnitTests.Helpers;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;
using PingAI.DialogManagementService.TestingUtil.AutoMoq;
using Xunit;

namespace PingAI.DialogManagementService.Application.UnitTests.Queries
{
    public class DeleteQueryCommandHandlerTests
    {
        [Fact]
        public async Task DeleteQuery()
        {
            var command = new DeleteQueryCommand(Guid.NewGuid());
            Mock<INluService>? nluService = null;
            IRequestHandler<DeleteQueryCommand, Unit> sut = FixtureFactory.CreateSut<DeleteQueryCommandHandler>(
                fixture =>
                {
                    fixture.InjectMock<IQueryRepository>(m =>
                        m.Setup(x => x.FindById(command.QueryId))
                            .ReturnsAsync(() =>
                            {
                                var query = new Query(Guid.NewGuid(), "q", new Expression[0],
                                    "test", null, 0);
                                query.AddIntent(new Intent("test", IntentType.STANDARD));
                                return query;
                            }));
                    nluService = fixture.InjectMock<INluService>(m => m
                        .Setup(x => x.DeleteIntent(It.IsAny<Guid>(), It.IsAny<Guid>()))
                        .Returns(Task.CompletedTask)
                        .Verifiable());
                    fixture.Inject<IUnitOfWork>(new UnitOfWorkMock());
                });

            await sut.Handle(command, CancellationToken.None);

            nluService!.Verify();
        }
    }
}
