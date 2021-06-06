using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Admin.Projects
{
    public class ListProjectsQueryHandler : IRequestHandler<ListProjectsQuery, IReadOnlyList<Project>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectVersionRepository _projectVersionRepository;

        public ListProjectsQueryHandler(IProjectRepository projectRepository,
            IProjectVersionRepository projectVersionRepository)
        {
            _projectRepository = projectRepository;
            _projectVersionRepository = projectVersionRepository;
        }

        public async Task<IReadOnlyList<Project>> Handle(ListProjectsQuery request, CancellationToken cancellationToken)
        {
            if (request.OrganisationId.HasValue)
                return await _projectRepository.ListByOrganisationId(request.OrganisationId.Value);

            return await _projectRepository.ListAll();
        }
    }
}
