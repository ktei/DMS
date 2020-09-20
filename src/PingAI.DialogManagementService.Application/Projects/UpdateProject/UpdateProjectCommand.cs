using System;
using MediatR;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Application.Projects.UpdateProject
{
    public class UpdateProjectCommand : IRequest<Project>
    {
        public Guid ProjectId { get; set; }
        public string WidgetTitle { get; set; }
        public string WidgetColor { get; set; }
        public string WidgetDescription { get; set; }
        public string FallbackMessage { get; set; }
        public string GreetingMessage { get; set; }
        public string[]? Domains { get; set; }
        public DateTime? BusinessTimeStartUtc { get; set; }
        public DateTime? BusinessTimeEndUtc { get; set; }
        public string? BusinessEmail { get; set; }

        public UpdateProjectCommand(Guid projectId, string widgetTitle, string widgetColor, string widgetDescription,
            string fallbackMessage, string greetingMessage, string[]? domains, DateTime? businessTimeStartUtc,
            DateTime? businessTimeEndUtc, string? businessEmail)
        {
            ProjectId = projectId;
            WidgetTitle = widgetTitle;
            WidgetColor = widgetColor;
            WidgetDescription = widgetDescription;
            FallbackMessage = fallbackMessage;
            GreetingMessage = greetingMessage;
            Domains = domains;
            BusinessTimeStartUtc = businessTimeStartUtc;
            BusinessTimeEndUtc = businessTimeEndUtc;
            BusinessEmail = businessEmail;
        }
    }
}