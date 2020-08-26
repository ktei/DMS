using System;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class ProjectVersion : DomainEntity, IHaveTimestamps
    {
        public Guid Id { get; private set; }
        
        public Guid OrganisationId { get; private set; }
        
        public Organisation? Organisation { get; private set; }
        
        public Guid ProjectId { get; private set; }
        
        public Project? Project { get; private set; }
        
        public Guid VersionGroupId { get; private set; }
        
        public ProjectVersionNumber Version { get; private set; }

        public ProjectVersion(Guid projectId, Guid organisationId, Guid versionGroupId, ProjectVersionNumber version)
        {
            ProjectId = projectId;
            OrganisationId = organisationId;
            VersionGroupId = versionGroupId;
            Version = version;
        }
        
        public ProjectVersion(Project project, Guid organisationId, Guid versionGroupId, ProjectVersionNumber version)
        {
            Project = project ?? throw new ArgumentNullException(nameof(project));
            OrganisationId = organisationId;
            VersionGroupId = versionGroupId;
            Version = version;
        }
    }
}