using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Projects
{
    public class UpdateEnquiriesResponse
    {
        public string ProjectId { get; set; }
        public string[] Enquiries { get; set; }

        public UpdateEnquiriesResponse(string projectId, string[] enquiries)
        {
            ProjectId = projectId;
            Enquiries = enquiries;
        }

        public UpdateEnquiriesResponse(Project project) : this(project.Id.ToString(), project.Enquiries)
        {
            
        }

        public UpdateEnquiriesResponse()
        {
            
        }
    }
}