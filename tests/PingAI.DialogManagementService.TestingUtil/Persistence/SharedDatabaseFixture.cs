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
        public const string DefaultProjectName = "TEST_DEFAULT";

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
                }).Options);

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
                    context.EnsureSchemaDeleted();
                    context.EnsureSchemaCreated();

                    // seed Organisations
                    var organisation = new Organisation(Guid.NewGuid().ToString(), "TEST");
                    
                    // seed Users
                    var user = new User(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                    organisation.AddUser(user);
                    
                    context.Add(organisation);
                    // seed Projects
                    var project = Project.CreateWithDefaults(organisation.Id, "TEST_DEFAULT");
                    project.SetGreetingMessage("Hello, and welcome!");
                    project.SetDomains(new[]{"https://test.com"});
                    project.SetBusinessEmail("test-email@test.com");
                    context.Add(project);
                    
                    // seed EntityTypes
                    var foodEntityType = new EntityType(project.Id, "Food", 
                        "description 1");
                    context.Add(foodEntityType);
                    context.AddRange(new EntityValue(foodEntityType.Id, "pizza", null),
                        new EntityValue(foodEntityType.Id, "Dumpling", null));
                    var cityEntityType = new EntityType(project.Id, "City", 
                        "description 2");
                    context.Add(cityEntityType);
                    context.AddRange(new EntityValue(cityEntityType.Id, "Sydney", new[]{"SYD"}), 
                        new EntityValue(cityEntityType.Id, "Melbourne", new[]{"MEL"}));
                    
                    // seed EntityNames
                    var deliveryOrderEntityName = new EntityName(project.Id, "deliveryOrder", true);
                    var destinationEntityName = new EntityName(project.Id, "destination", true);
                    context.AddRange(deliveryOrderEntityName, destinationEntityName);
                    
                    // seed Intents
                    var orderDeliveryIntent = new Intent(project.Id, "ORDER_FOOD",  IntentType.STANDARD);
                    context.Add(orderDeliveryIntent);
                    orderDeliveryIntent.AddPhrase(new Phrase(0)
                        .AppendText("Hello, I want to order some ")
                        .AppendEntity("pizza", deliveryOrderEntityName));
                    
                    // seed Responses
                    var orderDeliveryResponse = new Response(project.Id, Resolution.Factory.RteText("It's done!"),
                        ResponseType.RTE, 0);
                    
                    // seed Queries
                    var query = new Query(project.Id, "ORDER_FOOD_QUERY", new Expression[0], "SEEDED",
                        null, 0);
                    query.AddIntent(orderDeliveryIntent);
                    query.AddResponse(orderDeliveryResponse);
                    context.Add(query);

                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        }

        public void Dispose() => Connection.Dispose();
    }
}
