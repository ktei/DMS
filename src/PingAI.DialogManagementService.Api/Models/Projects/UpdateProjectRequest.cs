namespace PingAI.DialogManagementService.Api.Models.Projects
{
    public class UpdateProjectRequest
    {
        public string ProjectId { get; set; }
        public string WidgetTitle { get; set; }
        public string WidgetColor { get; set; }
        public string WidgetDescription { get; set; }
        public string FallbackMessage { get; set; }
        public string GreetingMessage { get; set; }
    }
}