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
    public class ListProjectsQueryHandler : IRequestHandler<ListProjectsQuery, IReadOnlyList<Project>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IRequestContext _requestContext;

        public ListProjectsQueryHandler(IProjectRepository projectRepository,
            IRequestContext requestContext,
            IAuthorizationService authorizationService)
        {
            _projectRepository = projectRepository;
            _requestContext = requestContext;
            _authorizationService = authorizationService;
        }

        public async Task<IReadOnlyList<Project>> Handle(ListProjectsQuery request, CancellationToken cancellationToken)
        {
            var isAdmin = await _authorizationService.HasAdminPrivilege();
            IReadOnlyList<Project> projects;
            if (isAdmin)
            {
                projects = await _projectRepository.ListAll();
            }
            else
            {
                var user = await _requestContext.GetUser();
                if (user.Organisations.Any())
                {
                    // TODO: what if user has multiple organisations?
                    var organisation = user.Organisations.First();
                    projects = await _projectRepository.ListByOrganisationId(organisation.Id);
                }
                else
                {
                    throw new InvalidOperationException($"User {user.Id} does not belong to any organisation, " +
                                                        $"hence no project being found.");
                }
            }

            return projects;
        }
    }
}
