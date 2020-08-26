using System;
using PingAI.DialogManagementService.Domain.Model;

namespace PingAI.DialogManagementService.Domain.Events
{
    public class ProjectPublishedEvent : DomainEvent
    {
        public Project SourceProject { get; }
        public Project PublishedProject { get; }

        public ProjectPublishedEvent(Project sourceProject, Project publishedProject)
        {
            SourceProject = sourceProject;
            PublishedProject = publishedProject;
        }
    }
}