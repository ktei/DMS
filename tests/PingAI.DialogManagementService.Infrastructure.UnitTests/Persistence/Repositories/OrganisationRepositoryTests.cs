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
    public class OrganisationRepositoryTests : RepositoryTestBase
    {
        public OrganisationRepositoryTests(SharedDatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task FindByName()
        {
            var context = Fixture.CreateContext();
            var sut = new OrganisationRepository(context);
            var organisation = await context.Organisations.FirstAsync();

            var actual = await sut.FindByName(organisation.Name);
            
            actual.Should().NotBeNull();
            actual!.Name.Should().Be(organisation.Name);
        }
       
        [Fact]
        public async Task FindById()
        {
            var context = Fixture.CreateContext();
            var sut = new OrganisationRepository(context);
            var organisation = await context.Organisations.FirstAsync();

            var actual = await sut.FindById(organisation.Id);

            actual.Should().NotBeNull();
        }

        [Fact]
        public async Task Add()
        {
            var context = Fixture.CreateContext();
            var sut = new OrganisationRepository(context);
            var organisation = new Organisation(Guid.NewGuid().ToString(), "test");

            await sut.Add(organisation);
            await context.SaveChangesAsync();
            
            context.ChangeTracker.Clear();
            var actual = await context.Organisations.FirstOrDefaultAsync(x => x.Id == organisation.Id);
            actual.Should().NotBeNull();
        }

        [Fact]
        public async Task ListAll()
        {
            var context = Fixture.CreateContext();
            var sut = new OrganisationRepository(context);
            var organisation = new Organisation(Guid.NewGuid().ToString(), "test");
            await context.AddAsync(organisation);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();
            var actual = await sut.ListAll();
            actual.Should().HaveCountGreaterOrEqualTo(2);
        }

        [Fact]
        public async Task ListByUserId()
        {
            var context = Fixture.CreateContext();
            var sut = new OrganisationRepository(context);
            var organisation = new Organisation(Guid.NewGuid().ToString(), "test");
            var user = await context.Users.FirstAsync();
            await context.AddAsync(organisation);
            await context.AddAsync(new OrganisationUser(organisation.Id, user.Id));
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();
            var actual = await sut.ListByUserId(user.Id);

            actual.Should().HaveCount(2);
        }
    }
}
