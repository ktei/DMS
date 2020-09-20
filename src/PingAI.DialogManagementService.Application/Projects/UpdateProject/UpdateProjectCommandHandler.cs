using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Projects.UpdateProject
{
    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Project>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authorizationService;

        public UpdateProjectCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork,
            IAuthorizationService authorizationService)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _authorizationService = authorizationService;
        }

        public async Task<Project> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var canWrite = await _authorizationService.UserCanWriteProject(request.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ProjectWriteDenied);
            var project = await _projectRepository.GetProjectById(request.ProjectId);
            if (project == null)
            {
                // TODO: this should never happen
                // consider adding logs here
                throw new ForbiddenException(ProjectReadDenied);
            }
            project.UpdateWidgetTitle(request.WidgetTitle);
            project.UpdateWidgetColor(request.WidgetColor);
            project.UpdateWidgetDescription(request.WidgetDescription);
            project.UpdateFallbackMessage(request.FallbackMessage);
            project.UpdateGreetingMessage(request.GreetingMessage);
            project.UpdateDomains(request.Domains);
            if (request.BusinessTimeStartUtc.HasValue)
            {
                if (!request.BusinessTimeEndUtc.HasValue)
                    throw new BadRequestException($"{nameof(request.BusinessTimeEndUtc)} cannot be null, " +
                                                  $"given {nameof(request.BusinessTimeStartUtc)} is not null");
                project.UpdateBusinessHours(request.BusinessTimeStartUtc.Value,
                    request.BusinessTimeEndUtc.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.BusinessEmail))
            {
                project.UpdateBusinessEmail(request.BusinessEmail!);
            }

            await _unitOfWork.SaveChanges();
            return project;
        }
    }
}