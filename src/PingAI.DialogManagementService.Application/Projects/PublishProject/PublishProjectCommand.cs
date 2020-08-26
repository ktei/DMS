using System;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Projects.PublishProject
{
    public class PublishProjectCommand : IRequest<Project>
    {
        public Guid ProjectId { get; set; }

        public PublishProjectCommand(Guid projectId)
        {
            ProjectId = projectId;
        }

        public PublishProjectCommand()
        {
            
        }
    }
}