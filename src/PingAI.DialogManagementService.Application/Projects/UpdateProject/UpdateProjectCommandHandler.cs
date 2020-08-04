using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Projects.UpdateProject
{
    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Project>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;

        public UpdateProjectCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork,
            IAuthService authService)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _authService = authService;
        }

        public async Task<Project> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetProjectById(request.ProjectId);
            if (project == null) throw new ForbiddenException($"Project {request.ProjectId} cannot be accessed.");
            var canWrite = await _authService.UserCanWriteProject(project);
            if (!canWrite)
                throw new ForbiddenException($"Sorry, you have no permission to write to project {request.ProjectId}");
            project.UpdateWidgetTitle(request.WidgetTitle);
            project.UpdateWidgetColor(request.WidgetColor);
            project.UpdateWidgetDescription(request.WidgetDescription);
            project.UpdateFallbackMessage(request.FallbackMessage);
            project.UpdateGreetingMessage(request.GreetingMessage);
            project.UpdateEnquiries(request.Enquiries);
            await _unitOfWork.SaveChanges();
            return project;
        }
    }
}