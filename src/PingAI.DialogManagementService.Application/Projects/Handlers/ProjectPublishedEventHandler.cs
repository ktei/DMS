using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Services.Nlu;
using PingAI.DialogManagementService.Application.Utils;
using PingAI.DialogManagementService.Domain.Events;

namespace PingAI.DialogManagementService.Application.Projects.Handlers
{
    public class ProjectPublishedEventHandler : INotificationHandler<ProjectPublishedEvent>
    {
        private readonly INluService _nluService;

        public ProjectPublishedEventHandler(INluService nluService)
        {
            _nluService = nluService;
        }

        public async Task Handle(ProjectPublishedEvent notification, CancellationToken cancellationToken)
        {
            await PerformanceLogger.Monitor(() => _nluService.Export(notification.SourceProject.Id,
                notification.PublishedProject.Id), "NluService.Export");
            
            // We just exported source project's data and saved that using
            // published project's ID. Next, 
            await PerformanceLogger.Monitor(
                () => _nluService.Import(notification.PublishedProject.Id, 
                    notification.SourceProject.Id),
                "NluService.Import");
        }
    }
}
