using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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
        
        private class MeidatR : IMediator
        {
            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
                CancellationToken cancellationToken = new CancellationToken()) =>
                Task.FromResult(default(TResponse));

            public Task<object?> Send(object request, CancellationToken cancellationToken = new CancellationToken()) =>
                Task.FromResult(default(object?));

            public Task Publish(object notification, CancellationToken cancellationToken = new CancellationToken()) =>
                Task.CompletedTask;

            public Task Publish<TNotification>(TNotification notification,
                CancellationToken cancellationToken = new CancellationToken()) where TNotification : INotification =>
                Task.FromResult(default(TNotification));
        }
    }
}