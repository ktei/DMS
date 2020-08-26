using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Projects.ListProjects
{
    public class ListProjectsQueryHandler : IRequestHandler<ListProjectsQuery, List<Project>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectVersionRepository _projectVersionRepository;
        private readonly IRequestContext _requestContext;

        public ListProjectsQueryHandler(IProjectRepository projectRepository,
            IProjectVersionRepository projectVersionRepository, IRequestContext requestContext)
        {
            _projectRepository = projectRepository;
            _projectVersionRepository = projectVersionRepository;
            _requestContext = requestContext;
        }

        public async Task<List<Project>> Handle(ListProjectsQuery request, CancellationToken cancellationToken)
        {
            Guid? organisationId = null;
            var user = await _requestContext.GetUser();
            if (user.OrganisationIds.Any())
            {
                // TODO: what if user has multiple organisations?
                organisationId = user.OrganisationIds.First();
            }

            // no organisation found, no project to query
            if (!organisationId.HasValue)
                return new List<Project>(0);

            var projectVersions =
                await _projectVersionRepository.GetDesignTimeVersionsByOrganisationId(organisationId.Value);

            if (!projectVersions.Any())
                return new List<Project>(0);

            var projects = await _projectRepository.GetProjectsByIds(projectVersions.Select(p => p.ProjectId));
            return projects;
        }
    }
}