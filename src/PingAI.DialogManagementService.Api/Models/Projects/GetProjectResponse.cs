using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Projects
{
    public class GetProjectResponse : ProjectDto
    {
        public GetProjectResponse(Project project) : base(project)
        {
            
        }

        public GetProjectResponse()
        {
            
        }
    }
}