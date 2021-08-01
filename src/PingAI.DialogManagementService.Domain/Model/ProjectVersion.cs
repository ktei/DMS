using System;
using PingAI.DialogManagementService.Domain.Utils;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class ProjectVersion : DomainEntity
    {
        public Guid Id { get; private set; }

        public Guid OrganisationId { get; private set; }

        public Guid ProjectId { get; private set; }

        public Project Project { get; private set; }

        public Guid VersionGroupId { get; private set; }
        
        public ProjectVersionNumber Version { get; private set; }

        public ProjectVersion(Guid projectId, Guid organisationId, Guid versionGroupId, ProjectVersionNumber version)
        {
            if (projectId.IsEmpty())
                throw new ArgumentException($"{nameof(projectId)} cannot be empty.");
            Id = Guid.NewGuid();
            ProjectId = projectId;
            OrganisationId = organisationId;
            VersionGroupId = versionGroupId;
            Version = version;
        }

        public static ProjectVersion NewDesignTime(Project project) =>
            new ProjectVersion(project.Id, project.OrganisationId, 
                project.Id, ProjectVersionNumber.NewDesignTime());

        public ProjectVersion Next(Guid projectId)
        {
            return new ProjectVersion(projectId, OrganisationId, VersionGroupId, Version.Next());
        }
    }
}
