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
        private readonly IAuthorizationService _authorizationService;
        private readonly IRequestContext _requestContext;

        public ListProjectsQueryHandler(IProjectRepository projectRepository,
            IProjectVersionRepository projectVersionRepository, IRequestContext requestContext,
            IAuthorizationService authorizationService)
        {
            _projectRepository = projectRepository;
            _projectVersionRepository = projectVersionRepository;
            _requestContext = requestContext;
            _authorizationService = authorizationService;
        }

        public async Task<List<Project>> Handle(ListProjectsQuery request, CancellationToken cancellationToken)
        {
            var isAdmin = await _authorizationService.HasAdminPrivilege();
            List<ProjectVersion> projectVersions;
            if (isAdmin)
            {
                projectVersions = await _projectVersionRepository.GetDesignTimeVersions();
            }
            else
            {
                Organisation? organisation = null;
                var user = await _requestContext.GetUser();
                if (user.Organisations.Any())
                {
                    // TODO: what if user has multiple organisations?
                    organisation = user.Organisations.First();
                }

                // no organisation found, no project to query
                if (organisation == null)
                    return new List<Project>(0);

                projectVersions =
                    await _projectVersionRepository.GetDesignTimeVersionsByOrganisationId(organisation.Id);
            }

            if (!projectVersions.Any())
                return new List<Project>(0);

            var projects = await _projectRepository.GetProjectsByIds(
                projectVersions.Select(p => p.ProjectId));
            return projects;
        }
    }
}