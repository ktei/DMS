using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Queries.CreateQuery;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.TestingUtil.AutoMoq;
using Xunit;
using PhrasePart = PingAI.DialogManagementService.Application.Queries.CreateQuery.PhrasePart;
using Response = PingAI.DialogManagementService.Application.Queries.CreateQuery.Response;

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
                    new Response("rte response", null, 0)
                }, "test", null);
            var sut = FixtureFactory.CreateSut<CreateQueryCommandHandler>(fixture =>
            {
                fixture.InjectMock<IEntityNameRepository>(m => m.Setup(x => x.ListByProjectId(It.IsAny<Guid>()))
                    .ReturnsAsync(new EntityName[0]));
            });
            
            var query = await sut.Handle(command, CancellationToken.None);

            query.Should().NotBeNull();
        }
    }
}
