using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services.Nlu;
using PingAI.DialogManagementService.Application.Queries.CreateQuery;
using PingAI.DialogManagementService.Application.UnitTests.Helpers;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;
using PingAI.DialogManagementService.TestingUtil.AutoMoq;
using Xunit;
using PhrasePart = PingAI.DialogManagementService.Application.Queries.Shared.PhrasePart;
using Response = PingAI.DialogManagementService.Application.Queries.Shared.Response;

namespace PingAI.DialogManagementService.Application.UnitTests.Queries
{
    public class CreateQueryCommandHandlerTests
    {
        [Fact]
        public async Task CreateQuery()
        {
            var command = new CreateQueryCommand("q1", Guid.NewGuid(),
                new[]
                {
                    new PhrasePart(Guid.NewGuid(), PhrasePartType.TEXT, 0, "Hello", null, null)
                }, new Expression[0], new Response[]
                {
                    Response.FromText("rte response", 0)
                }, "test", null);
            Mock<INluService>? nluService = null;
            var sut = FixtureFactory.CreateSut<CreateQueryCommandHandler>(fixture =>
            {
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
