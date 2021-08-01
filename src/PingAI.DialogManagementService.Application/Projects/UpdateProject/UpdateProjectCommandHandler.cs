using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Application.Interfaces.Services.Caching;
using PingAI.DialogManagementService.Application.Interfaces.Services.Security;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Projects.UpdateProject
{
    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Project>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICacheService _cacheService;
        private readonly IResponseRepository _responseRepository;

        public UpdateProjectCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService, ICacheService cacheService,
            IResponseRepository responseRepository)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
            _cacheService = cacheService;
            _responseRepository = responseRepository;
        }

        public async Task<Project> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var canWrite = await _authorizationService.UserCanWriteProject(request.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ProjectWriteDenied);
            var project = await _projectRepository.FindById(request.ProjectId);
            if (project == null)
            {
                // TODO: this should never happen
                // consider adding logs here
                throw new NotFoundException(ProjectNotFound);
            }
            
            project.CustomiseWidget(
                request.WidgetTitle,
                request.WidgetColor,
                request.WidgetDescription);
            
            project.SetFallbackMessage(request.FallbackMessage);
            
            var responsesToRemove = new List<Response>();
            responsesToRemove.AddRange(project.SetGreetingMessage(request.GreetingMessage));
            responsesToRemove.AddRange(project.SetQuickReplies(request.QuickReplies));
            _responseRepository.RemoveRange(responsesToRemove);
            
            project.SetDomains(request.Domains);
            
            if (request.BusinessTimeStartUtc.HasValue)
            {
                if (!request.BusinessTimeEndUtc.HasValue)
                    throw new BadRequestException($"{nameof(request.BusinessTimeEndUtc)} cannot be null, " +
                                                  $"given {nameof(request.BusinessTimeStartUtc)} is not null");
                project.SetBusinessHours(request.BusinessTimeStartUtc.Value,
                    request.BusinessTimeEndUtc.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.BusinessEmail))
            {
                project.SetBusinessEmail(request.BusinessEmail!);
            }

            await _unitOfWork.SaveChanges();
            
            // update cache
            var cache = ProjectCache.FromProject(project);
            await _cacheService.SetObject(cache.GetKey(), cache);
            
            return project;
        }
    }
}