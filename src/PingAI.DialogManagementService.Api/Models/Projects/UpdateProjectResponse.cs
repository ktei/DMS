using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Projects
{
    public class UpdateProjectResponse
    {
        public string ProjectId { get; set; }
        public string WidgetTitle { get; set; }
        public string WidgetColor { get; set; }
        public string WidgetDescription { get; set; }
        public string FallbackMessage { get; set; }
        public string GreetingMessage { get; set; }
        public string[] Enquiries { get; set; }

        public UpdateProjectResponse()
        {
            
        }

        public UpdateProjectResponse(Project project)
        {
            ProjectId = project.Id.ToString();
            WidgetTitle = project.WidgetTitle;
            WidgetColor = project.WidgetColor;
            WidgetDescription = project.WidgetDescription;
            FallbackMessage = project.FallbackMessage;
            GreetingMessage = project.GreetingMessage;
            Enquiries = project.Enquiries ?? new string[0];
        }
    }
}