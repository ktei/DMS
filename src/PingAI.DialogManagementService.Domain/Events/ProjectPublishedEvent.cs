using System;

namespace PingAI.DialogManagementService.Domain.Events
{
    public class ProjectPublishedEvent : DomainEvent
    {
        public Guid ProjectId { get; }

        public ProjectPublishedEvent(Guid projectId)
        {
            ProjectId = projectId;
        }
    }
}