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
                    var foodEntityType = new EntityType("Food", project.Id,
                        "description 1");
                    context.Add(foodEntityType);
                    context.AddRange(new EntityValue("pizza", foodEntityType.Id, null),
                        new EntityValue("Dumpling", foodEntityType.Id, null));
                    var cityEntityType = new EntityType("City", project.Id,
                        "description 2");
                    context.Add(cityEntityType);
                    context.AddRange(new EntityValue("Sydney", cityEntityType.Id, new[]{"SYD"}), 
                        new EntityValue("Melbourne", cityEntityType.Id, new[]{"MEL"}));
                    
                    // seed EntityNames
                    var deliveryOrderEntityName = new EntityName("deliveryOrder", project.Id, true);
                    var destinationEntityName = new EntityName("destination", project.Id, true);
                    context.AddRange(deliveryOrderEntityName, destinationEntityName);
                    
                    // seed Intents
                    var orderDeliveryIntent = new Intent("ORDER_FOOD", project.Id, IntentType.STANDARD);
                    context.Add(orderDeliveryIntent);
                    var orderDeliveryIntentPhraseId = Guid.NewGuid();
                    object[] orderDeliveryPhrase = {
                        new PhrasePart(orderDeliveryIntent.Id, orderDeliveryIntentPhraseId, 0, "Hello, I want to order some ",
                            null, PhrasePartType.TEXT, default(Guid?), default(Guid?), 0),
                        new PhrasePart(orderDeliveryIntent.Id, orderDeliveryIntentPhraseId, 1, "pizza",
                            null, PhrasePartType.ENTITY, deliveryOrderEntityName.Id, foodEntityType.Id, 0),
                    };
                    context.AddRange(orderDeliveryPhrase);

                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        }

        public void Dispose() => Connection.Dispose();
    }
}
