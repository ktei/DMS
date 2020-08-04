using System;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Projects.GetProject
{
    public class GetProjectQuery : IRequest<Project>
    {
        public Guid ProjectId { get; set; }

        public GetProjectQuery(Guid projectId)
        {
            ProjectId = projectId;
        }

        public GetProjectQuery()
        {
            
        }
    }
}