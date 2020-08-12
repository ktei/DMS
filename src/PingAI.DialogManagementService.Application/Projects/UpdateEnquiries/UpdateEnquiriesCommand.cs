using System;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Projects.UpdateEnquiries
{
    public class UpdateEnquiriesCommand : IRequest<Project>
    {
        public Guid ProjectId { get; set; }
        public string[] Enquiries { get; set; }

        public UpdateEnquiriesCommand(Guid projectId, string[] enquiries)
        {
            ProjectId = projectId;
            Enquiries = enquiries;
        }

        public UpdateEnquiriesCommand()
        {
            
        }
    }
}