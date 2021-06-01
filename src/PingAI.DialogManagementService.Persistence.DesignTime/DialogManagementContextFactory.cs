using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Moq;
using PingAI.DialogManagementService.Infrastructure.Persistence;

namespace PingAI.DialogManagementService.Persistence.DesignTime
{
    public class DialogManagementContextFactory : IDesignTimeDbContextFactory<DialogManagementContext>
    {
        public DialogManagementContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DialogManagementContext>();
            optionsBuilder.UseNpgsql(
                "Host=localhost;Database=postgres;Username=postgres;Password=admin");
            return new DialogManagementContext(optionsBuilder.Options, Mock.Of<IMediator>());
        }
    }
}
