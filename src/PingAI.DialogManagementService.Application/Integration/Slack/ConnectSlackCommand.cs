using System;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Integration.Slack
{
    public class ConnectSlackCommand : IRequest<SlackWorkspace>
    {
        public Guid ProjectId { get; set; }
        public string State { get; set; }
        public string Code { get; set; }
        public string RedirectUri { get; set; }

        public ConnectSlackCommand(Guid projectId, string code, string state, string redirectUri)
        {
            ProjectId = projectId;
            Code = code;
            State = state;
            RedirectUri = redirectUri;
        }

        public ConnectSlackCommand()
        {
            
        }
    }
}