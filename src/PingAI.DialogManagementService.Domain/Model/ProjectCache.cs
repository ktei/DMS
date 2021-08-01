using System;
using System.Collections.Generic;
using System.Linq;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class ProjectCache
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid OrganisationId { get; set; }
        public string? WidgetTitle { get; set; }
        public string? WidgetColor { get; set; }
        public string? WidgetDescription { get; set; }
        public string? FallbackMessage { get; set; }
        public string? GreetingMessage { get; set; }
        public string[]? QuickReplies { get; set; }
        public string[]? Enquiries { get; set; }
        public string[]? Domains { get; set; }
        public string BusinessTimezone { get; set; }
        public DateTime? BusinessTimeStartUtc { get; set; }
        public DateTime? BusinessTimeEndUtc { get; set; }
        public string? BusinessEmail { get; set; }
        public DateTime CreatedAt { get; set; }

        public string GetKey() => ProjectCache.MakeKey(Id);

        public static string MakeKey(Guid projectId) => $"project:{projectId}";

        public static ProjectCache FromProject(Project project)
        {
            var quickReplies = new List<string>();
            foreach (var gr in project.GreetingResponses
                .Where(x => x.Response!.Type == ResponseType.QUICK_REPLY)
                .OrderBy(x => x.Response!.Order))
            {
                quickReplies.Add(gr.Response!.GetDisplayText());
            }

            var cache = new ProjectCache
            {
                Id = project.Id,
                Name = project.Name,
                OrganisationId = project.OrganisationId,
                WidgetTitle = project.WidgetTitle,
                WidgetColor = project.WidgetColor,
                WidgetDescription = project.WidgetDescription,
                FallbackMessage = project.FallbackMessage,
                Enquiries = project.Enquiries,
                Domains = project.Domains,
                BusinessTimezone = project.BusinessTimezone,
                BusinessTimeStartUtc = project.BusinessTimeStartUtc,
                BusinessTimeEndUtc = project.BusinessTimeEndUtc,
                BusinessEmail = project.BusinessEmail,

                GreetingMessage = project.GreetingResponses.FirstOrDefault(gr =>
                    gr.Response!.Type == ResponseType.RTE)?.Response!.GetDisplayText(),

                QuickReplies = quickReplies.ToArray()
            };

            return cache;
        }

        public ProjectCache()
        {
        }
    }
}
