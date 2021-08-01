using System;
using System.Collections.Generic;
using System.Linq;
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
        public string? GreetingMessage { get; set; }
        public string[] QuickReplies { get; set; }
        public string[]? Domains { get; set; }
        public DateTime? BusinessTimeStartUtc { get; set; }
        public DateTime? BusinessTimeEndUtc { get; set; }
        public string? BusinessEmail { get; set; }

        public UpdateProjectCommand(Guid projectId, string widgetTitle, string widgetColor, string widgetDescription,
            string fallbackMessage, string? greetingMessage, IEnumerable<string> quickReplies,
            string[]? domains, DateTime? businessTimeStartUtc,
            DateTime? businessTimeEndUtc, string? businessEmail)
        {
            ProjectId = projectId;
            WidgetTitle = widgetTitle;
            WidgetColor = widgetColor;
            WidgetDescription = widgetDescription;
            FallbackMessage = fallbackMessage;
            GreetingMessage = greetingMessage;
            QuickReplies = (quickReplies ?? throw new ArgumentNullException(nameof(quickReplies))).ToArray();
            Domains = domains;
            BusinessTimeStartUtc = businessTimeStartUtc;
            BusinessTimeEndUtc = businessTimeEndUtc;
            BusinessEmail = businessEmail;
        }
    }
}