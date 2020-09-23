using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Services.Caching;
using PingAI.DialogManagementService.Domain.Events;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Projects.Handlers
{
    public class ProjectUpdatedEventHandler : INotificationHandler<ProjectUpdatedEvent>
    {
        private readonly ICacheService _cacheService;

        public ProjectUpdatedEventHandler(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }
        
        public Task Handle(ProjectUpdatedEvent notification, CancellationToken cancellationToken)
        {
            var project = notification.Project;
            return _cacheService.SetObject(Project.Cache.MakeKey(project.Id), new Project.Cache(project));
        }
    }
}