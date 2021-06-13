using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Admin.Projects
{
    public class GetRuntimeProjectQueryHandler : IRequestHandler<GetRuntimeProjectQuery, Project>
    {
        private readonly IProjectVersionRepository _projectVersionRepository;
        private readonly IProjectRepository _projectRepository;

        public GetRuntimeProjectQueryHandler(IProjectVersionRepository projectVersionRepository,
            IProjectRepository projectRepository)
        {
            _projectVersionRepository = projectVersionRepository;
            _projectRepository = projectRepository;
        }

        public async Task<Project> Handle(GetRuntimeProjectQuery request, CancellationToken cancellationToken)
        {
            var projectVersion = await _projectVersionRepository.FindLatestByProjectId(request.DesignTimeProjectId);
            if (projectVersion == null)
                throw new BadRequestException(ProjectNotFound);
            var project = await _projectRepository.FindById(projectVersion.ProjectId);
            if (project == null)
                throw new BadRequestException(ProjectNotFound);
            return project;
        }
    }
}