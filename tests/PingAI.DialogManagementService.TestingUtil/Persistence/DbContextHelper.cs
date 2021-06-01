using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
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
    }
}
