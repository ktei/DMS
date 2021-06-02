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
            
            var actual = await sut.FindByName(Fixture.Organisation.Name);
            
            actual.Should().NotBeNull();
            actual!.Name.Should().Be(Fixture.Organisation.Name);
        }
       
        [Fact]
        public async Task FindById()
        {
            var context = Fixture.CreateContext();
            var sut = new OrganisationRepository(context);

            var actual = await sut.FindById(Fixture.Organisation.Id);

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
            context.Attach(Fixture.User);
            organisation.AddUser(Fixture.User);
            await context.AddAsync(organisation);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();
            var actual = await sut.ListByUserId(Fixture.User.Id);

            actual.Should().HaveCount(2);
        }
    }
}
