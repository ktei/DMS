using System;
using System.Collections.Generic;
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
        public IEnumerable<Response> GreetingResponses { get; set; }
        public string[]? Domains { get; set; }
        public DateTime? BusinessTimeStartUtc { get; set; }
        public DateTime? BusinessTimeEndUtc { get; set; }
        public string? BusinessEmail { get; set; }

        public UpdateProjectCommand(Guid projectId, string widgetTitle, string widgetColor, string widgetDescription,
            string fallbackMessage, IEnumerable<Response> greetingResponses,
            string[]? domains, DateTime? businessTimeStartUtc,
            DateTime? businessTimeEndUtc, string? businessEmail)
        {
            ProjectId = projectId;
            WidgetTitle = widgetTitle;
            WidgetColor = widgetColor;
            WidgetDescription = widgetDescription;
            FallbackMessage = fallbackMessage;
            GreetingResponses = greetingResponses;
            Domains = domains;
            BusinessTimeStartUtc = businessTimeStartUtc;
            BusinessTimeEndUtc = businessTimeEndUtc;
            BusinessEmail = businessEmail;
        }
    }
}