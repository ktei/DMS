using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
using PingAI.DialogManagementService.TestingUtil.Persistence;
using Xunit;

namespace PingAI.DialogManagementService.Infrastructure.UnitTests.Persistence.Repositories
{
    public class QueryRepositoryTests : RepositoryTestBase
    {
        public QueryRepositoryTests(SharedDatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task FindById()
        {
            var context = Fixture.CreateContext();
            var sut = new QueryRepository(context);
            var query = await context.Queries.FirstAsync();
            
            context.ChangeTracker.Clear();
            var actual = await sut.FindById(query.Id);

            actual.Should().NotBeNull();
        }

        [Theory]
        [InlineData(ResponseType.RTE, 2)]
        [InlineData(ResponseType.FORM, 1)]
        [InlineData(ResponseType.HANDOVER, 1)]
        [InlineData(null, 3)]
        public async Task ListByProjectId(ResponseType? filterByType, int expectedQueryCount)
        {
            var context = Fixture.CreateContext();
            var sut = new QueryRepository(context);
            var organisation = await context.Organisations.FirstAsync();
            var project = Project.CreateWithDefaults(organisation.Id);
            await context.AddAsync(project);
            project.StampVersion(ProjectVersionNumber.DesignTime);
            var rteQuery = new Query(project.Id, Guid.NewGuid().ToString(), new Expression[0],
                "whatever", null, 0);
            rteQuery.AddResponse(new Response(project.Id, Resolution.Factory.RteText("Hello world"), ResponseType.RTE, 0));
            var quickReplyQuery = new Query(project.Id, Guid.NewGuid().ToString(), new Expression[0],
                "whatever", null, 0);
            quickReplyQuery.AddResponse(new Response(project.Id, Resolution.Factory.RteText("quick reply"), ResponseType.QUICK_REPLY, 1));
            var formQuery = new Query(project.Id, Guid.NewGuid().ToString(), new Expression[0],
                "whatever", null, 0);
            formQuery.AddResponse(new Response(project.Id, Resolution.Factory.Form(new FormResolution(new FormField[]
            {
                new FormField("Name", "name")
            })), ResponseType.FORM, 2));
            var handoverQuery = new Query(project.Id, Guid.NewGuid().ToString(), new Expression[0],
                "whatever", null, 0);
            handoverQuery.AddResponse(new Response(project.Id, Resolution.Factory.RteText("handover"), ResponseType.HANDOVER, 3));
            await context.AddRangeAsync(rteQuery, quickReplyQuery, formQuery, handoverQuery);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();
            var actual = await sut.ListByProjectId(project.Id, filterByType);

            actual.Should().HaveCountGreaterOrEqualTo(expectedQueryCount);
        }

        [Fact]
        public async Task ListByProjectIdWithoutJoins()
        {
            var context = Fixture.CreateContext();
            var sut = new QueryRepository(context);
            var project = await context.Projects.FirstAsync();

            context.ChangeTracker.Clear();
            var actual = await sut.ListByProjectIdWithoutJoins(project.Id);

            actual.Should().HaveCountGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task Remove()
        {
            var context = Fixture.CreateContext();
            var sut = new QueryRepository(context);
            var project = await context.Projects.FirstAsync();
            var query = new Query(project.Id, Guid.NewGuid().ToString(), new Expression[0],
                Guid.NewGuid().ToString(), null, 40);
            await context.AddAsync(query);
            
            sut.Remove(query);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            var actual = await context.Queries.AnyAsync(q => q.Id == query.Id);

            actual.Should().BeFalse();
        }

        [Fact]
        public async Task GetMaxDisplayOrder()
        {
            var context = Fixture.CreateContext();
            var sut = new QueryRepository(context);
            var project = await context.Projects.FirstAsync();
            var q1 = new Query(project.Id, Guid.NewGuid().ToString(), new Expression[0],
                Guid.NewGuid().ToString(), null, 40);
            var q2 = new Query(project.Id, Guid.NewGuid().ToString(), new Expression[0],
                Guid.NewGuid().ToString(), null, 39);
            var q3 = new Query(project.Id, Guid.NewGuid().ToString(), new Expression[0],
                Guid.NewGuid().ToString(), null, 21);
            await context.AddRangeAsync(q1, q2, q3);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();
            var actual = await sut.GetMaxDisplayOrder(project.Id);

            actual.Should().Be(40);
        }
    }
}
