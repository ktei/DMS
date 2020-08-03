using System;
using System.Linq;
using PingAI.DialogManagementService.Domain.ErrorHandling;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class Project : IHaveTimestamps
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Guid OrganisationId { get; private set; }
        public Organisation? Organisation { get; private set; } = null;
        public string? WidgetTitle { get; private set; }

        private string? _widgetColor;
        public string? WidgetColor
        {
            get => (_widgetColor ?? string.Empty).TrimEnd();
            private set => _widgetColor = value;
        }
        
        public string? WidgetDescription { get; private set; }
        public string? FallbackMessage { get; private set; }
        public string? GreetingMessage { get; private set; }
        public string[]? Enquiries { get; private set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Project(Guid id, string name, Guid organisationId, string? widgetTitle, string widgetColor,
            string? widgetDescription, string? fallbackMessage, string? greetingMessage, string[]? enquiries)
        {
            Id = id;
            Name = name;
            OrganisationId = organisationId;
            WidgetTitle = widgetTitle;
            WidgetColor = widgetColor;
            WidgetDescription = widgetDescription;
            FallbackMessage = fallbackMessage;
            GreetingMessage = greetingMessage;
            Enquiries = enquiries;
        }

        public void UpdateWidgetTitle(string widgetTitle)
        {
            if (!string.IsNullOrEmpty(widgetTitle) && widgetTitle.Length > 255)
                throw new BadRequestException($"{nameof(widgetTitle)}'s length cannot exceed 255");
            WidgetTitle = widgetTitle;
        }

        public void UpdateWidgetColor(string widgetColor)
        {
            if (string.IsNullOrWhiteSpace(widgetColor))
                throw new BadRequestException($"{nameof(widgetColor)} cannot be empty");
            WidgetColor = widgetColor; 
        }

        public void UpdateWidgetDescription(string widgetDescription)
        {
            WidgetDescription = widgetDescription;
        }

        public void UpdateFallbackMessage(string fallbackMessage)
        {
            FallbackMessage = fallbackMessage;
        }

        public void UpdateGreetingMessage(string greetingMessage)
        {
            GreetingMessage = greetingMessage;
        }

        public void UpdateEnquiries(string[] enquiries)
        {
            Enquiries = (enquiries ?? new string[]{})
                .OrderBy(e => e)
                .Distinct().ToArray();
        }
    }
}