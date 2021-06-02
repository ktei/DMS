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
        
        public Organisation Organisation { get; private set; }
        
        public User User { get; private set; }

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
                    
                    Organisation = new Organisation(nameof(Organisation), nameof(Organisation));
                    var project = new Project("Seeded Project",
                        Organisation.Id, Defaults.WidgetTitle,
                        Defaults.WidgetColor, Defaults.WidgetDescription,
                        Defaults.FallbackMessage, null, null,
                        Defaults.BusinessTimezone, Defaults.BusinessTimeStartUtc,
                        Defaults.BusinessTimeEndUtc, null);
                    Organisation.AddProject(project);
                    context.Add(Organisation);
                    var projectVersion = new ProjectVersion(project.Id,
                        Organisation.Id, project.Id,
                        ProjectVersionNumber.NewDesignTime());
                    Organisation.AddProjectVersion(projectVersion);
                    context.AddRange(projectVersion);

                    User = new User("SEEDED_USER", "AUTH0_ID");
                    Organisation.AddUser(User);
                    context.Add(User);
                    
                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        }

        public void Dispose() => Connection.Dispose();
    }
}
