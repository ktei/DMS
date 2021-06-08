using System.IO;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Npgsql;
using PingAI.DialogManagementService.Infrastructure.Persistence;

namespace PingAI.DialogManagementService.TestingUtil.Persistence
{
    internal static class DbContextHelper
    {
        public static DialogManagementContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DialogManagementContext>();
            optionsBuilder.UseNpgsql(
                "Host=localhost;Database=postgres;Username=postgres;Password=admin");
            return new DialogManagementContext(optionsBuilder.Options, Mock.Of<IMediator>());
        }

        public static void EnsureSchemaDeleted(this DialogManagementContext context)
        {
            context.Database.ExecuteSqlRaw(@"DROP SCHEMA chatbot CASCADE");
        }
        
        public static void EnsureSchemaCreated(this DialogManagementContext context)
        {
            var script = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(),
                "Persistence", "Scripts", "create_schema.sql"));
            context.Database.ExecuteSqlRaw(script);
            var conn = context.Database.GetDbConnection();
            conn.Open();
            ((NpgsqlConnection)conn).ReloadTypes();
        }
    }
}
