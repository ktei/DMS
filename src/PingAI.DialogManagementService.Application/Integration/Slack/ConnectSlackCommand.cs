using System;
using MediatR;

namespace PingAI.DialogManagementService.Application.Integration.Slack
{
    public class ConnectSlackCommand : IRequest
    {
        public Guid ProjectId { get; set; }
        public string State { get; set; }
        public string RedirectUri { get; set; }

        public ConnectSlackCommand(Guid projectId, string state, string redirectUri)
        {
            ProjectId = projectId;
            State = state;
            RedirectUri = redirectUri;
        }

        public ConnectSlackCommand()
        {
            
        }
    }
}