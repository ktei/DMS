using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Domain.ErrorHandling;

namespace PingAI.DialogManagementService.Application.Projects.GetIntegration
{
    public class GetIntegrationQueryHandler : IRequestHandler<GetIntegrationQuery, IntegrationResult>
    {
        private readonly ISlackWorkspaceRepository _slackWorkspaceRepository;
        private readonly IAuthorizationService _authorizationService;

        public GetIntegrationQueryHandler(ISlackWorkspaceRepository slackWorkspaceRepository,
            IAuthorizationService authorizationService)
        {
            _slackWorkspaceRepository = slackWorkspaceRepository;
            _authorizationService = authorizationService;
        }

        public async Task<IntegrationResult> Handle(GetIntegrationQuery request, CancellationToken cancellationToken)
        {
            var canRead = await _authorizationService.UserCanReadProject(request.ProjectId);
            if (!canRead)
                throw new ForbiddenException(ErrorDescriptions.ProjectReadDenied);

            var slackWorkspace = await _slackWorkspaceRepository.GetSlackWorkspaceByProjectId(request.ProjectId);
            return new IntegrationResult
            {
                SlackWorkspace = slackWorkspace
            };
        }
    }
}