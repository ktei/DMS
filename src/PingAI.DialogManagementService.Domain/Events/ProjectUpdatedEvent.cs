using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Domain.Events
{
    public class ProjectUpdatedEvent : DomainEvent
    {
        public Project Project { get; set; }

        public ProjectUpdatedEvent(Project project)
        {
            Project = project;
        }

        public ProjectUpdatedEvent()
        {
            
        }
    }
}