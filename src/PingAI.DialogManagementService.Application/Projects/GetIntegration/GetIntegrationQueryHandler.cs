using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Services.Security;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Repositories;

namespace PingAI.DialogManagementService.Application.Projects.GetIntegration
{
    public class GetIntegrationQueryHandler : IRequestHandler<GetIntegrationQuery, Integration>
    {
        private readonly ISlackWorkspaceRepository _slackWorkspaceRepository;
        private readonly IAuthorizationService _authorizationService;

        public GetIntegrationQueryHandler(ISlackWorkspaceRepository slackWorkspaceRepository,
            IAuthorizationService authorizationService)
        {
            _slackWorkspaceRepository = slackWorkspaceRepository;
            _authorizationService = authorizationService;
        }

        public async Task<Integration> Handle(GetIntegrationQuery request, CancellationToken cancellationToken)
        {
            var canRead = await _authorizationService.UserCanReadProject(request.ProjectId);
            if (!canRead)
                throw new ForbiddenException(ErrorDescriptions.ProjectReadDenied);

            var slackWorkspace = await _slackWorkspaceRepository.FindByProjectId(request.ProjectId);
            return new Integration
            {
                SlackWorkspace = slackWorkspace
            };
        }
    }
}