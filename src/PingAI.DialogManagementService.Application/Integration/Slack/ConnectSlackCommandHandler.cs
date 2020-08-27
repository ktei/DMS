using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using PingAI.DialogManagementService.Application.Interfaces.Services.Slack;
using PingAI.DialogManagementService.Domain.ErrorHandling;

namespace PingAI.DialogManagementService.Application.Integration.Slack
{
    public class ConnectSlackCommandHandler : AsyncRequestHandler<ConnectSlackCommand>
    {
        private readonly ISlackService _slackService;

        public ConnectSlackCommandHandler(ISlackService slackService)
        {
            _slackService = slackService;
        }
        
        protected override async Task Handle(ConnectSlackCommand request, CancellationToken cancellationToken)
        {
            var (state, code) = ParseRedirectUri(request.RedirectUri);
            if (request.State != state)
            {
                throw new ForbiddenException("Invalid state");
            }
            
            var slackResponse = await _slackService.ExchangeOAuthCode(state, code);
            if (!slackResponse.Ok)
            {
                throw new BadRequestException($"Failed to connect to Slack: {slackResponse.Error}");
            }
            
            // TODO: save access token
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