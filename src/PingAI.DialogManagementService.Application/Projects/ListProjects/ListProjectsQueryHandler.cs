using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Application.Interfaces.Services.Security;
using PingAI.DialogManagementService.Domain.Model;
using PingAI.DialogManagementService.Domain.Repositories;

namespace PingAI.DialogManagementService.Application.Projects.ListProjects
{
    public class ListProjectsQueryHandler : IRequestHandler<ListProjectsQuery, IReadOnlyList<Project>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IIdentityContext _identityContext;

        public ListProjectsQueryHandler(IProjectRepository projectRepository,
            IIdentityContext identityContext,
            IAuthorizationService authorizationService)
        {
            _projectRepository = projectRepository;
            _identityContext = identityContext;
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
                var user = await _identityContext.GetUser();
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
