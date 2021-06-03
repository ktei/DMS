using System;
using System.Data.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Npgsql;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Infrastructure.Persistence;

namespace PingAI.DialogManagementService.TestingUtil.Persistence
{
    public class SharedDatabaseFixture : IDisposable
    {
        private static readonly object Lock = new object();
        private static bool _databaseInitialized;

        public DbConnection Connection { get; }
        
        public SharedDatabaseFixture()
        {
            Connection = new NpgsqlConnection("Host=localhost;Database=postgres;Username=postgres;Password=admin");

            Seed();
        }

        public DialogManagementContext CreateContext(DbTransaction? transaction = null)
        {
            var context = new DialogManagementContext(new DbContextOptionsBuilder<DialogManagementContext>()
                .UseNpgsql(Connection, options =>
                {
                    options.UseAdminDatabase("rui");
                }).Options, Mock.Of<IMediator>());

            if (transaction != null)
            {
                context.Database.UseTransaction(transaction);
            }

            return context;
        }

        private void Seed()
        {
            lock (Lock)
            {
                if (_databaseInitialized) return;

                using (var context = CreateContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                   
                    // seed Organisations
                    var organisation = new Organisation("Seeded Organisation", "TEST");
                    context.Add(organisation);
                    
                    // seed Projects
                    var project = new Project("Seeded Project",
                        organisation.Id, Defaults.WidgetTitle,
                        Defaults.WidgetColor, Defaults.WidgetDescription,
                        Defaults.FallbackMessage, null, null,
                        Defaults.BusinessTimezone, Defaults.BusinessTimeStartUtc,
                        Defaults.BusinessTimeEndUtc, null);
                    context.Add(project);
                    
                    // seed ProjectVersions
                    var projectVersion = new ProjectVersion(project.Id,
                        organisation.Id, project.Id,
                        ProjectVersionNumber.NewDesignTime());
                    context.Add(projectVersion);

                    // seed Users
                    var user = new User("SEEDED_USER", "AUTH0_ID");
                    context.Add(user);
                    context.Add(new OrganisationUser(organisation.Id, user.Id));

                    // seed EntityTypes
                    var entityType1 = new EntityType("SEEDED_ENTITY_TYPE1", project.Id,
                        "description 1");
                    context.Add(entityType1);
                    var entityType1Value1 = new EntityValue("v1", entityType1.Id, null);
                    var entityType1Value2 = new EntityValue("v2", entityType1.Id, null);
                    context.AddRange(entityType1Value1, entityType1Value2);
                    var entityType2 = new EntityType("SEEDED_ENTITY_TYPE2", project.Id,
                        "description 2");
                    context.Add(entityType2);
                    var entityType2Value1 = new EntityValue("v1", entityType2.Id, null);
                    var entityType2Value2 = new EntityValue("v2", entityType2.Id, null);
                    context.AddRange(entityType2Value1, entityType2Value2);
                    
                    // seed EntityNames
                    var entityName1 = new EntityName("SEEDED_ENTITY_NAME1", project.Id, true);
                    var entityName2 = new EntityName("SEEDED_ENTITY_NAME2", project.Id, true);
                    context.AddRange(entityName1, entityName2);

                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        }

        public void Dispose() => Connection.Dispose();
    }
}
