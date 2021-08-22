namespace PingAI.DialogManagementService.Api.Models.Projects
{
    public class UpdateProjectRequest
    {
        public string WidgetTitle { get; set; }
        public string WidgetColor { get; set; }
        public string WidgetDescription { get; set; }
        public string FallbackMessage { get; set; }
        public string? GreetingMessage { get; set; }
        public string[]? QuickReplies { get; set; }
        public string[]? Domains { get; set; }
        public string? BusinessTimeStart { get; set; }
        public string? BusinessTimeEnd { get; set; }
        public string? BusinessEmail { get; set; }
    }
}