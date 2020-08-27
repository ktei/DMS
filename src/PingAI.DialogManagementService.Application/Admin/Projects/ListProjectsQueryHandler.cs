using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Admin.Projects
{
    public class ListProjectsQueryHandler : IRequestHandler<ListProjectsQuery, List<Project>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectVersionRepository _projectVersionRepository;

        public ListProjectsQueryHandler(IProjectRepository projectRepository,
            IProjectVersionRepository projectVersionRepository)
        {
            _projectRepository = projectRepository;
            _projectVersionRepository = projectVersionRepository;
        }

        public async Task<List<Project>> Handle(ListProjectsQuery request, CancellationToken cancellationToken)
        {
            var projectVersions =
                await _projectVersionRepository.GetDesignTimeVersionsByOrganisationId(request.OrganisationId);

            if (!projectVersions.Any())
                return new List<Project>(0);

            var projects = await _projectRepository
                .GetProjectsByIds(projectVersions.Select(p => p.ProjectId));
            return projects;
        }
    }
}