using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Services.Nlu;
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
            await _nluService.Export(notification.SourceProject.Id, notification.PublishedProject.Id);
        }
    }
}