using System;
using MediatR;

namespace PingAI.DialogManagementService.Application.Projects.GetIntegration
{
    public class GetIntegrationQuery : IRequest<Integration>
    {
        public Guid ProjectId { get; }

        public GetIntegrationQuery(Guid projectId)
        {
            ProjectId = projectId;
        }
    }
}