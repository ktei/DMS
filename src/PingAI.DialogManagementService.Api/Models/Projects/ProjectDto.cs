using System;
using System.Globalization;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Api.Models.Projects
{
    public class ProjectDto
    {
        public string ProjectId { get; set; }
        public string Name { get; set; }
        public string? WidgetTitle { get; set; }
        public string WidgetColor { get; set; }
        public string? WidgetDescription { get; set; }
        public string? FallbackMessage { get; set; }
        public string? GreetingMessage { get; set; }
        public string[] Enquiries { get; set; }
        public string? ApiKey { get; set; }
        public string[] Domains { get; set; }
        public string? BusinessTimeStart { get; set; }
        public string? BusinessTimeEnd { get; set; }
        public string BusinessTimezone { get; set; }
        public string? BusinessEmail { get; set; }

        public ProjectDto(string projectId, string name, string? widgetTitle, string widgetColor,
            string? widgetDescription,
            string? fallbackMessage, string? greetingMessage, string[] enquiries, string? apiKey, string[] domains,
            string? businessTimeStart, string? businessTimeEnd, string businessTimezone, string? businessEmail)
        {
            ProjectId = projectId;
            Name = name;
            WidgetTitle = widgetTitle;
            WidgetColor = widgetColor;
            WidgetDescription = widgetDescription;
            FallbackMessage = fallbackMessage;
            GreetingMessage = greetingMessage;
            Enquiries = enquiries;
            ApiKey = apiKey;
            Domains = domains;
            BusinessTimeStart = businessTimeStart;
            BusinessTimeEnd = businessTimeEnd;
            BusinessTimezone = businessTimezone;
            BusinessEmail = businessEmail;
        }

        public ProjectDto(Project project)
        {
            _ = project ?? throw new ArgumentNullException(nameof(project));
            ProjectId = project.Id.ToString();
            Name = project.Name;
            WidgetTitle = project.WidgetTitle;
            WidgetColor = project.WidgetColor ?? string.Empty;
            WidgetDescription = project.WidgetDescription;
            FallbackMessage = project.FallbackMessage;
            GreetingMessage = project.GreetingMessage;
            Enquiries = project.Enquiries ?? new string[0];
            ApiKey = project.ApiKey?.Key;
            Domains = project.Domains ?? new string[0];
            BusinessTimeStart = project.BusinessTimeStartUtc.HasValue
                ? ConvertUtcToString(project.BusinessTimeStartUtc.Value)
                : null;
            BusinessTimeEnd = project.BusinessTimeEndUtc.HasValue
                ? ConvertUtcToString(project.BusinessTimeEndUtc.Value)
                : null;
            BusinessTimezone = project.BusinessTimezone;
            BusinessEmail = project.BusinessEmail;
        }

        public static DateTime? TryConvertStringToUtc(string s) =>
            DateTime.TryParseExact(s,
                "yyyy-MM-ddTHH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var result)
                ? result
                : default(DateTime?);

        public static string ConvertUtcToString(DateTime utc) =>
            utc.ToString("yyyy-MM-ddTHH:mm:ss");

        public ProjectDto()
        {
            
        }
    }
}