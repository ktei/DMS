using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Projects
{
    public class UpdateProjectResponse : ProjectDto
    {
        public UpdateProjectResponse(Project project) : base(project)
        {
            
        }

        public UpdateProjectResponse()
        {
            
        }
    }
}