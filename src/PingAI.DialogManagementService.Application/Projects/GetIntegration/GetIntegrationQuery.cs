using System;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Projects.GetIntegration
{
    public class GetIntegrationQuery : IRequest<IntegrationResult>
    {
        public Guid ProjectId { get; set; }

        public GetIntegrationQuery(Guid projectId)
        {
            ProjectId = projectId;
        }

        public GetIntegrationQuery()
        {
            
        }
    }

    public class IntegrationResult
    {
        public SlackWorkspace? SlackWorkspace { get; set; }
    }
}