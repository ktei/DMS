using System;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class ProjectVersion : DomainEntity, IHaveTimestamps
    {
        public Guid Id { get; private set; }
        
        public Guid ProjectId { get; private set; }
        
        public Project? Project { get; private set; }
        
        public Guid VersionGroupId { get; private set; }
        
        public int Version { get; private set; }

        public ProjectVersion(Guid projectId, Guid versionGroupId, int version)
        {
            ProjectId = projectId;
            VersionGroupId = versionGroupId;
            Version = version;
        }
    }
}