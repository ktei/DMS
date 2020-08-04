using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace PingAI.DialogManagementService.Infrastructure.Persistence
{
    public class DialogManagementContextFactory : IDesignTimeDbContextFactory<DialogManagementContext>
    {
        public DialogManagementContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DialogManagementContext>();
            optionsBuilder.UseNpgsql(
                "Host=localhost;Database=postgres;Username=postgres;Password=admin");
            return new DialogManagementContext(optionsBuilder.Options);
        }



    }
}