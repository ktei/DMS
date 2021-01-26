using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Application.Interfaces.Services.Slack;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Domain.Model;
using static PingAI.DialogManagementService.Domain.ErrorHandling.ErrorDescriptions;

namespace PingAI.DialogManagementService.Application.Integration.Slack
{
    public class ConnectSlackCommandHandler : IRequestHandler<ConnectSlackCommand, SlackWorkspace>
    {
        private readonly ISlackService _slackService;
        private readonly ISlackWorkspaceRepository _slackWorkspaceRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUnitOfWork _uow;

        public ConnectSlackCommandHandler(ISlackService slackService,
            ISlackWorkspaceRepository slackWorkspaceRepository, IAuthorizationService authorizationService,
            IUnitOfWork uow)
        {
            _slackService = slackService;
            _slackWorkspaceRepository = slackWorkspaceRepository;
            _authorizationService = authorizationService;
            _uow = uow;
        }

        public async Task<SlackWorkspace> Handle(ConnectSlackCommand request, CancellationToken cancellationToken)
        {
            var canWrite = await _authorizationService.UserCanWriteProject(request.ProjectId);
            if (!canWrite)
                throw new ForbiddenException(ProjectWriteDenied);
            
            // var (state, code) = ParseRedirectUri(request.RedirectUri);
            // if (request.State != state)
            // {
            //     throw new ForbiddenException("Invalid state");
            // }
            
            var slackResponse = await _slackService.ExchangeOAuthCode(request.Code, request.RedirectUri);
            if (!slackResponse.Ok)
            {
                throw new BadRequestException($"Failed to connect to Slack: {slackResponse.Error}");
            }

            var slackWorkspace = await _slackWorkspaceRepository.GetSlackWorkspaceByProjectId(request.ProjectId);
            if (slackWorkspace == null)
            {
                slackWorkspace = await _slackWorkspaceRepository.AddSlackWorkspace(
                    new SlackWorkspace(request.ProjectId, slackResponse.AccessToken!,
                        slackResponse.IncomingWebhook!.Url,
                        slackResponse.TeamId));
            }
            else
            {
                slackWorkspace.UpdateSlackConfig(
                    slackResponse.AccessToken!,
                    slackResponse.IncomingWebhook!.Url,
                    slackResponse.TeamId);
            }

            await _uow.SaveChanges();

            return slackWorkspace;
        }

        private static (string State, string Code) ParseRedirectUri(string redirectUri)
        {
            var uri = new Uri(redirectUri);
            var parseResult = HttpUtility.ParseQueryString(uri.Query);
            var state = parseResult.Get("state");
            var code = parseResult.Get("code");

            return (state, code);
        }
    }
}