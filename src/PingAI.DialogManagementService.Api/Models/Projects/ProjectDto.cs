using System;
using System.Linq;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Projects
{
    public class ProjectDto
    {
        public string ProjectId { get; set; }
        public string? WidgetTitle { get; set; }
        public string WidgetColor { get; set; }
        public string? WidgetDescription { get; set; }
        public string? FallbackMessage { get; set; }
        public string? GreetingMessage { get; set; }
        public string[] Enquiries { get; set; }
        public string? ApiKey { get; set; }
        public string[] Domains { get; set; }

        public ProjectDto(string projectId, string? widgetTitle, string widgetColor, string? widgetDescription,
            string? fallbackMessage, string? greetingMessage, string[] enquiries, string? apiKey, string[] domains)
        {
            ProjectId = projectId;
            WidgetTitle = widgetTitle;
            WidgetColor = widgetColor;
            WidgetDescription = widgetDescription;
            FallbackMessage = fallbackMessage;
            GreetingMessage = greetingMessage;
            Enquiries = enquiries;
            ApiKey = apiKey;
            Domains = domains;
        }
        
        public ProjectDto(Project project)
        {
            _ = project ?? throw new ArgumentNullException(nameof(project));
            ProjectId = project.Id.ToString();
            WidgetTitle = project.WidgetTitle;
            WidgetColor = project.WidgetColor ?? string.Empty;
            WidgetDescription = project.WidgetDescription;
            FallbackMessage = project.FallbackMessage;
            GreetingMessage = project.GreetingMessage;
            Enquiries = project.Enquiries ?? new string[0];
            ApiKey = project.ApiKey?.Key;
            Domains = project.Domains ?? new string[0];
        }

        public ProjectDto()
        {
            
        }
    }
}