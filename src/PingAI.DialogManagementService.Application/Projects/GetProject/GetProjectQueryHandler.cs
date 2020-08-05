using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Projects.GetProject
{
    public class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, Project>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IAuthService _authService;

        public GetProjectQueryHandler(IProjectRepository projectRepository, IAuthService authService)
        {
            _projectRepository = projectRepository;
            _authService = authService;
        }

        public async Task<Project> Handle(GetProjectQuery request, CancellationToken cancellationToken)
        {
            var canRead = await _authService.UserCanReadProject(request.ProjectId);
            if (!canRead)
                throw new ForbiddenException($"Project {request.ProjectId} cannot be accessed");
            
            var project = await _projectRepository.GetProjectById(request.ProjectId);
            if (project == null)
            {
                throw new ForbiddenException($"Project {request.ProjectId} cannot be accessed");
            }

            return project;
        }
    }
}